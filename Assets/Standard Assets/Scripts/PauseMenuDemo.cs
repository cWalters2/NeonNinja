using UnityEngine;
using System.Collections;

public class PauseMenuDemo : MonoBehaviour {
	public AudioSource audMain;
	public AudioSource audBGM;
	public AudioClip pauseSound;
	public AudioClip bgm;
	public GameObject pausePanel, winPanel;
	SPoint[] mLoc;
	STimer inputTmr;
	int numPos;
	int menuSel;
	float vThresh;
	bool paused;
	int plrP;
	float tmrT;
	// Use this for initialization
	void Awake(){
		pausePanel.SetActive (false);
		winPanel.SetActive (false);
		//transform.Translate(new Vector3 (-1000.0f, -1000.0f,0.0f) );

	}
	void Start () {
		numPos = 3;
		menuSel = 0;
		vThresh = 0.3f;
		mLoc = new SPoint [4];
		mLoc [0] = new SPoint (-60.0f, -10.0f);
		mLoc [1] = new SPoint (-60.0f, -35.0f);
		mLoc [2] = new SPoint (-60.0f, -60.0f);
		paused = false;
		plrP = 0;
		inputTmr = new STimer (0.35f);
		tmrT = Time.deltaTime;
		//transform.Translate( new Vector3(-1000.0f, -1000.0f,0.0f) );
	}
	
	// Update is called once per frame
	void Update () {
		if (!paused) {
			if(Input.GetButtonDown("Start1"))
				plrP=1;
			else if(Input.GetButtonDown("Start2"))
				plrP=2;
			if(inputTmr.IsReady()&&(plrP>0)){
				paused=true;
				tmrT = Time.deltaTime;
                Time.timeScale=0;
				transform.Translate( new Vector3(1000.0f, 1000.0f,0.0f) );
				pausePanel.SetActive (true);

				audBGM.Pause ();
				audMain.PlayOneShot(pauseSound);
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
	public void DemoCharSwap(){
		//todo: this
		GameObject p1=GameObject.FindGameObjectWithTag("Player1");
		GameObject p2=GameObject.FindGameObjectWithTag("Player2");
		Ninja f1 = p1.GetComponent<Ninja> ();
		Ninja f2 = p2.GetComponent<Ninja> ();
		f1.SetPNum (2);
		f2.SetPNum (1);
		Unpause ();
	}
	public void Unpause(){

		paused = false;
		menuSel = 0;
		plrP = 0;
		transform.Translate(new Vector3 (-1000.0f, -1000.0f, 0.0f) );
        pausePanel.SetActive (false);
		//f1.ReloadStats ();
		//f2.ReloadStats ();
		Time.timeScale = 1;
		inputTmr.SetTimer ();
		audBGM.UnPause ();
	}

}
