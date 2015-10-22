using UnityEngine;
using System.Collections;

public class DemoSelectHandler : MonoBehaviour {

	public int plNum;
	SPoint[] mLoc;
	STimer inputTmr;
	int numPos;
	int menuSel;
	float vThresh;
	MatchHelper mh;
	public bool isActive;
	public DemoSelectHandler next;

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
	}
	
	/// Update is called once per frame
	void Update () {
		if (isActive) {
			float axis = Input.GetAxis ("LStickY1");
			if (inputTmr.IsReady ()) {
					if (Input.GetAxis ("LStickY1") > vThresh) {
							menuSel--;
							inputTmr.SetTimer ();
					moveAud.Play();
					}
					if (Input.GetAxis ("LStickY1") < -vThresh) {
							menuSel++;
							inputTmr.SetTimer ();
					moveAud.Play();
					}
			} else
					inputTmr.RunTimer (Time.deltaTime);
			if (menuSel >= numPos)
					menuSel = menuSel % numPos;
			if (menuSel < 0)
					menuSel = menuSel + numPos;
			transform.localPosition = new Vector3 (mLoc [menuSel].x, mLoc [menuSel].y, -0.0f);
			if((inputTmr.IsReady())&& (Input.GetButton ("select"))){
					MenuSelect (menuSel);
					inputTmr.SetTimer();
					}
				}
	}
	
	void MenuSelect(int sel){

		if(isActive){
		if (sel == 3){
				deselAud.Play();
			Application.LoadLevel ("menu");
				}
		else {

			if (sel == 0) 
				mh.SelectPlayer(plNum, MatchHelper.SERENITY);
			else if (sel == 1) 
				mh.SelectPlayer(plNum, MatchHelper.YARA);
			else if (sel == 2) 
				mh.SelectPlayer(plNum, MatchHelper.KIROCH);
				if (next != null) {
					isActive = false;
					next.isActive = true;
					next.inputTmr.SetTimer();
					selAud.Play ();
	            }else{
					 selAud.Play();
					mh.StartDemoMatch();
				}
			}
        }
	}
	
}
