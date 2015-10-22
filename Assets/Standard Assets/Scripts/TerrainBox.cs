using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class TerrainBox : BoundBox
	{
	const float SCALE_FAC = 1.0f;

	public float[][] ang;
	public bool[] isPform;
	public int numPoly, numPform;
	public Mesh[] subMesh;
	public bool[] rLipAr;
	public bool[] lLipAr;
	public bool LoadCollisionBoxes(TextAsset colDetFilename){
		bool pFormFlag = false;
		numPoly = 0;
		numPform = 0;
		 

		bool countFlag = false;
		int vCount = 0;
		int pInd = 0;
		string line;
		string[] lines = colDetFilename.text.Split('\n');
		try{
			
			foreach (string rLine in lines){
				line = rLine.Trim();
                if (line.CompareTo ("pgon") == 0)
									numPoly++;
					if (line.CompareTo ("llip") == 0)
						numPoly++;
					if (line.CompareTo ("rlip") == 0)
						numPoly++;
							if (line.CompareTo ("pform") == 0)
									numPform++;
					}
					AllocateFirstIndices (numPoly + numPform);
				subMesh=new Mesh[numPoly + numPform];


			//this time, we need the vertex count for each polygon. 

			foreach (string rLine in lines){
				line = rLine.Trim();
					if(line.CompareTo("end") == 0){
						countFlag = false;
						AllocateSecondIndices(pInd, vCount);
						isPform[pInd] = pFormFlag;
						pInd++;
						vCount = 0;
					}	
					if(countFlag)
						vCount++;
					if(line.CompareTo("pgon") == 0){    
						countFlag = true;
						pFormFlag = false;
					}
					if(line.CompareTo("llip") == 0){    
						countFlag = true;
						pFormFlag = false;
					}
					if(line.CompareTo("rlip") == 0){    
						countFlag = true;
						pFormFlag = false;
					}
					if(line.CompareTo("pform") == 0){
						countFlag = true;
						pFormFlag = true;
					}
				}
			
		}
		catch(FileNotFoundException e){
						return false;
				}
		string xVal, yVal;
		int bpInd = 0;
		int vInd = -1;

		foreach (string rLine in lines){
			line=rLine.Trim();
				if(countFlag)
					vInd++;
				if(line.CompareTo("pgon") == 0)
					countFlag = true;
				else if(line.CompareTo("end") == 0){
					countFlag = false;
					vInd = -1;  //vInd will be incrimented before it is used as an index,
					bpInd++;	//so it is reset to -1
				}else{
					xVal = line.Substring(0, line.IndexOf(' '));
					yVal = line.Substring(line.IndexOf(' ') + 1);
					SetVertex(Convert.ToSingle(xVal)*SCALE_FAC, Convert.ToSingle(yVal)*SCALE_FAC, bpInd, vInd);
				}
			}	  
		

		SPoint tp, tq;
		//fill in objects to aid in collision detection
		for(int i = 0; i < iLength; i++){
			for(int j = 0; j < GetJlength(i); j++){
				if( j != GetJlength(i) - 1){
					tp = GetSPoint(i, j+1);
					tq = GetSPoint(i, j);
					ang[i][j] = Mathf.Atan2(GetSPoint(i, j+1).y - GetSPoint(i, j).y, GetSPoint(i, j+1).x - GetSPoint(i, j).x);}
				else{
					ang[i][j] = Mathf.Atan2(GetSPoint(i, 0).y - GetSPoint(i, j).y, GetSPoint(i, 0).x - GetSPoint(i, j).x);}
			}
		}
		return true;
	}
	
	public void AllocateFirstIndices(int num){
		iLength = num;
		pBounds = new SPoint[iLength][];
		jLength = new int[iLength];
		isPform = new bool[iLength];
		ang = new float[iLength][];
		rLipAr = new bool[iLength];
		lLipAr = new bool[iLength];
		for (int i=0; i<iLength; i++) {
			rLipAr[i] = false;
			lLipAr[i] = false;
		}
	}
	
	public void AllocateSecondIndices(int ind, int num){
		jLength[ind] = num;
		pBounds[ind] = new SPoint[num];
		jLength[ind] = num;
		ang[ind] = new float[num];	
		subMesh [ind] = new Mesh ();
	}
	public float GetBoxWidth(int polyIndex){
		float xMin = 0;
		float xMax = 0;
		if (polyIndex < iLength) {


			if (jLength [polyIndex] > 0) {//sanity check
				xMin = pBounds [polyIndex] [0].x;
				xMax = xMin;
			}
			for (int j = 0; j<jLength[polyIndex]; j++) {
				if (xMin > pBounds [polyIndex] [j].x)
					xMin = pBounds [polyIndex] [j].x;
				if (xMax < pBounds [polyIndex] [j].x)
					xMax = pBounds [polyIndex] [j].x;
			}

	}
		return xMax-xMin;
	}
	public float GetAng(int i, int j){
		if((i >=0)&&(j>=0)&&(i < iLength)&&(j < jLength[i]))
			return ang[i][j];
		return 0;
	}
	public SPoint GetSPoint(int iInd, int jInd){
        int tf = 0;
        if ((iInd < 0) || (jInd < 0))
            tf = 1;
		if((iInd < iLength)&&(jInd<jLength[iInd]))
			return pBounds[iInd][jInd];
		else
			return new SPoint(0, 0);
	}
	
	public int GetJlength(int j){
		if(j < iLength)
			return jLength[j];
		return 0;
	}
	public bool Render(string mat){
		bool hr = true;
		string sName = "";
		CombineInstance[] combine = new CombineInstance[iLength];
		for (int i = 0; i < iLength; i++) {
			if(!isPform[i]){
				sName="solid";
			}else{
				sName="platform";
			}
			DrawStageBox(pBounds[i], sName, jLength[i], i);
			combine[i].mesh=subMesh[i];
		}
		//combine subMeshes
		mesh = new Mesh ();
		mesh.CombineMeshes (combine, true, false);

		/*stringstream name;
		LPCSTR lpName;
		string sName;
		HRESULT hr;
		
		for(int i = 0; i < iLength; i++){
			name.str("");
			name << "TER-" << i;
			sName = name.str().c_str();
			lpName = sName.c_str();
			if(!isPform[i])
				hr=eng->DrawTriangleFan(pBounds[i], mat, sName.c_str(), 0,jLength[i]-1, SPoint(0, 0)); 
			else
				hr=eng->DrawTriangleFan(pBounds[i], "Platform", sName.c_str(),0 ,jLength[i]-1, SPoint(0, 0)); 
		}*/
		return hr;
	}
	public bool DrawStageBox(SPoint[] v, string mat, int vTot, int ind){
		subMesh[ind].Clear();
		Vector3[] vertices = new Vector3[vTot];
		Color32[] col = new Color32[vTot];
		int vT = (vTot-2)*3;
		if (vT < 1)
			vT = 1;

		int[] tri = new int[vT];

		for (int i = 0; i<vTot; i++) {
			vertices[i]=new Vector3(v[i].x , v[i].y , 0);
			if(mat.CompareTo("platform")==0)
				col[i] = new Color32(130, 150,210,155);
			else 
				col[i] = new Color32(20, 20, 210,255);
			if(i==vT){
				i=vT;
			}
			if(i>1){//basic triangle fan
				tri[(i-2)*3]=0;
				tri[(i-2)*3+1]=i-1;
				tri[(i-2)*3+2]=i;
			}

			
		}
		subMesh[ind].vertices=vertices;
		subMesh[ind].triangles = tri;
		subMesh[ind].colors32 = col;
		return true;
	}

}
