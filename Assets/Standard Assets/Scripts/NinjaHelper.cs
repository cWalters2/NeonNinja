using UnityEngine;
using System.Collections;

public class NinjaHelper  {

	public const float MOVEMENT_THRESH = 0.1f;

	public int  atkType, lastState;
	public bool airborne;
	private bool isFacingRight;
	public float frame, animSpeed, animLoopStart;
	public string nextAnim;
	SPoint displacement = new SPoint();
	bool nLoop;

	public STimer actionTmr=new STimer();
	public STimer aniTmr=new STimer();
	GameObject player;
	public Animator anim;
	//public Animation animation;
	public Transform TR; // for transforms
	public const int MAX_TRACKS=8;

	public int seqInd=0;
	bool turnAfter=false;
	public NinjaHelper(){
		//state=0;
		lastState=0;
		
		isFacingRight=true;
		airborne=true;
		atkType=0;
		frame=0;
		nextAnim="";


	}


	public bool NextLoop(){
		if (nLoop) {
			nLoop=false;
			return true;
		}
		return false;

	}

	public int IntFacingRight(){
		//returns 1 on right -1 for left
		//useful for flipping stuff
		if(isFacingRight)
			return 1;
		else
			return -1;
	}
	public bool IsFacingRight(){
		return isFacingRight;
	}


	public void UpdateAnim(float timeLapsed){
		//if(animState)
		//	animState->addTime(timeLapsed*animSpeed);
	}
	public void WeightedAnim(float timeWgt, string animName){
		anim.Play (animName, 0, timeWgt);
		frame = timeWgt;
	}
	public void SetWeightedAnim(float timeWgt){

		anim.Play("Guard", -1, timeWgt);
	/*	if(animState){
			double cTP = animState->getTimePosition();
			animState->setTimePosition(animState->getLength()*timeWgt);
			cTP = animState->getTimePosition();
			cTP=cTP*1;
		}
	*/	
	}
	public bool InAnim(string animName){
		if (anim.GetCurrentAnimatorStateInfo (0).IsName (animName))
						return true;
		return false;
	}
	public void TurnAfterAnim(){
		//makes a note that the sprite must be pivoted 180d agter animation ends
		turnAfter = true;
	}
	public float Animate(string animName, bool loop, float startTime){
		animSpeed=1;
		if (turnAfter) {
			if(isFacingRight)
				isFacingRight=false;
			else
				isFacingRight=true;
			turnAfter=false;
			Pivot (180);
			//sanitation
			nextAnim="";
		}

		//if (animName.CompareTo ("Halt") == 0)
		//				return 0;//force exit since some lacking halt anims

		int stopAnim = (int)PlayMode.StopAll;
		if (anim.GetCurrentAnimatorStateInfo (0).IsName (animName))
						anim.ForceStateNormalizedTime (0.0f);
				else {
						
						anim.Play (animName);
						anim.Update (0.001f);
						
				}
		//anim.s
		//animation.Play (animName);
		//if(loop)
			//anim.getAwrapMode = WrapMode.Loop;
		//returns the animation length, on a successful animation
		AnimatorStateInfo holder = anim.GetCurrentAnimatorStateInfo (0);
		float retTime = anim.GetCurrentAnimatorStateInfo (0).length;

		/*if(spriteEntity->hasAnimationState(animName)){
			animState->setEnabled(false);
			animState = spriteEntity->getAnimationState(animName);
			animState->setTimePosition(startTime);
			animState->setLoop(loop);
			animState->setEnabled(true);
			retTime= animState->getLength()-startTime;
			aniTmr.SetTimer(retTime);
		}
		else{
			std::ofstream fs(ERR_LOG);
			if(fs.is_open()){
				fs << "c_Sprite could find no animation named '" << animName;
				fs << "' in Ninja " << name << "\r\n";
				fs.close();
			}
		}*/
		aniTmr.SetTimer (retTime);
		return retTime;
	}

	public void Orient(float ang){
		float tAng =( Mathf.PI* ang )/ 180.0f;
		Vector3 o = new Vector3(Mathf.Cos(tAng)+TR.position.x, TR.position.y, Mathf.Sin (tAng)+TR.position.z);
		TR.LookAt(o);
	}

		
public void SetAnimSpeed(float speed){
		animSpeed = speed;
	}

public void FaceRight(bool t){
        int faceR=1;
        Vector3 ls = TR.localScale;
        if (!t)
            faceR = -1;
		isFacingRight = t;
        TR.localScale = new Vector3(faceR*ls.x, ls.y, ls.z);
        
            

}
	public void Pivot(float ang){
		if(InAnim("EdgeBalance"))
		   Animate("Idle", true, 0);
		TR.Rotate (0, ang, 0);
		//spriteNode->yaw(Ogre::Degree(ang));//ang is a measure of degrees
	}

	//never use Translate
	//public void Translate(SPoint v){
	//	TR.Translate (v.x, v.y, 0, Space.World);	
	//}
	
	public void ReTranslate(SPoint v){
	//	if((displacement.x!=0)||(displacement.y!=0))//need to untranslate first
	//		UnTranslate();
	//	displacement = new SPoint (-v.x, -v.y);
	//	TR.Translate (v.x, v.y, 0, Space.World);
	}
	public SPoint UnTranslate(){	//reverts a retranslate
		//returns the coordinates fet to ReTranslate
	//	TR.Translate (displacement.x, displacement.y, 0, Space.World);
		SPoint rtnPt = new SPoint(displacement.x, displacement.y);
	//	displacement = new SPoint(0,0);
		return rtnPt;
	}

}
