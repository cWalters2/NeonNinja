using UnityEngine;
using System.Collections;

public class HitBox : BoundBox {

	const int BOUNDS=4;
	public bool isActive=true;


	float GetAng(int i){
		float retAng = 0;
		if(i < BOUNDS){
			if( i != BOUNDS - 1)
				retAng = Mathf.Atan2(GetSPoint(0, i+1).y - GetSPoint(0, i).y, GetSPoint(0, i+1).x - GetSPoint(0, i).x);
			else
				retAng = Mathf.Atan2(GetSPoint(0, 0).y - GetSPoint(0, i).y, GetSPoint(0, 0).x - GetSPoint(0, i).x);
		}
		return retAng;
	}
	public bool Render(string mat, SPoint p, int i){
		bool hr = true;
		if(isActive)
			hr =  DrawColBox(pBounds[0], mat, p, i, GetJlength(0)); 				
		return hr;
	}
	public bool  DrawColBox(SPoint[] v, string mat,SPoint p, int pNum, int vTot){
		//specialized manual function for attackboxes
		//name derived from material name.
		mesh.Clear();
		Vector3[] vertices = new Vector3[vTot];
		Color32[] col = new Color32[vTot];

		for (int i = 0; i<vTot; i++) {
			vertices[i]=new Vector3(v[i].x + p.x, v[i].y + p.y, 0);
			if(mat.CompareTo("HitBox")==0)
				col[i] = new Color32(30, 250,10,155);
			else if(mat.CompareTo("AttkBox")==0)
				col[i] = new Color32(250,30,10,255);
			else if(mat.CompareTo("IsHit")==0)
			        col[i] = new Color32(230, 210,210,255);

		}
		int[] tri = new int[6];
		if (vTot == 6)
			tri = new int[12];
		tri [0] = 0;
		tri [1] = 1;
		tri [2] = 2;
		tri [3] = 0;
		tri [4] = 2;
		tri [5] = 3;

		if (vTot == 6) {
			tri = new int[12];
			tri [0] = 0;
			tri [1] = 1;
			tri [2] = 2;
			tri [3] = 0;
			tri [4] = 2;
			tri [5] = 3;
			tri [6] = 0;
			tri [7] = 3;
			tri [8] = 5;
			tri [9] = 3;
			tri [10] = 4;
			tri [11] = 5;
		
		}
		mesh.vertices=vertices;
		mesh.triangles = tri;
		mesh.colors32 = col;
		return true;

	}
}
