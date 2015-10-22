using UnityEngine;
using System.Collections;

public class BoundBox {
	public int iLength;
	public int[] jLength;

	public SPoint[][] pBounds;
	public Mesh mesh;
	public BoundBox(){
		mesh = new Mesh();
		iLength=0;
		//pBounds=NULL;
		//jLength=NULL;
	}

	public void AllocateFirstIndices(int num){
		iLength = num;
		pBounds = new SPoint[iLength][];
		jLength = new int[iLength];


	}
	public void AllocateSecondIndices(int ind, int num){
		jLength[ind] = num;
		pBounds[ind] = new SPoint[num];	
	}
	public SPoint GetSPoint(int iInd, int jInd){
		if((iInd < iLength)&&(jInd<jLength[iInd]))
			return pBounds[iInd][jInd];
		else
			return new SPoint(0, 0);
	}
	
	public void SetVertex(float px, float py, int iInd, int jInd){
		if((iInd < iLength)&&(jInd<jLength[iInd]))
			pBounds[iInd][jInd] = new SPoint(px, py);	
	}
	public int GetJlength(int j){
		if(j < iLength)
			return jLength[j];
		return 0;
	}
}
