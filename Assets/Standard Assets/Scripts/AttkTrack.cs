using UnityEngine;
using System.Collections;

public class AttkTrack : BoundBox
{
 
	public AttkVtx [][] aVtx;
	public bool[][] hit;
	public float[][] atkAng;
	public bool noHit;//for when there is no hit
	public bool isRadial;
	public bool isVacuum;
	public AttkVtx centre;
	public int priority;
	public int trackType;
	public int[] polyNum;
    public const int CIRC_VNUM = 20;
	public AttkTrack( ){
		priority = -1;//just a default
		trackType=0;
		aVtx=null;
		hit=null;
		centre = null;
		polyNum=null;
		noHit=false;
		isRadial = false;
		isVacuum = false;
		atkAng=null;
	

	}
	public void SetGrabBox(SPoint loc, float wid){
		AllocateFirstIndices (1);

		AllocateSecondIndices (0, 4);
		SetVertex (new AttkVtx (new SPoint (0.0f+loc.x, 1.0f+loc.y), 0, 0, 0, 0), 0, 0);
		SetVertex (new AttkVtx (new SPoint (wid+loc.x, 1.0f+loc.y), 0, 0, 0, 0), 0, 1);
		SetVertex (new AttkVtx (new SPoint (wid+loc.x, -1.0f+loc.y), 0, 0, 0, 0), 0, 2);
		SetVertex (new AttkVtx (new SPoint (0.0f+loc.x, -1.0f+loc.y), 0, 0, 0, 0), 0, 3);
	}


	public void ResetHits(){
		for(int i=0;i<iLength;i++)
			for(int j=0;j<4;j++)
				hit[i][j]=false;
	}
	public bool HasHits(int aInd){
		for (int j=0; j<4; j++)
			if (hit [aInd] [j])
				return true;
		return false;
		}
	public void TranslateTrack(SPoint t){
		AttkVtx  a;
		for(int i = 0; i < iLength;i++)
		for(int j=0;j<jLength[i];j++){
			a =  aVtx[i][j];
			while(a!=null){
				a.pos.x+=t.x;
				a.pos.y+=t.y;
				a=a.next;
			}
		}
	}
	
	public void  AllocateFirstIndices(int num){
		iLength = num;
		jLength = new int[iLength];
		hit = new bool[iLength][];
		atkAng = new float[iLength][];
		aVtx = new AttkVtx[iLength][];
		pBounds = new SPoint[iLength][];
		centre = new AttkVtx();
		polyNum = new int[iLength];
		for(int i = 0; i < iLength;i++){
			aVtx[i]=null;
			if(iLength>1)//issues with null arrays
				atkAng[i]=null;
			pBounds[i] = null;
			jLength[i]=0;
			polyNum[i]=1;
			
			hit[i]=new bool[4];
			for(int j=0;j<4;j++)	
				hit[i][j]=false;
		}
	}
	
	public void  AllocateSecondIndices(int ind, int num){
		jLength[ind] = num;
		aVtx[ind] = new AttkVtx[num];
		atkAng[ind] = new float[num];
		pBounds[ind] = new SPoint[num];
		for (int i=0; i<num; i++) {
				atkAng [ind] [i] = 0;
			aVtx[ind][i]=new AttkVtx();
			}
	}
	
	public void  SetBreak(bool val, int iInd, int jInd){
		if((iInd < iLength)&&(jInd<jLength[iInd]))
			aVtx[iInd][jInd].brk = val;
		polyNum[iInd]++;
	}
	
	public int FetchNextBreak(int tInd, int lastB, bool isRight){
		//returns the index of the first vertex with break set to true,
		//after index lastB
		AttkVtx vertex;
		int numVerts = jLength[tInd];
		if (isRadial) {
			return lastB+CIRC_VNUM -1;
		}
		else if(isRight){ //account for direction faced
			for(int i = lastB+1; i < numVerts; i++){//iterate before; skip last break
				vertex = aVtx[tInd][i];
				if(vertex.brk)
					return i;
			}
		}else{ //mirror immage/ read the array in reverse.
			//thought; I should really write a function to reverse an array...
			for(int i = numVerts-lastB-1; i >= 0; i--){
				vertex = aVtx[tInd][i];
				if((vertex.brk)&&(i!=numVerts-lastB-1))
					return (numVerts-i)-2;//-1 because arr starts at 0; another -1 to stop before the break
			}
		}
		//if(jLength[tInd]==0)
	//		int a=0;
		return jLength[tInd]-1;//nothing found, end of hitbox
	}
	
	public void SetWeight(float weight, float frame, int  iInd, int jInd){
		AttkVtx vertex = aVtx[iInd][jInd];
		AttkVtx lastVertex = null;
		if((iInd < iLength)&&(jInd<jLength[iInd])){
			while((vertex != null)&&(vertex.frame >= 0)&&(vertex.frame != frame)){
				lastVertex = vertex;
				vertex = vertex.next;
			}
			if(vertex == null){
				vertex = new AttkVtx(lastVertex);
				vertex.frame = frame;
			}
			vertex.wgt = weight;
		}
	}
	
