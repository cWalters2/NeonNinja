using UnityEngine.UI;
using UnityEngine;
using System.Collections;



public class Stage : MonoBehaviour {
	
	protected int numObj;
	public int numPlr;
	protected SPoint lBound=new SPoint();
	protected SPoint uBound=new SPoint();
	//public SPoint[] startPos = new SPoint[2];
	protected TerrainBox tBox;
	const float EPS = 0.00001f;
	const int STAGE_LIMIT = 1000;
	public Ninja[] player = new Ninja[2];
	public bool debugModeOn;
	public GameObject stageHitbox;
    public TextAsset Terrain;

    public Material stageDebugMat;
	public bool matchModeOn=false;
	bool gameWon=false;
	// Use this for initialization
	protected void Start () {
		//for (int i=0; i<numPlr; i++)
		//	if (player [i] == null)
		//		SetPlStart (player [i]);
        //startPos[0] = new SPoint(0 , 0 );
        //startPos[1] = new SPoint(0 , 0 );
        LoadStage();
        numPlr = 2;
        
        //matchModeOn = GameObject.FindGameObjectWithTag("MatchHelper").GetComponent<MatchHelper>().isMatch;
    }
	
	// Update is called once per frame
	protected void Update () {


		string pName;
		//SPoint vel = player[0].GetVel();
		for (int i=0; i<numPlr; i++) {
			if(player[i]==null){
				GameObject go = new GameObject ();
				pName= "Player" + (i+1);
				go = GameObject.FindGameObjectWithTag (pName);
				player[i]=go.GetComponent<Ninja>();

			}
		}
		if(Time.deltaTime!=0){
		for(int i=0;i<numPlr;i++){
			ColDetect(player[i]);
			player[i].PostUpdate();
			
		}
			if(matchModeOn)
			WinCheck ();
			if(debugModeOn)
				RenderHitbox ();
		}
	}
	void WinCheck(){
		int winner = -1;//-1 for sentinel val
		for(int i=0;i<numPlr;i++)
			if(player[i].stats.stocks>0)
				if(winner==-1)
					winner=i;
		else//more than one player remaining
			winner=-2;//value for no winner yet
		
		if(winner>=0){//winner found and not a debug match
			WinScreen(player[winner]);
			//Render();
			return;}//stop here
	}	
	
