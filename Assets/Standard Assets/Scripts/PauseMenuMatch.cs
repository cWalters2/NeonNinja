using UnityEngine;
using System.Collections;

public class PauseMenuMatch : MonoBehaviour {
	
	public GameObject pausePanel;
	public GameObject winPanel;
	SPoint[] mLoc;
	STimer inputTmr;
	int numPos;
	int menuSel;
	Stats[] ps;
	float vThresh;
	bool paused;
	bool gameWon;
	int plrP;
	float tmrT;
	// Use this for initialization
	void Awake(){
		pausePanel.SetActive (false);
		transform.Translate(new Vector3 (-1000.0f, -1000.0f,0.0f) );
		
	}
	void Start () {
		ps = new Stats[2];
		GameObject g1 = GameObject.FindGameObjectWithTag ("Player1");
		GameObject g2 = GameObject.FindGameObjectWithTag ("Player2");
		Ninja f1 = g1.GetComponent<Ninja> ();
		Ninja f2 = g2.GetComponent<Ninja> ();
		ps [0] = f1.stats;
		ps [1] = f2.stats;
		numPos = 3;
		menuSel = 0;
		vThresh = 0.3f;
		mLoc = new SPoint [4];
		mLoc [0] = new SPoint (-80.0f, 5.0f);
		mLoc [1] = new SPoint (-80.0f, -15.0f);
		mLoc [2] = new SPoint (-80.0f, -35.0f);
		paused = false;
		plrP = 0;
		inputTmr = new STimer (0.35f);
		tmrT = Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (ps [0].stocks <= 0) {
			paused=true;
			Time.timeScale=0;
			winPanel.SetActive(true);
			gameWon=true;
			//transform.Translate( new Vector3(1000.0f, 1000.0f,0.0f) );
			//pausePanel.SetActive (true);
			//inputTmr.SetTimer();	
		}
		if (gameWon) {
			if(Input.GetButtonDown("Start1"))
				ReturnToMenu();
			else if(Input.GetButtonDown("Start2"))
				ReturnToMenu();
			return;
		}
			return;
		if (!paused) {
			if(Input.GetButtonDown("Start1"))
				plrP=1;
			else if(Input.GetButtonDown("Start2"))
				plrP=2;
			if(inputTmr.IsReady()&&(plrP>0)){
				paused=true;
				Time.timeScale=0;
				transform.Translate( new Vector3(1000.0f, 1000.0f,0.0f) );
				pausePanel.SetActive (true);
				inputTmr.SetTimer();
			}else
				inputTmr.RunTimer(tmrT);
		} else {
			float axis = Input.GetAxis ("LStickY"+plrP) ;
			if(inputTmr.IsReady ()){
				if (axis  > vThresh){
					menuSel--;
					inputTmr.SetTimer();
				}
				if (axis < -vThresh){
					menuSel++;
					inputTmr.SetTimer();
				}
			}else
				inputTmr.RunTimer(tmrT);
			if (menuSel >= numPos)
				menuSel = menuSel % numPos;
			if (menuSel < 0)
				menuSel = menuSel + numPos;
			transform.localPosition=new Vector3(mLoc[menuSel].x, mLoc[menuSel].y, -0.0f);
			if(inputTmr.IsReady ()){
				if((Input.GetButton("Start"+plrP))||(Input.GetButton("A"+plrP)))
					MenuSelect (menuSel);
			}
		}
		
		
	}
	
	void MenuSelect(int sel){
		if (sel == 0)
			Unpause ();
		if (sel == 1) 
			DemoCharSwap ();
		
		if (sel == 2) {
			foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
				Destroy(o);}
			Application.LoadLevel ("menu");
		}
		
	}
	public void ReturnToMenu(){
		foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
			Destroy(o);}
		Application.LoadLevel ("menu");
	
	}
	public void DemoCharSwap(){
		//todo: this
		GameObject p1=GameObject.FindGameObjectWithTag("Player1");
		GameObject p2=GameObject.FindGameObjectWithTag("Player2");
		Ninja f1 = p1.GetComponent<Ninja> ();
		Ninja f2 = p2.GetComponent<Ninja> ();
		p1.tag = "Player2";
		f1.stats.id.num = 2;
		f1.plNum = 2;
		p2.tag = "Player1";
		f2.stats.id.num = 1;
		f2.plNum = 1;
		Unpause ();
	}
	public void Unpause(){
		paused = false;
		plrP = 0;
		transform.Translate(new Vector3 (-1000.0f, -1000.0f, 0.0f) );
		pausePanel.SetActive (false);
		Time.timeScale = 1;
		inputTmr.SetTimer ();
	}
	
}
