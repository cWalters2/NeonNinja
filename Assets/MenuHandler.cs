using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	public GameObject stage;
	public GameObject Kiroch;
	SPoint[] mLoc;
	STimer inputTmr;
	int numPos;
	int menuSel;
	float vThresh;
	// Use this for initialization
	void Start () {
		numPos = 4;
		menuSel = 0;
		vThresh = 0.5f;
		mLoc = new SPoint [4];
		mLoc [0] = new SPoint (-7.1f, 0.35f);
		mLoc [1] = new SPoint (-7.1f, -0.33f);
		mLoc [2] = new SPoint (-7.1f, -0.956f);
		mLoc [3] = new SPoint (-7.1f, -1.69f);
		inputTmr = new STimer (0.4f);

	}
	
	// Update is called once per frame
	void Update () {
		float axis = Input.GetAxis ("LStickY1") ;
		if(inputTmr.IsReady ()){
		if (axis  > vThresh){
			menuSel++;
				inputTmr.SetTimer();
			}
		if (axis < -vThresh){
			menuSel--;
			inputTmr.SetTimer();
			}
		}else
			inputTmr.RunTimer(Time.deltaTime);
		if (menuSel >= numPos)
			menuSel = menuSel % numPos;
		if (menuSel < 0)
			menuSel = menuSel + numPos;
		transform.position=new Vector3(mLoc[menuSel].x, mLoc[menuSel].y, -0.2f);
		if(Input.GetButton("select"))
			MenuSelect (menuSel);
	}

	void MenuSelect(int sel){
		if (sel == 1) {
			GameObject pl1 = Instantiate(Resources.Load("Yara")) as GameObject;
		
			GameObject pl2 = Instantiate(Resources.Load("Kiroch")) as GameObject;
			stage = Instantiate(Resources.Load("StRuins")) as GameObject;
			DontDestroyOnLoad(pl1);
			DontDestroyOnLoad(pl2);
			DontDestroyOnLoad(stage);
			//Stage stScr = stage.GetComponent ("stRuins") as stRuins;

			
			
		}
		if(sel==2)
			Application.LoadLevel("options");
	}
}