	void WinScreen(Ninja wnr){
		if(matchModeOn){//not demo
			if(gameWon==false){

				GameObject wp = GameObject.FindGameObjectWithTag("MenuCursor").GetComponent<PauseMenuDemo>().winPanel;
				wp.SetActive(true);
				
				 
				gameWon=true;}
			for(int i=0;i<numPlr;i++){
				player[i].stats.flags.mBusy=true;
				player[i].stats.flags.aBusy=true;
				if(player[i].gCont.Tapped(GameController.START)){
					foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
						Destroy(o);}
					Application.LoadLevel ("menu");

				//	player[i].gCont.Pressed(GameController.START)=false;
				}
			}
		}
	}

    public bool LoadStage()
    {

        tBox = new TerrainBox();
        tBox.LoadCollisionBoxes(Terrain);
        uBound.x = 350;
        uBound.y = 350;
        lBound.x = -350;
        lBound.y = -350;
        return true;
    }
    public bool ProjectileDetect(Projectile p){
		SPoint pCtr, prV;
		bool pHitFlag=false;
		for (int i = 0; i < tBox.iLength; i++) {
			float[] prAngs = new float[tBox.GetJlength (i)];
			pCtr = new SPoint (p.pos.x, p.pos.y);
			int pJl = tBox.GetJlength (i) - 1;
			float lastAng = Mathf.Atan2 ((tBox.pBounds [i] [pJl].y - pCtr.y), (tBox.pBounds [i] [pJl].x - pCtr.x));
			pHitFlag = true;
            for (int j = 0; j < tBox.GetJlength(i); j++){
                if (!p.CheckAxis(tBox.GetSPoint(i, j), tBox.GetAng(i, j), p.pos, tBox.pBounds[i], tBox.jLength[i]))
                { //no axis intersection
                    pHitFlag = false;
                    j = tBox.GetJlength(i);//exit loop early
                }
            }
                /*for (int j = 0; j < tBox.GetJlength(i); j++) {
                    prV = new SPoint (tBox.pBounds [i] [j].x - pCtr.x, tBox.pBounds [i] [j].y - pCtr.y);
                    prAngs [j] = Mathf.Atan2 (prV.y, prV.x);
                    if ((lastAng > prAngs [j]) && (lastAng -prAngs [j]  < Mathf.PI )){
                        pHitFlag = false;

                    }
                    if ((lastAng < prAngs [j]) && (lastAng -prAngs [j]  < - Mathf.PI )){
                        pHitFlag = false;

                    }

                    lastAng = prAngs [j];
                }*/
                if (pHitFlag){
				
                return true;
            }
			
		}
		return pHitFlag;
	}
	protected bool ColDetect(Ninja plr){

		if (gameWon) {
	
		if(Input.anyKey){
				Application.LoadLevel ("menu");
				foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
					Destroy(o);}
				Application.LoadLevel ("menu");
            }
		return false;
		}
		

		bool hitflag=false;   
		int polyIndex = -1;//to keep record of which polygon caused the collision
		int hitIndex = -1;//to keep record of which side of the polygon caused collision
		SPoint[] plBox, plLast;
		bool retFlag=false;//holds the return value
		//this loads plBox[] and plLast[] with the player's hitboc this
		//and last frame respectively
		int hbLen=4;
		SPoint posDiff = new SPoint(plr.GetPos().x-plr.GetLastPos().x, plr.GetPos().y-plr.GetLastPos().y);
		if(posDiff.SqDistFromOrigin()!=0){
			plBox = plr.GetCurrentColBox();
			plLast=plr.GetCurrentColBox();
            hbLen = 6;
			for(int i=0;i<hbLen;i++){
				plLast[i].x-=posDiff.x;
				plLast[i].y-=posDiff.y;
			}
		}else{
			plBox = plr.GetHitBox();
			plLast=plr.GetHitBox();
			hbLen=4;
		}
		if((plr.GetPos().x>9)&&(plr.GetPos().x-posDiff.x<9))
            hbLen= hbLen;
		
		SPoint hld;
		float[] plAng = new float[hbLen];
		for(int i=0;i<hbLen-1;i++){
			plAng[i]= new SPoint(plBox[i].x-plBox[i+1].x,plBox[i].y-plBox[i+1].y).GetDir();
		}
		plAng[hbLen-1] = new SPoint(plBox[hbLen-1].x-plBox[0].x,plBox[hbLen-1].y-plBox[0].y).GetDir();
		// FillCollisionBox(plBox, plr->GetPos(), plr->stats.size.y, plr->stats.size.x);
		// FillCollisionBox(plLast, plr->stats.motion.lastPos, plr->stats.size.y, plr->stats.size.x);
		//set up angles here for easy acces along with the hitboxes above.
		
		for(int i = 0; i < tBox.iLength; i++){
			hitflag = true;//assume true, then prove false.
			for(int j = 0; j < tBox.GetJlength(i); j++)
			if(!plr.CheckAxis(tBox.GetSPoint(i, j), tBox.GetAng(i, j), plBox,tBox.pBounds[i], hbLen, tBox.jLength[i])){ //no axis intersection
				hitflag = false;
				j=tBox.GetJlength(i);//exit loop early
			}
			if(hitflag)//possible collision; check against other polygon axise
			for(int j = 0; j < hbLen; j++){
				if(!plr.CheckAxis(plBox[j], plAng[j], plBox, tBox.pBounds[i], hbLen, tBox.jLength[i])){ //no axis intersection
					hitflag = false;
					j=hbLen;//exit loop early
				}
			}   
			if(hitflag){//all checks are done, collision is sure
				polyIndex = i;//keep record of the last polygon caused the flag
				i=tBox.iLength;//Collision detected, end loop early
			}

		}

		SPoint prV=new SPoint();
		SPoint pCtr;

				//extra added to detect and destroy any projectiles on-screen
		int cv;
		bool pHitFlag = true;
		
		float flAng = 1.55f;
        if(polyIndex!=-1){
			//hitflag = true;//force to true to return
			retFlag=true;
			//see which side has collided
			for(int i = 0; i < tBox.GetJlength(polyIndex); i++){
				if(!plr.CheckAxis(tBox.GetSPoint(polyIndex, i), tBox.GetAng(polyIndex, i), plLast, tBox.pBounds[polyIndex], hbLen, tBox.jLength[polyIndex])){
					hitIndex = i;
					i=tBox.GetJlength(polyIndex);//end loop prematurely
				}
			}
			bool pAxisFlag=false;
			if(hitIndex == -1){
				for(int i  = 0; i < hbLen; i++){//check player angs to find the axis
					if(!plr.CheckAxis(plLast[i], plAng[i], plLast, tBox.pBounds[polyIndex], hbLen, tBox.jLength[polyIndex]) ){
						hitIndex = i;
						pAxisFlag = true; 
					}
				}//do collision detection again for this particular index on the polygon
			}//to get the exit vector

			SPoint axP = new SPoint();        
			if(hitIndex>=0){
               
                
                if (!pAxisFlag){//using poly axis
					axP = ExitDist(polyIndex, tBox.GetSPoint(polyIndex, hitIndex), tBox.GetAng(polyIndex, hitIndex), plr, 0);
					flAng=tBox.GetAng(polyIndex, hitIndex);
                                       }
				else{//using player axis
					axP = ExitDist(polyIndex, plBox[hitIndex], plAng[hitIndex], plr, 0);

					flAng=plAng[hitIndex];
				}

                if ((axP.x == 0) && (axP.y == 0))
                {
                polyIndex = polyIndex;// V-jank fix, add a check to get this answer in the first place
                    axP = ExitDist(polyIndex, tBox.GetSPoint(polyIndex, hitIndex+2), tBox.GetAng(polyIndex, hitIndex+2), plr, 0);
                }
			}else
				retFlag=false;
            if (polyIndex == 2)
                polyIndex = 2;
            if (polyIndex == 1)
                polyIndex = 1;
            plr.stats.motion.pos.x+=axP.x;
			plr.stats.motion.pos.y+=axP.y;
			//for walking    
			float axAng = Mathf.Atan2(axP.y, axP.x);
			float wallAng = Mathf.Atan2(plr.GetVel().y, plr.GetVel().x); 
			float wallDist = Mathf.Sqrt(Mathf.Pow(plr.GetVel().x, 2) + Mathf.Pow(plr.GetVel().y, 2) );
			wallAng -= axAng;
			
			//separate cases for wall collision
			//depend on whether player is tumbling
			
			//non-reounding
				
				if((axAng>0)&&(axAng < Mathf.PI))
					plr.Land(flAng);        
				plr.stats.motion.vel.x -= Mathf.Cos(wallAng)*wallDist*Mathf.Cos(axAng);
				plr.stats.motion.vel.y -= Mathf.Cos(wallAng)*wallDist*Mathf.Sin(axAng);
            if (( Mathf.Abs(  Mathf.Abs(axAng) - Mathf.PI )< 0.1f) && (plr.fHelper.airborne))
                plr.WallHang(axAng);
            else if  ((Mathf.Abs(axAng) < 0.1f) && (plr.fHelper.airborne))
                    plr.WallHang(axAng);


        }
       
        if ((!plr.fHelper.airborne)&&(!retFlag)&&(!plr.tPortFlag))            
			if(!GroundTrack(plr))//ground tracking done here
				plr.Fall();
		
		
		if((plr.GetPos().x > uBound.x)||(plr.GetPos().y > uBound.y)||(plr.GetPos().x < lBound.x)||(plr.GetPos().y < lBound.y)){
			if((plr.stats.stocks>0)||(!matchModeOn)){
			//SetPlStart(plr);
			plr.stats.stocks--;
			plr.stats.motion.vel = new SPoint(0,0);
			plr.stats.damage=0;}
		}
		if (retFlag)
			ColDetect (plr);
		return retFlag;
	}


    public SPoint ExitDist(int iInd, SPoint origin, float dir, Ninja plr, int ifGrab)
    {
        //plBox must contain 4 vertices!!!
        bool isHit = true;
        float rAng, distSq, projVal, moveDist;
        SPoint moveVec = new SPoint();
        moveVec.x = plr.GetPos().x - plr.GetLastPos().x;
        moveVec.y = plr.GetPos().y - plr.GetLastPos().y;
        SPoint[] plBox;
        int hbLen = 4;
       
            if (ifGrab < 0)
            {//implies groundCheck
                plBox = plr.GetHitBox();
                hbLen = 4;
            }
            else
            {
                plBox = plr.GetCurrentColBox();
                hbLen = 6;
            }
      
        
        float dHitMinSq = 1000;
        float dHitMaxSq = 0;
        float dPrMinSq = 1000;
        float dPrMaxSq = 0;
        SPoint projVec = new SPoint();
        float projMin = STAGE_LIMIT * STAGE_LIMIT;
        float projMax = -STAGE_LIMIT * STAGE_LIMIT;
        float hitMin = STAGE_LIMIT * STAGE_LIMIT;
        float hitMax = -STAGE_LIMIT * STAGE_LIMIT;
        for (int i = 0; i < tBox.GetJlength(iInd); i++)
        {
            projVec.x = tBox.GetSPoint(iInd, i).x - origin.x;
            projVec.y = tBox.GetSPoint(iInd, i).y - origin.y;
            distSq = projVec.x * projVec.x + projVec.y * projVec.y;
            if (distSq < EPS * EPS)
                rAng = dir; //Do NOT take atan2(0, 0)
            else
                rAng = dir - Mathf.Atan2(projVec.y, projVec.x);
            projVal = Mathf.Sin(rAng) * distSq;
            if (projVal < projMin){
                projMin = projVal;
                dPrMinSq = distSq;
            }if (projVal > projMax){
                projMax = projVal;
                dPrMaxSq = distSq;
            }
        }
        for (int i = 0; i < hbLen; i++)
        {
            projVec.x = plBox[i].x - origin.x;
            projVec.y = plBox[i].y - origin.y;
            distSq = projVec.x * projVec.x + projVec.y * projVec.y;
            if (distSq < EPS * EPS)
                rAng = dir; //Do NOT take atan2(0, 0)
            else
                rAng = dir - Mathf.Atan2(projVec.y, projVec.x);
            projVal = Mathf.Sin(rAng) * distSq;
            if (projVal < hitMin){
                hitMin = projVal;
                dHitMinSq = distSq;
            } if (projVal > hitMax) {
                hitMax = projVal;
                dHitMaxSq = distSq;
            }
        }
        if (dPrMinSq != 0)
            projMin = projMin / Mathf.Sqrt(dPrMinSq);
        if (dPrMaxSq != 0)
            projMax = projMax / Mathf.Sqrt(dPrMaxSq);
        if (dHitMinSq != 0)
            hitMin = hitMin / Mathf.Sqrt(dHitMinSq);
        if (dHitMaxSq != 0)
            hitMax = hitMax / Mathf.Sqrt(dHitMaxSq);
        if ((hitMax <= projMin) || (projMax <= hitMin))
            isHit = false;
        if ((Mathf.Abs(projMax - hitMin) < EPS) || (Mathf.Abs(hitMax - projMin) < EPS))
            isHit = false;
        //determine exit vector here
        float moveAng = Mathf.Atan2(moveVec.y, moveVec.x);
        float angDiff = Mathf.Abs(moveAng - (dir - Mathf.PI / 2));
        if (angDiff > Mathf.PI)
            angDiff = Mathf.Abs(angDiff - Mathf.PI * 2);
        moveDist = 0;
        if (isHit)
        {
            if (angDiff < Mathf.PI / 2) //moveVector in direction of exit vector
                moveDist = -Mathf.Abs(hitMax - projMin);
            else
                moveDist = Mathf.Abs(projMax - hitMin);
        }
        moveVec.x = moveDist * Mathf.Cos(dir - Mathf.PI / 2);
        moveVec.y = moveDist * Mathf.Sin(dir - Mathf.PI / 2);
      

        return moveVec;
    }
    public SPoint ExitDistN(int iInd, SPoint origin, float dir, Ninja plr, int ifGrab){
        // helper function for the Separating Axis Theorem that takes an axis defined by origin and dir 
        //re-conditioned to handle the parameters of the attack box 
        bool isHit = true;
        float projMin, projMax, hitMin, hitMax, distSq, projVal, cVal;
        float dHitMinSq = 1000;
        float dHitMaxSq = 0;
        float dPrMinSq = 1000;
        float dPrMaxSq = 0;
        SPoint projVec, nVec, rVec, cVec;
        projVec = new SPoint();
        SPoint moveVec = new SPoint();
        projMin = STAGE_LIMIT;
        projMax = -STAGE_LIMIT;
        hitMin = STAGE_LIMIT;
        hitMax = -STAGE_LIMIT;
        moveVec.x = plr.GetPos().x - plr.GetLastPos().x;
        moveVec.y = plr.GetPos().y - plr.GetLastPos().y;
        SPoint[] plBox;
        int hbLen = 4;
        if (ifGrab < 0)
        {//implies groundCheck
            plBox = plr.GetHitBox();
            hbLen = 4;
        }
        else
        {
            plBox = plr.GetCurrentColBox();
            hbLen = 6;
        }

        for (int i = 0; i < tBox.GetJlength(iInd); i++)
        {
            projVec.x = tBox.GetSPoint(iInd, i).x - origin.x;
            projVec.y = tBox.GetSPoint(iInd, i).y - origin.y;
        

            distSq = projVec.x * projVec.x + projVec.y * projVec.y;
            nVec = projVec.GetNormal();


            cVec = new SPoint(Mathf.Cos(dir + Mathf.PI / 2), Mathf.Sin(dir + Mathf.PI / 2));

            cVal = -cVec.Dot(nVec);
            //projVal = sin(rAng)*abs(sin(rAng))*distSq;
            projVal = cVal * Mathf.Abs(cVal) * distSq;

            if (projVal < projMin){
                projMin = projVal;
                dPrMinSq = distSq;
            }
            if (projVal > projMax) {
                projMax = projVal;
                dPrMaxSq = distSq;
            }
        }
        if (dPrMinSq != 0)
            projMin = projMin / Mathf.Sqrt(dPrMinSq);
        if (dPrMaxSq != 0)
            projMax = projMax / Mathf.Sqrt(dPrMaxSq);
        if (dHitMinSq != 0)
            hitMin = hitMin / Mathf.Sqrt(dHitMinSq);
        if (dHitMaxSq != 0)
            hitMax = hitMax / Mathf.Sqrt(dHitMaxSq);
        for (int i = 0; i < hbLen; i++)
        {
            projVec.x = plBox[i].x - origin.x;
            projVec.y = plBox[i].y - origin.y;
            distSq = projVec.x * projVec.x + projVec.y * projVec.y;
            cVec = new SPoint(Mathf.Cos(dir + Mathf.PI / 2), Mathf.Sin(dir + Mathf.PI / 2));
            nVec = projVec.GetNormal();
            cVal = -cVec.Dot(nVec);
            //projVal = sin(rAng)*abs(sin(rAng))*distSq;
            projVal = cVal * Mathf.Abs(cVal) * distSq;
            if (projVal < hitMin){
                hitMin = projVal;
                dHitMinSq = distSq;
            }if (projVal > hitMax){
                hitMax = projVal;
                dHitMaxSq = distSq;
            }
        }
        if ((hitMax <= projMin) || (projMax <= hitMin))
            isHit = false;
        if ((Mathf.Abs(projMax - hitMin) < EPS) || (Mathf.Abs(hitMax - projMin) < EPS))
            isHit = false;

        //determine exit vector here
        float moveAng = Mathf.Atan2(moveVec.y, moveVec.x);
		float angDiff = Mathf.Abs(moveAng - (dir - Mathf.PI/2)) ;
		if(angDiff > Mathf.PI)
			angDiff = Mathf.Abs(angDiff - Mathf.PI*2);
		float moveDist=0;
		if(isHit){
			if(angDiff < Mathf.PI/2) //moveVector in direction of exit vector
				moveDist = -Mathf.Abs(hitMax - projMin)/2;
			else
				moveDist = Mathf.Abs(projMax - hitMin)/2;
		}
		moveVec.x = moveDist*Mathf.Cos(dir - Mathf.PI/2);
		moveVec.y = moveDist*Mathf.Sin(dir - Mathf.PI/2);
		if(!isHit){
			moveVec.x = 0;
			moveVec.y = 0;
		}

		return moveVec;
	}

	public bool GroundTrack(Ninja plr){
		//specialized collision detection function
		//ensures the Ninja keeps flush with the floor	
		bool hitflag = true;
		bool indflag = true;
		bool pAxisFlag = false;
		float hitInd = -1;
		int polyIndex = -1;
		SPoint[] plBox = new SPoint[4];
		SPoint[] plLast = new SPoint[4];
		for(int i=0;i<4;i++){
			plBox[i]=new SPoint();
			plLast[i]=new SPoint();
	}
		float[] plAng = {0, Mathf.PI/2, Mathf.PI, -Mathf.PI/2};
		int hbLen=4;

				//modified from colision detect to move the player down
	
		FillCollisionBox(plBox, new SPoint(plr.GetPos().x, plr.GetPos().y-plr.stats.size.y/2), plr.stats.size.y, plr.stats.size.x);
		FillCollisionBox(plLast, new SPoint(plr.GetPos().x, plr.GetPos().y+plr.stats.size.y/2), plr.stats.size.y, plr.stats.size.x);
		//holds default angles
		
		for(int i = 0; i < tBox.iLength; i++){
			hitflag = true;
			for(int j = 0; j < tBox.GetJlength(i); j++)
				if(!plr.CheckAxis(tBox.GetSPoint(i, j), tBox.GetAng(i, j), plBox,tBox.pBounds[i], hbLen, tBox.jLength[i]) ) //no axis intersection
					hitflag = false;
			if(hitflag)
				for(int j = 0; j < 4; j++)
					if(!plr.CheckAxis(plBox[j], plAng[j], plBox,tBox.pBounds[i], hbLen, tBox.jLength[i]) ) //no axis intersection
						hitflag = false;
			if(hitflag)
				polyIndex = i;
		}
		int hitIndex = -1;
		if(polyIndex >= 0){
			hitflag = true;
			float moveDist = 0;
			for(int i = 0; i < tBox.GetJlength(polyIndex); i++){
				if(!plr.CheckAxis(tBox.GetSPoint(polyIndex, i), tBox.GetAng(polyIndex, i), plLast,tBox.pBounds[polyIndex], hbLen, tBox.jLength[polyIndex])){
					hitIndex = i;
				}
			}
			if(hitIndex == -1){
				//check player angs to find the axis
				for(int i  = 0; i < 4; i++){
					if(!plr.CheckAxis(plLast[i], plAng[i], plLast, tBox.pBounds[polyIndex], hbLen, tBox.jLength[polyIndex]) ){
						hitIndex = i;
						pAxisFlag = true;
					}
				}
			}		
			
			bool floorHit = false;
			SPoint floorVec=new SPoint(0,0);
			plr.stats.motion.pos.y-=plr.stats.size.y/2;
			if(!pAxisFlag)
				floorVec = ExitDist(polyIndex, tBox.GetSPoint(polyIndex, hitIndex), tBox.GetAng(polyIndex, hitIndex), plr, -1);
			else
				floorVec = ExitDist(polyIndex, plBox[hitIndex], plAng[hitIndex], plr, -1);
			float wallAng = Mathf.Atan2(floorVec.y, floorVec.x)-Mathf.PI/2;
			float wallDist=Mathf.Sqrt(floorVec.SqDistFromOrigin());
			if((floorVec.x==0)&&(floorVec.y==0))
				plr.stats.motion.pos.y+=plr.stats.size.y/2;
			else if(Mathf.Cos(wallAng)!=0){
				plr.stats.motion.pos.y+=(wallDist)/Mathf.Cos(wallAng);
				floorHit=true;
			}
			else{
				plr.stats.motion.pos.y+=plr.stats.size.y/2;
				floorHit=true;
			}
			if((floorHit)&&(!plr.fHelper.airborne)){
				plr.Land(tBox.GetAng(polyIndex, hitIndex));
				
				return true;
			}
		}
		return false;
	}
	
	void FillCollisionBox(SPoint[] plBox, SPoint pos, float h, float w){
		plBox[0].x = pos.x - w/2;
		plBox[0].y = pos.y;
		plBox[1].x = pos.x + w/2;
		plBox[1].y = pos.y;
		plBox[2].x = pos.x + w/2;
		plBox[2].y = pos.y + h;
		plBox[3].x = pos.x - w/2;
		plBox[3].y = pos.y + h;
	}
	bool  RenderHitbox( ){
		if (stageHitbox == null) {
			stageHitbox = new GameObject ();
			stageHitbox.name = "stage";
			MeshFilter mf = stageHitbox.AddComponent<MeshFilter> ();
			MeshRenderer mr = stageHitbox.AddComponent<MeshRenderer> ();
			tBox.Render ("CollisionBox");
			Material stMat = Resources.Load ("Material", typeof(Material)) as Material;
			mf.mesh = tBox.mesh;
			mr.material = stageDebugMat;
			return true;
	} else
			return false;
	}
}
