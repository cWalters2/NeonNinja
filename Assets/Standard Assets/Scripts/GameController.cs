using UnityEngine;
using System.Collections;

public class GameController {
	public const int NUM_BUT = 9;
	public const float DBL_TAP_LMT = 0.4f;//time frame to double tap a button
	public const float TAP_LMT = 0.24f; //time a tap will remain registered for

	public const int UP = 0;
	public const int DN = 1;
	public const int LEFT = 2;
	public const int RIGHT = 3;
	public const int A = 4;
    public const int B = 5;
    public const int C = 6;
    public const int D = 7;
	public const int START = 8;
	string[] bNames = new string[NUM_BUT];
	bool[] pressed = new bool[NUM_BUT];
	float[] tapTmr = new float[NUM_BUT];
	float[] dblTapTmr = new float[NUM_BUT];
	float POLL_MAX=1000.0f;//needs to line up with the pollnum of the device
	float A_THRESH = 0.05f;
	float T_THRESH = 0.6f;
	public const float LOWTHRESH = 0.2f;
	public const float HITHRESH = 0.9f;
	int pNum;//identfier for the player
	public SPoint lStick=new SPoint();
	public SPoint rStick=new SPoint();

	GameController last;
	// Use this for initialization
	void Start () {
		for(int i=0;i<NUM_BUT;i++){
			pressed[i]=false;
			tapTmr[i]=0;
			dblTapTmr[i]=0;
		}

		pNum=-1;
		bNames[UP] = "UP";
		bNames[DN] = "DN";
		bNames[LEFT] = "Left";
		bNames[RIGHT] = "Right";
		bNames[A] = "A";
        bNames[B] = "B";
        bNames[D] = "D";
		
	}
	void SetPlayerNum(int num){
		pNum = num;
	
	}
	
	// Update is called once per frame
	public void FrameUpdate (int cNum) {
		int pN = cNum;




		float pollMax = 1;
		CloneToLast();

		lStick.x = -Input.GetAxis ("LStickX"+pN)/pollMax;													  //fecth params
		lStick.y = -Input.GetAxis ("LStickY"+pN)/pollMax;	//inverted for consistancy
		if (cNum == 1) {
			if(Input.GetKey("left"))
				lStick.x=1.0f;
			if(Input.GetKey("right"))
				lStick.x=-1.0f;
			if(Input.GetKey("up"))
				lStick.y=1.0f;
			if(Input.GetKey("down"))
				lStick.y=-1.0f;
		}
		//gCont->rStick.x = js->lRx/pollMax;													  //fecth params
		//gCont->rStick.y = js->lRy/pollMax;
		/*if(js->rgdwPOV[0]==0)
			gCont->pressed[gCont->UP]=true;
		else
			gCont->pressed[gCont->UP]=false;
		if(js->rgdwPOV[0]==18000)
			gCont->pressed[gCont->DN]=true;
		else
			gCont->pressed[gCont->DN]=false;
*/
		SetButton(A, Input.GetButton("A"+pN));
        SetButton(B, Input.GetButton("B" + pN));
        SetButton(C, Input.GetButton("C" + pN));
        SetButton(D, Input.GetButton("D"+pN));
		SetButton(START, Input.GetButton ("Start"+pN));
		if(lStick.x<-T_THRESH)
			pressed[LEFT]=true;
		else
			pressed[LEFT]=false;
		if(lStick.x>T_THRESH)
			pressed[RIGHT]=true;
		else
			pressed[RIGHT]=false;
		if(lStick.y>T_THRESH)
			pressed[UP]=true;
		else
			pressed[UP]=false;
		if(lStick.y<-T_THRESH)
			pressed[DN]=true;
		else
			pressed[DN]=false;
		
		
		//sanitize axis output
		if(Mathf.Abs(lStick.x)<A_THRESH)
			lStick.x=0;
		if(Mathf.Abs(lStick.y)<A_THRESH)
			lStick.y=0;
		if(Mathf.Abs(rStick.x)<A_THRESH)
			rStick.x=0;
		if(Mathf.Abs(rStick.y)<A_THRESH)
			rStick.y=0;

		for (int i=0; i<NUM_BUT; i++) {
			if ((pressed [i]) && (!last.pressed [i])){
				if (tapTmr [i] == 0)
					tapTmr [i] = TAP_LMT;
				else 
					dblTapTmr [i] = DBL_TAP_LMT;
			}
		}

	}
	private void SetButton(int i, bool v){
		pressed[i]=(bool)v;
	}
	public void Process(float timeLapsed){
		//processes time, temoral information processed here
		//is used to determine for how long a button can be 
		//tapped or double tapped

		for(int i=0;i<NUM_BUT;i++){
			if(dblTapTmr[i]>0)
				dblTapTmr[i]-=timeLapsed;
			if(dblTapTmr[i]<0)
				dblTapTmr[i]=0;
			if(tapTmr[i]>0)
				tapTmr[i]-=timeLapsed;
			if(tapTmr[i]<0)
				tapTmr[i]=0;
			if((pressed[i])&&(!last.pressed[i])) // fresh tap
				tapTmr[i] = TAP_LMT;
			else if((!pressed[i])&&(last.pressed[i])){//released
				dblTapTmr[i]=DBL_TAP_LMT;
			}
		}
	}
	void CloneToLast(){
		last=new GameController();
		for(int i=0;i<NUM_BUT;i++)
			last.pressed[i]=pressed[i];
		last.lStick.x = lStick.x;
		last.lStick.y = lStick.y;
		last.rStick.x = rStick.x;
		last.rStick.y = rStick.y;
	}
	public bool Pressed(int t){
		return pressed[t];
	}
	public bool Tapped(int t){
		if((tapTmr[t]>0)||TappedThisFrame(t)){
			tapTmr[t]=0;		
			return true;
		}else
			return false;
	}
	public bool Held(int t){
		return pressed[t];
	}
	public bool Tapped(string t){
		int ind = -1;
		for (int i=0; i<NUM_BUT; i++)
			if (bNames [i].CompareTo (t) == 0)
				ind = i;
		if (ind >= 0)
				return pressed [ind];
		else
				return false;
		}
	public bool TappedThisFrame(int t){
		if((pressed[t])&&(!last.pressed[t]))
			return true;
		return false;
	}
	public bool StickDblTapped(){
		if((lStick.x==-1)&&(Mathf.Abs(last.lStick.x)<T_THRESH)&&(tapTmr[2]>0))
			return true;
		if((lStick.x==1)&&(Mathf.Abs(last.lStick.x)<T_THRESH)&&(tapTmr[3]>0))
			return true;
		else
			return false;
		//clear tapTmrs if needed
		tapTmr[2]=0;
		tapTmr[3]=0;
	}
	public bool LStickTapped(){
		if((lStick.x==-1)&&(Mathf.Abs(last.lStick.x)<T_THRESH))
			return true;
		if((lStick.x==1)&&(Mathf.Abs(last.lStick.x)<T_THRESH))
			return true;
		else
			return false;
		//clear tapTmrs if needed
		tapTmr[2]=0;
		tapTmr[3]=0;
	}
}
