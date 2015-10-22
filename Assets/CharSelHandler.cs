using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CharSelHandler : MonoBehaviour {
	public int plNum;

	SPoint[] mLoc;
	STimer inputTmr;
	bool isSelected;
	int numPos;
	int menuSel;
	float vThresh;
	public RawImage curs;
	MatchHelper mh;
	public AudioSource moveAud;
	public AudioSource selAud;
	public AudioSource deselAud;
	// Use this for initialization
	void Start () {
		mh = GameObject.FindGameObjectWithTag ("MatchHelper").GetComponent ("MatchHelper") as MatchHelper;
		numPos = 4;
		menuSel = 0;
		vThresh = 0.5f;
		mLoc = new SPoint [4];
		mLoc [0] = new SPoint (-280.0f, 120.0f);
		mLoc [1] = new SPoint (-280.0f, 60.0f);
		mLoc [2] = new SPoint (-280.0f, 0.0f);
		mLoc [3] = new SPoint (-280.0f, -100.0f);
		inputTmr = new STimer (0.2f);
		isSelected = false;
	}
	
	/// Update is called once per frame
	void Update () {
		if (!isSelected) {
						float axis = Input.GetAxis ("LStickY" + plNum);
						if (inputTmr.IsReady ()) {
								if (Input.GetAxis ("LStickY" + plNum) > vThresh) {
										menuSel--;
										inputTmr.SetTimer ();
										moveAud.Play ();
								}
								if (Input.GetAxis ("LStickY" + plNum) < -vThresh) {
										menuSel++;
										inputTmr.SetTimer ();
										moveAud.Play ();
								}
						} else
								inputTmr.RunTimer (Time.deltaTime);
						if (menuSel >= numPos)
								menuSel = menuSel % numPos;
						if (menuSel < 0)
								menuSel = menuSel + numPos;
						transform.localPosition = new Vector3 (mLoc [menuSel].x, mLoc [menuSel].y, -0.0f);
						if (Input.GetButton ("A" + plNum))
								MenuSelect (menuSel);
				} else {
			if (Input.GetButtonDown ("B" + plNum)){
				isSelected=false;
				curs.color=Color.white;}
		}
	}
	
	void MenuSelect(int sel){
		//if(sel==0)
		if (sel == 3) {
				Application.LoadLevel ("menu");
			deselAud.Play ();
				}
		else {
			selAud.Play ();
			if (sel == 0) 
					mh.SelectPlayer (plNum-1, MatchHelper.SERENITY);
			else if (sel == 1) 
					mh.SelectPlayer (plNum-1, MatchHelper.YARA);
			else if (sel == 2) 
					mh.SelectPlayer (plNum-1, MatchHelper.KIROCH);
			isSelected=true;
			if(plNum==1)
				curs.color=Color.red;
			else
				curs.color=Color.green;
			if(mh.IsReady (2))
				mh.StartMatch ();
		}
            
	}
	
}
