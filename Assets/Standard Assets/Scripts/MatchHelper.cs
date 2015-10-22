using UnityEngine;
using System.Collections;

public class MatchHelper : MonoBehaviour {
	//physics settings
	public float tumble_RollThresh;//the threshold to roll instead of bounce on tumble landing
	public float tumble_ReboundThresh;//threshold for maximum reflection vs shallow reflection
	public float tumble_bounceRatio;//variable to determine bounce off tumble reflection
	public float magnitude_scale;
	public float DI_scale;
	public float DI_reduce;
	public const int SERENITY = 1;
	public const int SERENITYSWORD = 111;
	public const int YARA = 2;
	public const int KIROCH = 3;
	public AudioSource deselAud;
	public int stocks;
	public int numPlr;
	public int stunMult;
	public int[] cSel;
	public Ninja[] plr;
	public bool debugMode;
	public bool isMatch=false;
	// Use this for initialization
	void Start () {
		plr = new Ninja[4];
		DontDestroyOnLoad (this);
		GameObject[] mhr = GameObject.FindGameObjectsWithTag("MatchHelper"); 
		if (mhr.Length > 1) {
			deselAud.Play ();
			Destroy (mhr[1]);
		}

		


	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void   PlaySound()
	{
		//?audio.Play();
	}
	protected void SetPhysVars (Ninja plr){
		//sanitize
		if (DI_reduce == 0)
			DI_reduce = 1;//avoid div/0
		plr.SetPhysVars (tumble_ReboundThresh, tumble_RollThresh, tumble_bounceRatio, magnitude_scale, DI_scale, DI_reduce);
		 
	}
	public void SelectPlayer(int pNum, int cNum){
		cSel[pNum]=cNum;
	}
	public void StartQuickMatch(){
		 
		string[] plSel;
		string[] clSel;
		Ninja[] plScr;

        plSel = new string[2];
		plScr = new Ninja[2];
		plSel[ 0]="Yara";
		plSel[1]="Serenity";
		cSel [0] = YARA;
		cSel [1] = SERENITY;
		GameObject pl1 = Instantiate(Resources.Load(plSel[0])) as GameObject;
		GameObject pl2 = Instantiate(Resources.Load(plSel[1])) as GameObject;
		//GameObject stage = Instantiate(Resources.Load("StRuins")) as GameObject;
		GameObject hud;
		if(debugMode)
			hud = Instantiate (Resources.Load ("debugHud")) as GameObject;
		else
			hud = Instantiate (Resources.Load ("matchHud")) as GameObject;
		DontDestroyOnLoad(pl1);
		DontDestroyOnLoad(pl2);
		//DontDestroyOnLoad(stage);
		DontDestroyOnLoad(hud);
		//Stage stScr = stage.GetComponent ("stRuins") as stRuins;
		
		
		
		plScr[0].SetPNum(1);
		plScr [1].SetPNum (2);
		plScr [0].SetStunMult (stunMult);
		plScr [1].SetStunMult (stunMult);
		plScr[0].debugModeOn=debugMode;
		plScr[1].debugModeOn = debugMode;
        plScr [0].stats.damage = 150.0f;
        plScr[1].stats.damage = 150.0f;

        
        //set physics vars here
        SetPhysVars (plScr [0]);
        SetPhysVars (plScr [1]);

        Application.LoadLevel("stonyruins");
		}
	public void StartDemoMatch(){
		string[] plSel;
		string[] clSel;
		Ninja[] plScr;

		plSel = new string[4];
		plScr = new Ninja[4];
		for(int i=0;i<numPlr; i++){
			if(cSel[i]==YARA)
				plSel[i]="Yara";
			else if(cSel[i]==SERENITY)
				plSel[i]="Serenity";
			else if(cSel[i]==KIROCH)
				plSel[i]="Kiroch";
		}
		GameObject pl1 = Instantiate(Resources.Load(plSel[0])) as GameObject;
	    GameObject pl2 = Instantiate(Resources.Load(plSel[1])) as GameObject;
		//GameObject stage = Instantiate(Resources.Load("StRuins")) as GameObject;
		GameObject hud;
		if(debugMode)
			hud = Instantiate (Resources.Load ("debugHud")) as GameObject;
		else
			hud = Instantiate (Resources.Load ("matchHud")) as GameObject;
		DontDestroyOnLoad(pl1);
		DontDestroyOnLoad(pl2);
		//DontDestroyOnLoad(stage);
		DontDestroyOnLoad(hud);
		//Stage stScr = stage.GetComponent ("stRuins") as stRuins;

		

	
		plScr[0].SetPNum(1);
		plScr [1].SetPNum (2);
		plScr[0].debugModeOn=debugMode;
		plScr[1].debugModeOn = debugMode;
		plScr [0].stats.damage = 150.0f;
		plScr[1].stats.damage = 150.0f;

		//stScr.player[0]=plScr[0];
		//stScr.player[1]=plScr[1];

		//set physics vars here
		SetPhysVars (plScr [0]);
		SetPhysVars (plScr [1]);
		//Application.LoadLevel("fightmatch");
		Application.LoadLevel("stonyruins");

	}
	public void StartMatch(){
		string[] plSel;
		string[] clSel;
		Ninja[] plScr;
		isMatch = true;
		plSel = new string[4];
		plScr = new Ninja[4];
		for(int i=0;i<numPlr; i++){
			if(cSel[i]==YARA)
				plSel[i]="Yara";
			else if(cSel[i]==SERENITY)
				plSel[i]="Serenity";
			else if(cSel[i]==KIROCH)
				plSel[i]="Kiroch";
		}
		GameObject pl1 = Instantiate(Resources.Load(plSel[0])) as GameObject;
		GameObject pl2 = Instantiate(Resources.Load(plSel[1])) as GameObject;
		//GameObject stage = Instantiate(Resources.Load("StRuins")) as GameObject;
		GameObject hud;
		if(debugMode)
			hud = Instantiate (Resources.Load ("debugHud")) as GameObject;
		else
			hud = Instantiate (Resources.Load ("matchHud")) as GameObject;
		DontDestroyOnLoad(pl1);
		DontDestroyOnLoad(pl2);
		//DontDestroyOnLoad(stage);
		DontDestroyOnLoad(hud);
		//Stage stScr = stage.GetComponent ("stRuins") as stRuins;

		
		
		plScr [0].SetStunMult (stunMult);
		plScr [1].SetStunMult (stunMult);
		plScr [0].stats.stocks = stocks;
		plScr [1].stats.stocks = stocks;
		
		plScr[0].SetPNum(1);
		plScr [1].SetPNum (2);
		plScr[0].debugModeOn=true;
		plScr[1].debugModeOn = true;
		plScr [0].stats.damage = 0.0f;
		plScr[1].stats.damage = 0.0f;
		//stScr.player[0]=plScr[0];
		//stScr.player[1]=plScr[1];
		
		//set physics vars here
		SetPhysVars (plScr [0]);
		SetPhysVars (plScr [1]);

		//Application.LoadLevel("fightmatch");
		Application.LoadLevel("stonyruins");

	}
	public bool IsReady(int nPlr){
		bool check = true;
		for (int i=0; i<nPlr; i++) {
			if(cSel[i]==0)	
				check=false;
		}
		return check;

	}

	public void SetDebugMode(bool b){
		debugMode=b;
	}
	public bool GetDebugMode(){
		return debugMode;
    }
	public void SetStocks(int s){
		stocks = s;
	}public int GetStocks(){
		return stocks;
    }
}
