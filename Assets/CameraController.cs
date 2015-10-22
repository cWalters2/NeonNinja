using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	bool paused;
	SPoint at;
	float dist;
	float distTarget;
	float panSpeed;
	float zoomSpeed;
	const float defDist = 90.0f;
	public Transform TR; // for transforms
		SPoint target;
	float minDist=100.0f;
	float yFOV_Rad;
	GameObject[] player;
	float aRatio;//aspect ratio
	public int numPlrs;
	// Use this for initialization
	void Start () {
		player = new GameObject[4];
		paused = false;
		TR = transform;
		for (int i=0; i<numPlrs; i++) {
			player[i]=GameObject.FindGameObjectWithTag("Player" + (i+1));
				}

	}
	
	// Update is called once per frame
	void Update () {
		if(!paused){
			SPoint p; 
			SPoint camTgt=new SPoint();
			float plrDistMin=defDist;
			float plrDistMax=-defDist;
			if(numPlrs>1){
				for(int i = 0; i < numPlrs; i++){
					p = new SPoint(player[i].transform.position.x, player[i].transform.position.y);
					if(p.x<plrDistMin)
						plrDistMin=p.x;
					if(p.x>plrDistMax)
						plrDistMax=p.x;
					camTgt.y+=p.y;
				}
				camTgt.y=camTgt.y/numPlrs+20;
				camTgt.x=(plrDistMin+plrDistMax)/2;
				SetTarget(camTgt);
				distTarget=-(plrDistMin-plrDistMax);
			}else{
				p = new SPoint(player[0].transform.position.x, player[0].transform.position.y);
				FocusPlayer(p);
				distTarget=(defDist);
			}
			UpdatePos();
			TR.position=new Vector3(at.x, at.y, dist);
		}
	}
	public CameraController(){
		dist=defDist;

		panSpeed=3.1f;
		zoomSpeed=4.1f;
		yFOV_Rad=45.0f*(Mathf.PI/180.0f);//default for ogre's camera deg
		distTarget=90;
		at = new SPoint (0, defDist);
		target = new SPoint (0, 0);
	}
	

	
	public void UpdatePos(){
		SPoint vec = new SPoint();
		vec.y=target.y-at.y;
		vec.x=target.x-at.x;
		if(distTarget<minDist)
			distTarget=minDist;
		float movDistSq = vec.SqDistFromOrigin();
		float decel=0.5f;
		if(movDistSq>panSpeed*panSpeed){//moving too fast
			float dir = Mathf.Atan2(vec.y, vec.x);
			vec.x=Mathf.Cos(dir)*panSpeed;
			vec.y=Mathf.Sin(dir)*panSpeed;
			at.x+=vec.x*decel;
			at.y+=vec.y*decel;
		}else{
			at.x+=vec.x*decel;
			at.y+=vec.y*decel;
		}
		if(distTarget>dist)
			if(distTarget>dist+zoomSpeed)
				dist+=zoomSpeed;
		else
			dist=distTarget;
		else
			if(distTarget<dist-zoomSpeed)
				dist-=zoomSpeed;
		else
			dist=distTarget;
	}		
	
	public void SetAspectRatio(float w, float h){
		aRatio=w/h;
	}
	
	public void FocusPlayer(SPoint p){
		target=p;
		at=p;
	}
	
	public void SetTarget(SPoint p){
		target=p;
	}
	
	public void ZoomToWidth(float w){
		float h=(w/aRatio)*0.75f+10.0f;//height on screen (to work with yFOV
		distTarget = h/Mathf.Atan(yFOV_Rad);
		if(distTarget<defDist)//sanity check
			distTarget=defDist;
	}
}
