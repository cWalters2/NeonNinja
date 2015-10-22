using UnityEngine;
using System.Collections;


public class AttkData{
	 const int MAX_VERTS = 50;

	int vCount;
	public bool isThrow;
	public bool isVacuum;
	public int throwType;
	public float mag, dir, dmg, wgt, adir;
	public float xMin, xMax, yMin, yMax;
	public float wid, hgt, ctr, scl; //derived values
	public float perimeter;//used to average dir
	public float dirHold, lastDist, firstDist;
	public float tSz;//excess size of window 
	public bool noHit;//flag set when no hit should trigger
	public int attLen; //length of attack used
	public SPoint[] pBds; //player bounds
	public SPoint[][] aBds;//attack box in full
	public SPoint oriPt;
	public SPoint cenPt;
	public int arrLmt, curInd, aBdLen;
	public AttkVtx[] vtxArr = new AttkVtx[MAX_VERTS];
	public string hitCap;
	
	public AttkData(){
		arrLmt = 0;
		isThrow = false;
	}
	
	public AttkData(int arrNum){
		noHit=false;
		isThrow = false;
		isVacuum = false;
		mag=0;
		dir=0;
		dmg=0;
		wgt=1;
		adir=1;
		vCount=0;
		dirHold=0;
		lastDist=0;
		firstDist=0;
		perimeter=0;
		curInd=0;
		if(arrNum>MAX_VERTS/2)
			arrNum=MAX_VERTS/2;
		attLen=arrNum;
		arrLmt = arrNum*2 +1;
		//vtxArr = new AttkVtx[arrLmt];
		int i=4;
		cenPt = new SPoint (0, 0);
		pBds = new SPoint[i];
		tSz=3;//determines the 'containment box' around the attack data
		// will occupy 1/tSz of the availiable window
	}
	

	
	public SPoint[] GetPlrArr(){
		SPoint[] rtnArr = new SPoint[curInd];
		float offSet = (1-(1/tSz) )/2;
		for(int i=0;i<curInd;i++)
			rtnArr[i] = new SPoint((pBds[i].x-xMin)/scl + offSet,(pBds[i].y-yMin)/scl + offSet); 
		return rtnArr;
	}
	
	public SPoint[] GetSPArr(){
		SPoint[] rtnArr = new SPoint[curInd];
		float offSet = (1-(1/tSz) )/2;
		for(int i=0;i<curInd;i++)
			rtnArr[i] = new SPoint((vtxArr[i].pos.x-xMin)/scl + offSet,(vtxArr[i].pos.y-yMin)/scl + offSet); 
		return rtnArr;
	}
	
	public void SetConOrigin(SPoint p){
		float offSet = (1-(1/tSz) )/2;
		oriPt = new SPoint((p.x-xMin)/scl + offSet, (p.y-yMin)/scl + offSet);
	}
	
	public SPoint GetConOrigin(){
		return oriPt;
	}
	
	public float GetConInter(){
		return 1/scl;
	}
	
	public void PlayerHitBoxInput(SPoint[] s){
		float offSet = (1-(1/tSz) )/2;
		for(int i=0;i<4;i++){
			pBds[i] = new SPoint((s[i].x-xMin)/scl +offSet, (s[i].y-yMin)/scl +offSet );
			if(pBds[i].x<0)
				pBds[i].x=0;
			if(pBds[i].y<0)
				pBds[i].y=0;
			if(pBds[i].x>1)
				pBds[i].x=1;
			if(pBds[i].y>1)
				pBds[i].y=1;
		}
	}
	
	public void SetAVals(SPoint p){
		//used to calculate wgt=0;
		//p is the vector from the attk centreSPoint to the opponent
		adir = Mathf.Atan2(p.y, p.x);
		cenPt = p;
	}
	
	public void addVal(SPoint p, float m, float di, float diDist, float dm, float w){
		vtxArr[curInd]= new AttkVtx(p, m, di, dm, w);
		curInd++;
		if(vCount==0){//indicates a reset
			dir=0;
			wgt=0;
			perimeter=0;
			diDist=0;
			xMin = p.x;
			xMax = p.x;
			yMin = p.y;
			yMax = p.y;
		}
		dir+=di;
		perimeter+=1;
		wgt+=w;
		vCount++;
		if(m>mag)
			mag=m;
		if(dm>dmg)
			dmg=dm;
		if(xMin>p.x)
			xMin=p.x;
		if(xMax<p.x)
			xMax = p.x;
		if(yMin>p.y)
			yMin=p.y;
		if(yMax<p.y)
			yMax=p.y;
	}
	
	public void rollIntoAvg(){
		//used to average the aggregate dir sum
		//and condition the value between -pi and pi
		if(vCount>0){
			wid=xMax-xMin;
			hgt = yMax-yMin;
			if(hgt>wid)
				scl = hgt*tSz;
			else
				scl = wid*tSz;
			dir=dir/perimeter;
			wgt=wgt/vCount;
			vCount=0;
			perimeter=0;
			if(dir>Mathf.PI)
				dir-= 2*Mathf.PI;
			else if(dir<-Mathf.PI)
				dir+=2*Mathf.PI;
			dir = (1-wgt)*dir + wgt*adir;
		}
	}
	
	public string ResultString(){
		string s="";
		s ="dmg " + dmg + "\r\n" + "wgt " + wgt + "\r\n"; 
		s += "dir " + dir + "\r\n" + "mag " + mag + "\r\n";
		s += "adir " + adir + "\r\n";
		return s;
	}
};

