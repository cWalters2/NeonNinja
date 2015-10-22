using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MatchConsole : MonoBehaviour {
	public Material attMat;
	public Material boxMat;

	Text t;

	//HitDisplay hd;
	// Use this for initialization
	void Start () {
		t = GetComponent<Text>();
		
	}

	// Update is called once per frame
	void Update () {
	
	}
	public void PostToConsole(string s){

		t.text = t.text + s + "/r/n";


	}

}
