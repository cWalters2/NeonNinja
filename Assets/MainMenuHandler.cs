using UnityEngine;
using System.Collections;

public class MainMenuHandler : MonoBehaviour {
	
	public GameObject stage;
	public GameObject Kiroch;

	public AudioSource moveAud;
	public AudioSource selAud;
	public AudioSource deselAud;
	float timeLen = 0.4f;
	float lTime = 0.4f;
	float sTime = 0.1f;
	SPoint[] mLoc;
	STimer inputTmr;
	int numPos;
	int menuSel;
	float vThresh;
	// Use this for initialization
	void Start () {
		numPos = 4;
		menuSel = 1;
		vThresh = 0.5f;
		mLoc = new SPoint [4];
		mLoc [0] = new SPoint (100.0f, 5.0f);
		mLoc [1] = new SPoint (100.0f,-35.0f);
		mLoc [2] = new SPoint (100.0f, -75.0f);
		mLoc [3] = new SPoint (100.0f, -115.0f);
		inputTmr = new STimer (0.3f);
		if (Time.deltaTime == 0)
			Time.timeScale = 1;
		
	}
	
	// Update is called once per frame
void Update () {
	float axis = Input.GetAxis ("LStickY1") ;
	if (Input.GetKey ("up"))
		axis = 1;
	else if (Input.GetKey ("down"))
	axis = -1;
		if(inputTmr.IsReady ()){
			if (axis  > vThresh){
				menuSel--;
				inputTmr.SetTimer(timeLen);
				timeLen=sTime;
				moveAud.Play();
			}
			else if (axis < -vThresh){
				menuSel++;
				inputTmr.SetTimer(timeLen);
				timeLen=sTime;
				moveAud.Play();
			}else
				timeLen=lTime;
		}else
			inputTmr.RunTimer(Time.deltaTime);
		if (menuSel >= numPos)
			menuSel = menuSel % numPos;
		if (menuSel < 0)
			menuSel = menuSel + numPos;
		Vector3 lookVec = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
		transform.localPosition=new Vector3(transform.localPosition.x, mLoc[menuSel].y, -0.0f);
		if(Input.GetButton("select"))
			MenuSelect (menuSel);
	}
	
	void MenuSelect(int sel){
		selAud.Stop ();
		selAud.Play ();
		if(sel==0)
			Application.LoadLevel("charselect");
		if(sel==1)
			Application.LoadLevel("demoselect");
		if (sel == 2) {
            MatchHelper mh = GameObject.FindGameObjectWithTag ("MatchHelper").GetComponent ("MatchHelper") as MatchHelper;
			mh.StartQuickMatch();
		}
		if(sel==3)
			Application.LoadLevel("options");
	}
}
