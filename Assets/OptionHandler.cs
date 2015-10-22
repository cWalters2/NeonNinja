using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
public class OptionHandler : MonoBehaviour {
	SPoint[] mLoc;
	STimer inputTmr;
	STimer selTmr;
	float timeLen = 0.4f;
	float lTime = 0.4f;
	float sTime = 0.1f;
	float ssTime = 0.02f;
	int ssCount=0;
	int ssLim=10;
	bool inOpt =false;
	//int useLong = 1;
	int numPos;
	int menuSel;
	int stockNum=4;
	float vThresh;
	MatchHelper mh;
	public AudioSource moveAud;
	public AudioSource selAud;
	public AudioSource deselAud;
	public bool debugRender=true;
	public Text debugRYes;
	public Text debugRNo; 
	public Text stockDisp; 
	// Use this for initialization
void Start () {
		numPos = 3;
		menuSel = 0;
		vThresh = 0.5f;
		mLoc = new SPoint [numPos];
		mLoc [0] = new SPoint (100.0f, 5.0f);
		mLoc [1] = new SPoint (100.0f, -15.0f);
		mLoc [2] = new SPoint (100.0f, -35.0f);
		inputTmr = new STimer (lTime);
		selTmr = new STimer (lTime);
		mh = GameObject.FindGameObjectWithTag ("MatchHelper").GetComponent ("MatchHelper") as MatchHelper;
		DebugSelDisplay (mh.GetDebugMode ());
		stockNum = mh.GetStocks ();
		stockDisp.text=stockNum.ToString();
}
	
	/// Update is called once per frame
	void Update () {
		float axis = Input.GetAxis ("LStickY1") ;
		if(inputTmr.IsReady ()){
			if (Input.GetAxis ("LStickY1") > vThresh){
				menuSel--;
				inputTmr.SetTimer(timeLen);
					timeLen = sTime;
				moveAud.Play ();
			}
			else if (Input.GetAxis ("LStickY1") < -vThresh){
				menuSel++;
				inputTmr.SetTimer(timeLen);
				 
				timeLen = sTime;
                moveAud.Play ();
			}else
				timeLen = lTime;
		
		}else
			inputTmr.RunTimer(Time.deltaTime);
		SubMenuNav (menuSel);
		selTmr.RunTimer (Time.deltaTime);
		if (menuSel >= numPos)
			menuSel = menuSel % numPos;
		if (menuSel < 0)
			menuSel = menuSel + numPos;
		transform.localPosition=new Vector3(transform.localPosition.x, mLoc[menuSel].y, -0.0f);
		if (Input.GetButton ("select")) {
				if (selTmr.IsReady ()) {
					MenuSelect (menuSel);
					
					selTmr.SetTimer(lTime);
			}
		}

	}
void DebugSelDisplay(bool b){
	if (debugRender != b) {
			debugRender = b;
		}
			if(b){
				debugRNo.color=Color.white;
				debugRYes.color=Color.yellow;
			}else{
				debugRYes.color=Color.white;
				debugRNo.color=Color.yellow;
			}
	
}	
void MenuSelect(int sel){
	if (sel == 0) {
			DebugSelDisplay(!debugRender);
			mh.SetDebugMode(debugRender);
			moveAud.Play();
		}
		else if (sel == 1) {
			inOpt=true;
			 
		}
	else if (sel == 2) {
						
			deselAud.Play();
			Application.LoadLevel ("menu");
				}
	}


void SubMenuNav(int sel){
	if((inputTmr.IsReady())&& (sel == 1)) {

				
			 
			if (Input.GetAxis ("LStickX1") > vThresh){
				inputTmr.SetTimer (timeLen);
				stockNum++;
				inputTmr.SetTimer(timeLen);
				ssCount++;
				timeLen = sTime;
				moveAud.Play ();
				if(ssCount<10)
					timeLen = sTime;
				else
					timeLen=ssTime;
			}
			else if (Input.GetAxis ("LStickX1") < -vThresh){
				inputTmr.SetTimer (timeLen);				stockNum--;
				inputTmr.SetTimer(timeLen);
				ssCount++;

				if(ssCount<10)
					timeLen = sTime;
				else
					timeLen=ssTime;
				moveAud.Play ();
			}else{
				timeLen = lTime;
				ssCount=0;
			}
			mh.SetStocks (stockNum);
			if(stockNum>99)
				stockNum=99;
			else if(stockNum<0)
				stockNum=0;
			stockDisp.text=stockNum.ToString();
		}
	 
	}
	 
 
}