	public void SetMagnitude(float magn, float frame, int  iInd, int jInd){
		AttkVtx vertex = aVtx[iInd][jInd];
		AttkVtx lastVertex = null;
		if((iInd < iLength)&&(jInd<jLength[iInd])){
			while((vertex != null)&&(vertex.frame >= 0)&&(vertex.frame != frame)){
				lastVertex = vertex;
				vertex = vertex.next;
			}
			if(vertex == null){
				vertex = new AttkVtx(lastVertex);
				vertex.frame = frame;
				lastVertex.next = vertex;
			}
			vertex.mag = magn;
		}
	}
	
	public void SetDirection(float dir, float frame, int  iInd, int jInd){
		AttkVtx vertex = aVtx[iInd][jInd];
		AttkVtx lastVertex = null;
		if((iInd < iLength)&&(jInd<jLength[iInd])){
			while((vertex != null)&&(vertex.frame >= 0)&&(vertex.frame != frame)){
				lastVertex = vertex;
				vertex = vertex.next;
			}
			if(vertex == null){
				vertex = new AttkVtx(lastVertex);
				vertex.frame = frame;
				lastVertex.next = vertex;
			}
			vertex.dir = dir;
		}
	}
	public void SetRadius(float rad, float frame, int  iInd, int jInd){
		AttkVtx vertex = aVtx[iInd][jInd];
		AttkVtx lastVertex = null;
		if((iInd < iLength)&&(jInd<jLength[iInd])){
			while((vertex != null)&&(vertex.frame >= 0)&&(vertex.frame != frame)){
				lastVertex = vertex;
				vertex = vertex.next;
			}
			if(vertex == null){
				vertex = new AttkVtx(lastVertex);
				vertex.frame = frame;
				lastVertex.next = vertex;
			}
			vertex.rad = rad;
		}
	}
	public void SetDamage(float damage, float frame, int  iInd, int jInd){
		AttkVtx vertex = GetVertSPointer(iInd, jInd);
		AttkVtx lastVertex = null;
		if((iInd < iLength)&&(jInd<jLength[iInd])){
			while((vertex != null)&&(vertex.frame >= 0)&&(vertex.frame != frame)){
				lastVertex = vertex;
				vertex = vertex.next;
			}
			if(vertex == null){
				vertex = new AttkVtx(lastVertex);
				vertex.frame = frame;
				lastVertex.next = vertex;
			}
			vertex.dmg = damage;
		}
	}
	public void SetRadial(int  iInd){

		isRadial = true;

	}
	public void SetCentre(float x, float y, float frame, int iInd){
		AttkVtx c = centre;
		AttkVtx lC = null;
		if(c==null){
			c.pos = new SPoint(x, y);
			c.frame = frame;
		}else{
			while((c != null)&&(c.frame >= 0)&&(c.frame != frame)){
				lC = c;
				c = c.next;
			}
			if(c == null){
				c = new AttkVtx(lC);
				c.frame = frame;
				lC.next = c;
			}
			c.pos = new SPoint(x, y);
			c.frame = frame;
		}
	}
	
	public void  SetVertex(float px, float py, float frame, int iInd, int jInd){
		//sets a vertex with a given frame.
		//ALL verts are meant to be input in chronological order (by frames)
		//this function sets the vert values of the vert at the specified frame,
		// or makes a new frame if none exists.
		AttkVtx vertex = aVtx[iInd][jInd];
		AttkVtx lastVertex = null;
		if(frame<0){//vertex SPointer
			vertex.pos = new SPoint(px, py);
			vertex.frame = frame;
		}else{
			if((iInd < iLength)&&(jInd<jLength[iInd])){
				while((vertex != null)&&(vertex.frame >= 0)&&(vertex.frame != frame)){
					lastVertex = vertex;
					vertex = vertex.next;
				}
				if(vertex == null){
					vertex = new AttkVtx(lastVertex);
					vertex.frame = frame;
					lastVertex.next = vertex;
				}
				vertex.pos = new SPoint(px, py);
				vertex.frame = frame;
			}
		}
	}
	
	public AttkVtx  GetVertSPointer(int iInd, int jInd){
		AttkVtx rtn = null;
		if(iInd<iLength)
			if(jInd<jLength[iInd])
				rtn =  aVtx[iInd][jInd];
		return rtn;
	}
	public AttkVtx  GetCtrSPointer(int iInd){
		AttkVtx rtn = null;
		if(iInd<iLength)
			rtn =  centre;
		return rtn;
	}
	
