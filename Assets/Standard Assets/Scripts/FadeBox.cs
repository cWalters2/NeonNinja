using UnityEngine;
using System.Collections;

public class FadeBox : MonoBehaviour {
	STimer fTimer;
	public GameObject fadeBoxDisp;
	public Material fadeMat;
	MeshRenderer mra;
	// Use this for initialization
	void Start () {
		fTimer = new STimer (1.0f);
		fTimer.SetTimer ();
	}
	public void SetFadeBox(GameObject f){
		fadeBoxDisp = f;

		mra = fadeBoxDisp.GetComponent<MeshRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		float timeLapsed = Time.deltaTime;
		float frame = fTimer.tmr/fTimer.GetLen ();
		if (fTimer.RunTimer (timeLapsed)) {
						Destroy (fadeBoxDisp);
						Destroy (gameObject);
				} else {
						
						mra.material.color = new Color (frame, frame, frame);
				}
		}
}
