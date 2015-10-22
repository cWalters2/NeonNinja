using UnityEngine;
using System.Collections;

public class stRuins : Stage {


	protected void Start(){
		startPos[0] = new SPoint(-24*SCALE_FAC, 20*SCALE_FAC);
		startPos[1] = new SPoint(-20*SCALE_FAC, 20*SCALE_FAC);
		startPos[2] = new  SPoint(20, 0);
		startPos[3] = new SPoint(20, 0);
		LoadStage ();
		numPlr = 1;
		for (int i=0; i<numPlr; i++) {
			player[i].fHelper.stats.id.num = i + 1;
			player[i].SetPos(startPos[i]);
		}
	}
	// Update is called once per frame

	public bool LoadStage(){

		tBox = new TerrainBox ();
		tBox.LoadCollisionBoxes("Assets/Standard Assets/Stages/stonyruins.txt");
		uBound.x = 250;
		uBound.y = 300;
		lBound.x = -300;
		lBound.y = -75;
		return true;
	}
}