	public void  SetVertex(AttkVtx a, int iInd, int jInd){
		aVtx[iInd][jInd].pos.x = a.pos.x;
		aVtx[iInd][jInd].pos.y = a.pos.y;
		aVtx[iInd][jInd].mag = a.mag;
		aVtx[iInd][jInd].dmg = a.dmg;
		aVtx[iInd][jInd].dir = a.dir;
		aVtx[iInd][jInd].wgt = a.wgt;
	}
	SPoint[] MakeCircle(SPoint loc, float radius, int nVert){
		SPoint[] rtnArr;
		rtnArr = new SPoint[nVert];

		float accuracy = nVert/2;
		int point_index=0;


		

			for (float theta = 0; theta <= 2 * Mathf.PI; theta += Mathf.PI / accuracy) {
			rtnArr[point_index]=new SPoint();
			rtnArr[point_index].x = loc.x + radius * Mathf.Cos(theta);
			rtnArr[point_index].y = loc.y + radius*Mathf.Sin(theta);
				 
				point_index++;
			}
			
			return rtnArr;
	}
	public SPoint[] FetchRadialRenderingFrame(SPoint[] cenArr, float[] radArr){
		SPoint[] rtnArr;
		SPoint[] hArr;
		rtnArr = new SPoint[cenArr.Length*CIRC_VNUM];
		hArr = new SPoint[cenArr.Length];
		int mainInd = 0;
		for(int i=0;i<cenArr.Length; i++)
		{
			hArr=MakeCircle (cenArr[i], radArr[i],CIRC_VNUM);
			for(int j=0; j< CIRC_VNUM;j++){
				rtnArr[mainInd]=hArr[j];
				mainInd++;
			}
		}
		return rtnArr;
	}
	public SPoint[] FetchRenderingFrame(float frameInd, float aniLen, int tInd, SPoint loc, bool isRight){
	
		SPoint[] rtnArr;
		float[] radArr;
		float lastFrame = 0;
		SPoint lastPt = new SPoint ();
		float lastRad = 0;
		int trkPriority = 9999;
		float aWgt;
		int lastBk=0;
		int pInd=0;//to keep track of polygon/jIndex
		float frameTime = 1/60.0f;
		frameTime =  frameTime/aniLen;
		frameTime=frameTime=frameTime*6;
		AttkVtx vertex;
		int numVerts = jLength[tInd];
		rtnArr = new SPoint[numVerts];
		radArr = new float[numVerts];
		for (int i=0; i<numVerts; i++) 
				rtnArr [i] = new SPoint ();
		float diffHldr=0;
		float firstFrame=9;
		if(aVtx[tInd][0].frame > frameInd)//before the first frame
			return null;
		
		for(int j = 0; j < numVerts; j++){
			vertex =  aVtx[tInd][j];
			if(vertex.frame <0){ //vertex SPointer
				vertex =  aVtx[tInd][(int)vertex.pos.x];									
			}
			firstFrame = vertex.frame;
			while((vertex.next != null)&&(frameInd >= vertex.frame)){
				lastFrame = vertex.frame;
				lastPt.x = vertex.pos.x;
				lastPt.y = vertex.pos.y;
				if(isRadial)
					lastRad=vertex.rad;
				vertex = vertex.next;
			}
			diffHldr = frameInd-frameTime;
			if((vertex.frame<frameInd)&&(firstFrame>frameInd-frameTime)){
				//entire attack passed between last frame and this
				//retrieve first vals so it is not skipped
				return null;
				vertex.CopyFrom(aVtx[tInd][j]);
				lastPt.x=vertex.pos.x;
				lastPt.y=vertex.pos.y;
				if(isRadial)
					lastRad=vertex.rad;
				if(vertex.frame <0){ //vertex SPointer
					vertex = aVtx[tInd][(int)vertex.pos.x];
					lastPt.x=vertex.pos.x;
					lastPt.y=vertex.pos.y;
					if(isRadial)
						lastRad=vertex.rad;
				}
				frameInd=vertex.frame;//set to start
			}
			else if(vertex.frame<frameInd)
				return null;
			//past end of frame
			if(Mathf.Abs(lastFrame - vertex.frame) == 0)
				aWgt = 0;
			else
				aWgt = (frameInd - lastFrame)/Mathf.Abs(lastFrame - vertex.frame); //alpha weight for interpolation
			if(isRight){
				rtnArr[j].x = (1-aWgt)*lastPt.x + (aWgt)*vertex.pos.x + loc.x;
				rtnArr[j].y = (1-aWgt)*lastPt.y + (aWgt)*vertex.pos.y + loc.y;
				if(isRadial)
					radArr[j]=(1-aWgt)*lastRad + (aWgt)*vertex.rad;
			}else{
				rtnArr[numVerts-j-1].x = -(1-aWgt)*lastPt.x - (aWgt)*vertex.pos.x + loc.x;
				rtnArr[numVerts-j-1].y = (1-aWgt)*lastPt.y + (aWgt)*vertex.pos.y + loc.y;
				if(isRadial)
					radArr[numVerts-j-1]=(1-aWgt)*lastRad + (aWgt)*vertex.rad;
			}			
		}
		if(isRadial)
			return FetchRadialRenderingFrame(rtnArr, radArr);
		else
		return rtnArr;
	}	
	public void FetchSPArr(SPoint[][] rtnArr){
		//Specifically only for use with checkAxis()		
		for(int i=0;i<iLength;i++)
			for(int j=0;j<jLength[i];j++)
				rtnArr[i][j]=new SPoint(aVtx[i][j].pos.x, aVtx[i][j].pos.y);
		
	}
	public void SetPriority(float p, float frm){
		for(AttkVtx c = centre; c!=null;c=c.next)
		if (c.frame == frm){
				c.pri = p;
				return;
		}
	}
	
};