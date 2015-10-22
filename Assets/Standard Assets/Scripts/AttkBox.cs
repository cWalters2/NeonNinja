using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class AttkBox  {
	int numAtk;
	public AttkTrack[] trackList;
	const int NA = Ninja.NA;
	const int  SA = Ninja.SA;
	const int  DA = Ninja.DA;
	const int  UA = Ninja.UA;
	
	const int  NAIR = 4;
	const int  FAIR = 5;
	const int  BAIR = 6;
	const int  DAIR = 7;
	const int  UAIR = 8;
	const int  DATK = 9;
	
	
    
	const int  NUM_ATTACKS = 10;
	const int V_NUM=8;
	public const int NORMAL = 0;
	public const int SEQUENCE = 3;
	public const int FLASH = 4;
	public float grabRange = 12f;
	public bool flashAtkRdy=false;
	public Mesh mesh;
	protected Mesh[] lastMesh;
	public Mesh[] trackMesh;
	public Mesh[] subMesh;	
	SPoint[] circ;
	public AttkBox()
	{
		numAtk = NUM_ATTACKS;
		trackList = new AttkTrack[NUM_ATTACKS];
		for (int i=0; i<NUM_ATTACKS; i++)
			trackList [i] = new AttkTrack ();
		SetRadialTrace ();
	}
	void SetRadialTrace(){
		circ = new SPoint[V_NUM];
		 
		circ[0]=new SPoint(0,  1);
		circ[1]=new SPoint(-0.71f, .71f);
		circ[2]=new SPoint(-1, 0);
		circ[3]=new SPoint(-0.71f,-0.71f);
		circ[4]=new SPoint(0, -1);
		circ[5]=new SPoint(0.71f,-0.71f);
		circ[6]=new SPoint(1, 0);
		circ[7]=new SPoint(0.71f,0.71f);
	}
	public AttkBox(int atNum)
	{
		numAtk=atNum;
		trackList = new AttkTrack[atNum];
		for (int i=0; i<atNum; i++)
			trackList [i] = new AttkTrack ();
		SetRadialTrace ();

	}
	

	public bool  LoadFromScript( TextAsset colDetFilename, float scale, float hgt){
		int trackNum=0;
		int aInd = -1;
		int tInd = 0;
		int lastAInd = 0;
		string tLine="";
		string val;
		string one;
		string two;
		float xVal,yVal;
		float wgtD = 1;
		float magD = 1;
		float dirD = 0;
		float dmgD = 1;
		float curFrame = 0;
		float curPri = 0;
		int curInd = 0;
		int curBreak = 0;
		int enPos, endL;
		string line;
		try{
			//get the polygon count for the arrays
			string[] lines = colDetFilename.text.Split('\n');
			foreach (string rline in lines){
				line = rline.Trim();

					endL = line.Length-1;
					if(line.CompareTo("<NA>") == 0)
						aInd = NA;
					if(line.CompareTo("<SA>") == 0)
						aInd = SA;
					if(line.CompareTo("<DA>") == 0)
						aInd = DA;
					if(line.CompareTo("<UA>") == 0)
						aInd = UA;
					if(line.CompareTo("<NAIR>") == 0)
						aInd = NAIR;
					if(line.CompareTo("<FAIR>") == 0)
						aInd = FAIR;
					if(line.CompareTo("<BAIR>") == 0)
						aInd = BAIR;
					if(line.CompareTo("<DAIR>") == 0)
						aInd = DAIR;
					if(line.CompareTo("<UAIR>") == 0)
						aInd = UAIR;
					if(line.CompareTo("<DATK>") == 0)
						aInd = DATK;
					
					
					if(aInd!=lastAInd)
						trackNum=0;//reset
					
					lastAInd=aInd;
					if((line.Length > 10)&&(line.Substring(0, 10).CompareTo("<numtrack=") == 0 )){	//track
						enPos = line.IndexOf('=');
						val = line.Substring(enPos+1,line.Length -12); 
						trackList[aInd].AllocateFirstIndices(int.Parse(val));
					}
					if((line.Length > 8)&&(line.Substring(0, 8).CompareTo("<vertex=") == 0 )){	//vertex 
						val = line.Substring(8, line.Length-10);
						if(trackList[aInd].trackType==0)
							trackList[aInd].AllocateSecondIndices(0, int.Parse(val)); 
						else
							trackList[aInd].AllocateSecondIndices(tInd, int.Parse(val)); //allocate for a sub track
					}
					else if((line.Length > 6)&&(line.Substring(0, 6).CompareTo("<multi") == 0 ))
						trackList[aInd].trackType=1;
					else if((line.Length > 6)&&(line.Substring(0, 6).CompareTo("<paral") == 0 ))
						trackList[aInd].trackType=2;
					else if((line.Length > 6)&&(line.Substring(0, 6).CompareTo("<seque") == 0 ))
						trackList[aInd].trackType=3;
					else if((line.Length > 6)&&(line.Substring(0, 6).CompareTo("<flash") == 0 ))
						trackList[aInd].trackType=4;
					else if((line.Length > 6)&&(line.Substring(0, 6).CompareTo("<track") == 0 )){
						val = line.Substring(7, line.Length-8);
						tInd = int.Parse(val);
					}
					else if((line.Length == 8)&&(line.Substring(0, 8).CompareTo("</track>") == 0 ))
						tInd = 0;//will iterate only if there are new tracks
					else if((line.Length > 7)&&(line.Substring(0, 7).CompareTo("<centre") == 0 )){
						enPos = line.IndexOf(",");
						val = line.Substring(8, enPos - 8);
						xVal = float.Parse(val); //vtx number
						val = line.Substring(enPos+1, line.Length-enPos-2);
						//val = line.Substring(0, enPos);
						yVal = float.Parse(val);
						trackList[aInd].SetCentre(xVal*scale,(yVal-(hgt/2))*scale, curFrame, tInd);
					}else if((line.Length > 7)&&(line.Substring(0, 7).CompareTo("<radial") == 0 )){ 
						trackList[aInd].SetRadial(tInd);
					}else if((line.Length > 7)&&(line.Substring(0, 7).CompareTo("<vacuum") == 0 )){ 
						trackList[aInd].isVacuum=true;
					}
					else if((line.Length > 6)&&(line.Substring(0, 4).CompareTo("<vtx") == 0 )){
						enPos = line.IndexOf("=");
						one = line.Substring(enPos+1, endL-enPos-3);
						two = line.Substring(enPos+4, endL-enPos-5);
						if((line.Length > 9)&&(line.Substring(enPos+1, endL-enPos-3).CompareTo("vtx")==0)){
							val = line.Substring(4, enPos - 4);
							curInd = int.Parse(val);
							val = line.Substring(enPos+4, endL-enPos-5);
							trackList[aInd].SetVertex(int.Parse(val), 0, -2, tInd, curInd);
						}else{
							val = line.Substring(4, enPos - 4);
							curInd = int.Parse(val); //vtx number
							tLine = line.Substring(enPos+1, line.Length-enPos-1);
							enPos = tLine.IndexOf(",");
							val = tLine.Substring(0, enPos);
							xVal = float.Parse(val);
							val = tLine.Substring(enPos+1, tLine.Length-enPos-3);
							yVal = float.Parse(val);
							trackList[aInd].SetVertex(xVal*scale, (yVal)*scale, curFrame, tInd, curInd);
						}
					}
					else if((line.Length > 3)&&(line.Substring(0, 3).CompareTo("wgt") == 0 )){ 
						enPos = line.IndexOf(" ");
						val = line.Substring(3, enPos-3);
						curInd = int.Parse(val);
						val = line.Substring(enPos+1, line.Length-enPos-1);
						trackList[aInd].SetWeight(float.Parse(val), curFrame, tInd, curInd);
					}
					else if((line.Length > 3)&&(line.Substring(0, 3).CompareTo("mag") == 0 )){ 
						enPos = line.IndexOf(" ");
						val = line.Substring(3, enPos-3);
						curInd = int.Parse(val);
						val = line.Substring(enPos+1, line.Length-enPos-1);
						trackList[aInd].SetMagnitude(float.Parse(val), curFrame, tInd, curInd);
					}
					else if((line.Length > 3)&&(line.Substring(0, 3).CompareTo("dmg") == 0 )){ 
						enPos = line.IndexOf(" ");
						val = line.Substring(3, enPos-3);
						curInd = int.Parse(val);
						val = line.Substring(enPos+1, line.Length-enPos-1);
						trackList[aInd].SetDamage(float.Parse(val), curFrame, tInd, curInd);
					}
					else if((line.Length  > 3)&&(line.Substring(0, 3).CompareTo("dir") == 0 )){ 
						enPos = line.IndexOf(" ");
						val = line.Substring(3, enPos-3);
						curInd = int.Parse(val );
						val = line.Substring(enPos+1, line.Length -enPos-1);
						trackList[aInd].SetDirection(float.Parse(val), curFrame, tInd, curInd);		  
					}
					else if((line.Length  > 3)&&(line.Substring(0, 3).CompareTo("rad") == 0 )){ 
						enPos = line.IndexOf(" ");
						val = line.Substring(3, enPos-3);
						curInd = int.Parse(val );
						val = line.Substring(enPos+1, line.Length -enPos-1);
						trackList[aInd].SetRadius(float.Parse(val), curFrame, tInd, curInd);		  
					}
					else if((line.Length  > 7)&&(line.Substring(0, 7).CompareTo("<frame=") == 0 )){ 
						val = line.Substring(7, line.Length -8);
						curFrame = float.Parse(val);
					}
					else if((line.Length  > 10)&&(line.Substring(0, 10).CompareTo("<priority=") == 0 )){ 
						val = line.Substring(10, line.Length -11);
						curPri = float.Parse(val);
						trackList[aInd].SetPriority(curPri, curFrame);
                    }
					else if(line.CompareTo("</frame>") == 0 ){ 
						//end frame here
					}
					else if((line.Length  > 6)&&(line.Substring(0, 7).CompareTo("<break=") == 0 )){ 
						val = line.Substring(7, endL-7);
						//if((tInd==0)&&(aInd==NAIR))
						//	bool chk_pt = true;
						trackList[aInd].SetBreak(true, tInd, int.Parse(val));
						curBreak = int.Parse(val);
					}
				

			}
		}	catch(FileNotFoundException e){
			return false;
		}
		return true;
	}
	/*public bool _Render(GrEngine *eng, LPCSTR mat, SPoint p, int aInd, float frame, int plNum, bool isRight, double aniLen){
		if((aInd>=0)&&(frame>=0)){
			for(int t=0;t<trackList[aInd].iLength;t++){
				if(trackList[aInd].GetJlength(t)>0){
					SPoint* hitArr = trackList[aInd].FetchRenderingFrame(frame, aniLen, t, p, isRight);
					int brkNum = 0;
					int nxtBrk = trackList[aInd].FetchNextBreak(t, brkNum, isRight);
					int poly=0;
					if(hitArr!=NULL){
						while(brkNum<trackList[aInd].GetJlength(t)){				
							eng->DrawAttkBox(hitArr, mat, brkNum, nxtBrk, SPoint(0, 0), plNum, t, poly); //only draw this particular hitbox
							brkNum = nxtBrk+1;
							nxtBrk = trackList[aInd].FetchNextBreak(t, brkNum, isRight);
							poly++;
						}
					}
				}
			}
		}
		return S_OK;
	}*/
	
	public void  TranslateAttack(SPoint t, int aInd){
		trackList[aInd].TranslateTrack(t);
	}
	public void  FetchFlashFrame(int atkInd, float aniLen, float frameInd, int tInd, SPoint loc, bool isRight, AttkTrack aTr){
		}
public void  FetchFrame(int atkInd, float aniLen, float frameInd, int tInd, SPoint loc, bool isRight, AttkTrack aTr){
	AttkVtx vertex;

	vertex = trackList[atkInd].GetVertSPointer(tInd, 0);
	int atkLen;
	if((vertex==null)||(vertex.frame > frameInd)){// attack not yet active
		aTr.noHit=true;
		return;
	}
	if (trackList [atkInd].trackType == 4){// for flash attacks
				if (!flashAtkRdy)//leave if not ready
						return;
		else
				flashAtkRdy=false;
		}
			//FetchFlashFrame (atkInd,  aniLen,frameInd, tInd, loc, isRight, aTr);
		AttkVtx[] rtnArr;
		AttkVtx ctr=new AttkVtx();
		SPoint retCtr = new SPoint();
		if (trackList [atkInd].isRadial) {
			aTr.AllocateFirstIndices (trackList [atkInd].GetJlength (tInd));//prepare for number of circles
			aTr.AllocateSecondIndices(0, trackList[atkInd].FetchNextBreak(tInd,0, true)+1);//allocate for first poly
		}else{
		aTr.AllocateFirstIndices(trackList[atkInd].polyNum[tInd]);//prepare for number of sub-polygons
			aTr.AllocateSecondIndices(0, trackList[atkInd].FetchNextBreak(tInd,0, true)+1);//allocate for first poly
		}
		float lastFrame = 0;
		float firstFrame=0;
		SPoint lastPt = new SPoint();

		float lastDir=0, lastDmg=0, lastMag=0, lastWgt=0, lastCtrX=0, lastCtrY = 0, lastRad=0;
		float frameTime=1/60.0f;
		frameTime=frameTime/aniLen;
		float aWgt;
		float[] radArr;
		int lastBk=0;
		int pInd=0;//to keep track of tracks/jIndex	
		float diffHldr =0;
		if(atkInd < numAtk){
			int numVerts;
			if(trackList[atkInd].isRadial);
			numVerts= trackList[atkInd].jLength[tInd];
			rtnArr = new AttkVtx[numVerts];
			radArr = new float[numVerts];
			for(int j = 0; j < numVerts; j++)
				rtnArr[j]=new AttkVtx();
			for(int j = 0; j < numVerts; j++){
	
				vertex = trackList[atkInd].GetVertSPointer(tInd, j);
				if(vertex.frame <0) //vertex SPointer
					vertex = trackList[atkInd].GetVertSPointer(tInd, (int)vertex.pos.x);
				firstFrame = vertex.frame;
				ctr = trackList[atkInd].GetCtrSPointer(tInd);
				while((vertex.next != null)&&(frameInd >= vertex.frame)){
					lastFrame = vertex.frame;
					lastPt.x = vertex.pos.x;
					lastPt.y = vertex.pos.y;
					lastMag = vertex.mag;
					lastDir = vertex.dir;
					lastDmg = vertex.dmg;
					lastWgt = vertex.wgt;
					lastRad = vertex.rad;
					vertex = vertex.next;
					lastCtrX = ctr.pos.x;
					lastCtrY = ctr.pos.y;
					ctr=ctr.next;
				}	
				//end of vertlist implies the active window has ended. 
				//cancel the function here
				diffHldr = frameInd-frameTime;
				if((vertex.frame<frameInd)&&(firstFrame>frameInd-frameTime)){
					//entire attack passed between last frame and this
					//retrieve first vals so it is not skipped
					vertex = trackList[atkInd].GetVertSPointer(tInd, j);
					if(vertex.frame <0) //vertex SPointer
						vertex = trackList[atkInd].GetVertSPointer(tInd, (int)vertex.pos.x);
					frameInd=vertex.frame;//set to start
				}
				else if(vertex.frame<frameInd){
					aTr.noHit=true;
					return;
				}
				if(Mathf.Abs(lastFrame - vertex.frame) == 0)
					aWgt = 0;
				else
					aWgt = (frameInd - lastFrame)/Mathf.Abs(lastFrame - vertex.frame); //alpha weight for interpolation
				if(isRight){
					rtnArr[j].pos.x = (1-aWgt)*lastPt.x + (aWgt)*vertex.pos.x + loc.x;
					rtnArr[j].dir = (1-aWgt)*lastDir + (aWgt)*vertex.dir;
				}
				else{//INVERTED DIRS HERE
					rtnArr[j].pos.x = -(1-aWgt)*lastPt.x -(aWgt)*vertex.pos.x + loc.x;
					rtnArr[j].dir = (1-aWgt)*lastDir + (aWgt)*vertex.dir;
					float diff = ((Mathf.PI/2)-rtnArr[j].dir)*2.0f;
					rtnArr[j].dir+=diff;
				}				
				rtnArr[j].pos.y = (1-aWgt)*lastPt.y + (aWgt)*vertex.pos.y + loc.y;
				rtnArr[j].dmg = (1-aWgt)*lastDmg + (aWgt)*vertex.dmg;				
				rtnArr[j].mag = (1-aWgt)*lastMag + (aWgt)*vertex.mag;
				rtnArr[j].wgt = (1-aWgt)*lastWgt + (aWgt)*vertex.wgt;
				rtnArr[j].rad = (1-aWgt)*lastRad + (aWgt)*vertex.rad;
				radArr[j] =rtnArr[j].rad;
				retCtr.x = (1-aWgt)*lastCtrX + (aWgt)*ctr.pos.x;
				retCtr.y = (1-aWgt)*lastCtrY + (aWgt)*ctr.pos.y;
				aTr.SetCentre(retCtr.x, retCtr.y, 0, pInd);
				//this attack track is ONE track
				// with the polygons separated into new tracks.
				//always start each poly on new track
				SPoint[] spArr = new SPoint[numVerts];
				for(int k=0;k<numVerts;k++){
					spArr[k] = new SPoint(rtnArr[k].pos.x, rtnArr[k].pos.y);
				}
				SPoint[] circHolder = new SPoint[AttkTrack.CIRC_VNUM];
				if((trackList[atkInd].isRadial)&&(j==numVerts-1)){
					circHolder = trackList[atkInd].FetchRadialRenderingFrame(spArr, radArr);
					AttkVtx radVertHolder = new AttkVtx();
					for(int m = 0; m<numVerts;m++){
						for(int k=0; k<AttkTrack.CIRC_VNUM;k++)
						{
							radVertHolder = new AttkVtx(circHolder[k+m*AttkTrack.CIRC_VNUM], rtnArr[m].mag, rtnArr[m].dir,rtnArr[m].dmg, rtnArr[m].wgt);   
							aTr.SetVertex(radVertHolder, m, k);
                        }
					if(m<j){
							int brkCheck=trackList[atkInd].FetchNextBreak(tInd, m, true);
							aTr.AllocateSecondIndices(m+1, AttkTrack.CIRC_VNUM);
						}

				}
				}else if((trackList[atkInd].aVtx[tInd][j].brk)||(j==numVerts-1)){//end of this polygon
					//continue handling circles here
					if(isRight)
						for(int k = lastBk; k<=j; k++)//add rtnArr to aTr
							aTr.SetVertex(rtnArr[k], pInd, k-lastBk);
					else	
						for(int k = j; k>=lastBk; k--)//add rtnArr to aTr
							aTr.SetVertex(rtnArr[k], pInd, j-k);					
					pInd++;					
					if(j<numVerts-1){//do not preemtively allocate when done
						int brkCheck=trackList[atkInd].FetchNextBreak(tInd, j, true);
						aTr.AllocateSecondIndices(pInd, trackList[atkInd].FetchNextBreak(tInd, j, true)-j);
					}
					lastBk=j+1;
				}
			}
			
			//if(rtnArr!=null)
			//	rtnArr=null;
			if(trackList[atkInd].isRadial){
				for(int i=0;i<aTr.iLength;i++){
					for(int j = 0; j < AttkTrack.CIRC_VNUM-1; j++){
						aTr.atkAng[i][j] = Mathf.Atan2(aTr.aVtx[i][j+1].pos.y - aTr.aVtx[i][j].pos.y, aTr.aVtx[i][j+1].pos.x - aTr.aVtx[i][j].pos.x); //store angles for detection
						aTr.atkAng[i][AttkTrack.CIRC_VNUM-1] = Mathf.Atan2(aTr.aVtx[i][0].pos.y - aTr.aVtx[i][AttkTrack.CIRC_VNUM-1].pos.y, aTr.aVtx[i][0].pos.x - aTr.aVtx[i][AttkTrack.CIRC_VNUM-1].pos.x);  
					}
				}

			}else{
				for(int i=0;i<aTr.iLength;i++){
					atkLen=aTr.GetJlength(i);
					for(int j = 0; j < atkLen-1; j++)
						aTr.atkAng[i][j] = Mathf.Atan2(aTr.aVtx[i][j+1].pos.y - aTr.aVtx[i][j].pos.y, aTr.aVtx[i][j+1].pos.x - aTr.aVtx[i][j].pos.x); //store angles for detection
					aTr.atkAng[i][atkLen-1] = Mathf.Atan2(aTr.aVtx[i][0].pos.y - aTr.aVtx[i][atkLen-1].pos.y, aTr.aVtx[i][0].pos.x - aTr.aVtx[i][atkLen-1].pos.x);
				}
			}
		}
		AttkTrack rTr = FetchRadialFrame (aTr);
	}
	public AttkTrack FetchRadialFrame(AttkTrack aTr){
		return aTr;

		}
	public void  ResetHits(int aSel){
		trackList[aSel].ResetHits();

	}
	public bool HasHits(int aSel){
		bool rtn = false;
		for(int i=0;i<trackList[aSel].iLength;i++){
			if(trackList[aSel].HasHits(i))
				rtn=true;
		}
		return rtn;
	}
	public void  RenderSequence(string mat, SPoint p, int aInd, float frame, int plNum, bool isRight, float aniLen, int sInd){
		//function contains unwanted redundancy found in Render and should be fixed
		if((aInd>=0)&&(frame>=0)){
			CombineInstance[] tComb = new CombineInstance[trackList[aInd].iLength];
			trackMesh = new Mesh[trackList[aInd].iLength];
			int t =sInd-1;
				if(trackList[aInd].GetJlength(t)>0){
					SPoint[] hitArr = trackList[aInd].FetchRenderingFrame(frame, aniLen, t, new SPoint(0,0), isRight);
					int brkNum = 0;
					int nxtBrk = trackList[aInd].FetchNextBreak(t, brkNum, isRight);
					int poly=0;
					if(hitArr!=null){
						subMesh = new Mesh[trackList[aInd].GetJlength(t)];
						while(brkNum<trackList[aInd].GetJlength(t)){
							
							DrawAttkBox(hitArr, mat, brkNum, nxtBrk, p, plNum, t, poly); //only draw this particular hitbox
							brkNum = nxtBrk+1;
							nxtBrk = trackList[aInd].FetchNextBreak(t, brkNum, isRight);
							
							poly++;
						}
						CombineInstance[] combine = new CombineInstance[poly];
						for(int i=0;i<poly;i++){
							combine[i].mesh=subMesh[i];
						}
						trackMesh[t] = new Mesh ();
						trackMesh[t].CombineMeshes (combine, true, false);
						tComb[t].mesh=trackMesh[t];
					}
					
				}
			
			mesh = new Mesh();
			if(trackList[aInd].iLength==1){
				if(trackMesh[0]!=null)
					mesh=trackMesh[0];
			}else
				
				mesh.CombineMeshes(tComb, true, false);
		}
		
		
		
	}

	public SPoint[] FetchGrabRenderFrame(float frameInd,  SPoint loc, bool isRight){
		float h = 14;
		float b = 6;
		float u = 0;
		float range = grabRange;
		if (isRight)
				u=range;	
		SPoint[] rtnArr = new SPoint[4];
		rtnArr[0] = new SPoint (loc.x+u, loc.y+h);
		rtnArr[1] = new SPoint (loc.x+u-range, loc.y+h);
		rtnArr[2] = new SPoint (loc.x+u-range, loc.y+b);
		rtnArr[3] = new SPoint (loc.x+u, loc.y+b);
		return rtnArr;
	}
public int Render(string mat, SPoint p, int aInd, float frame, int plNum, bool isRight, float aniLen ){
	bool radial = false;
	int	fadeoutInd = -1;
	bool[] endFrameFlag;
	if((aInd>=0)&&(frame>=0)){
			CombineInstance[] tComb = new CombineInstance[trackList[aInd].iLength];
			if(lastMesh==null)
				lastMesh = new Mesh[trackList[aInd].iLength];
			endFrameFlag = new bool[trackList[aInd].iLength];
			trackMesh = new Mesh[trackList[aInd].iLength];
			for(int t=0;t<trackList[aInd].iLength;t++){
				endFrameFlag[t]=false;
				if(trackList[aInd].GetJlength(t)>0){
					SPoint[] hitArr = trackList[aInd].FetchRenderingFrame(frame, aniLen, t, new SPoint(0,0), isRight);
					int brkNum = 0;
					int nxtBrk = trackList[aInd].FetchNextBreak(t, brkNum, isRight);
					int poly=0;
					int aLen = 0;

					if(hitArr!=null){
						subMesh = new Mesh[trackList[aInd].GetJlength(t)];
						aLen = trackList[aInd].GetJlength(t);
						if(trackList[aInd].isRadial)
							aLen = aLen*AttkTrack.CIRC_VNUM;
						while(brkNum<aLen){
		
							DrawAttkBox(hitArr, mat, brkNum, nxtBrk, p, plNum, t, poly); //only draw this particular hitbox

							brkNum = nxtBrk+1;
							nxtBrk = trackList[aInd].FetchNextBreak(t, brkNum, isRight);

							poly++;
						}
						CombineInstance[] combine = new CombineInstance[poly];
						for(int i=0;i<poly;i++){
							combine[i].mesh=subMesh[i];
						}
						trackMesh[t] = new Mesh ();
						trackMesh[t].CombineMeshes (combine, true, false);
						tComb[t].mesh=trackMesh[t];

					}else if(frame>trackList[aInd].aVtx[t][0].frame){//check for end of keyframe
						endFrameFlag[t]=true;
					}//

				}
			}
			//if(!endFrameFlag[0])
			if(lastMesh.Length!=trackMesh.Length)
				lastMesh = new Mesh[trackMesh.Length];
			for(int m=0;m<trackMesh.Length;m++)
				if(trackMesh[m]!=null)
					lastMesh[m]=trackMesh[m];

			mesh = new Mesh();
			if(trackList[aInd].iLength==1){
				if(trackMesh[0]!=null)
					mesh=trackMesh[0];
			}else{

				for(int z=0;z<trackList[aInd].iLength;z++)
					if(endFrameFlag[z]){
						tComb[z].mesh=lastMesh[z];
					fadeoutInd = z;
				}
				mesh.CombineMeshes(tComb, false, false);

			}

		}


		return fadeoutInd;
	}
	
	public void	DrawAttkBox(SPoint[] v, string mat, int n, int endN, SPoint p, int pNum,  int t, int poly){
		//specialized manual function for attackboxes
		subMesh [poly] = new Mesh ();
		int vTot = endN - n + 1;
		Vector3[] vertices = new Vector3[vTot];
		Color32[] col = new Color32[vTot];

		//std::stringstream hbName;
		string hbName = "P#" + pNum + "HB-" + t + "-" + poly;


		for (int i = n; i <= endN; i++) {
			vertices [i-n] = new Vector3 (v [i].x + p.x, v [i].y + p.y, 0);
			col [i-n] = new Color32 (0, 250, 20, 150);
		}
		int tTot = (vTot - 2) * 3;
		int[] tri = new int[tTot];
		for (int i = 0; i<(tTot/3);i++) {//triangle fan
			tri[i*3]=0;
			tri[(i*3)+1]=i+1;
			tri[(i*3)+2]=i+2;
		}
		subMesh[poly].vertices=vertices;
		subMesh[poly].triangles = tri;
		subMesh[poly].colors32 = col;
	}

}
