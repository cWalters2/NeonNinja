using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Stats {
	 
	//NOT SCRIPT DEFINABLE
	public struct Motion{
		public SPoint pos;
		public SPoint vel;
		public SPoint accl;//use accl, your values will be added on update()
		public SPoint move; //to made immediate, one-frame movements	
		public SPoint lastPos; //position at last frane
	};
	public struct Walk{
		public float loopStart;
		public float vel;
		public float maxSpeed;
		public STimer shuffle;
		//end of definables
		public SPoint slopeAng;//to track the angle of slopes
		public float gndSpeed;//ground speed
		public float dirInf;//not yet used; directional influence
	};
	
	public struct Pivot{
		public STimer pTmr;
		public float fastTime;
		public float slowTime;
		public bool slowPivot;//flag for cases where pivot runs slow		
	};
	
	public struct  Jump {
		public float  vel;
		public float  airJumpVel;
	
		public STimer tmr;

	};
	public struct PlayerID{
		public int num;
		
	};
	
	
	public struct Flags{//strictly to hold bool flags
		//for communication with the Graphics Engine
		public bool landed;
		public bool aBusy;
		public bool mBusy;
		public bool invuln;
		public bool running;
	};


	public PlayerID id;
	public Motion motion;
	public Walk walk;
	
	public Jump jump;
	
	public Pivot pivot;

	public Flags flags;
	public SPoint size;
	public float damage;
	public int stocks;
	public float grav;
	public float fric;
	public float defence;
	public float grabRange;

	public STimer dodge=new STimer();
	public STimer land=new STimer();
	public Stats(){
		motion.lastPos=new SPoint();
		motion.pos=new SPoint();
		motion.accl=new SPoint();
		motion.move=new SPoint();
		motion.vel=new SPoint();
		walk.shuffle=new STimer(0.5f);
		walk.slopeAng=new SPoint();
		
		pivot.pTmr = new STimer (0.125f);
		pivot.fastTime=0.125f;
		pivot.slowTime = 0.4f;
		jump.tmr = new STimer (0.1f);
		
		jump.tmr = new STimer (0.12f);
	

		grav = 0.04f;
		grabRange = 24f;
		size = new SPoint ();
		land=new STimer(1.0f);
		dodge = new STimer ();
		flags.mBusy = false;
		flags.aBusy = false;

	}
	public bool LoadStats(TextAsset file){
		//loads the player's stats from the designated script
		//if not called, defaults from pl[NAME] will be used
		//should these be undefined, defaults from c_PlayerStats will be used
		bool hr = true;
		string line = "";
		int splitInd = 0;
		string[] lines = file.text.Split('\n');
		try{

			foreach (string rline in lines){
				line = rline.Trim();
				//while ((line = infile.ReadLine()) != null) {

					 if((line.Length>15)&&(line.Substring(0,15).CompareTo("walk.loopstart=") == 0)){
						splitInd = line.IndexOf("=")+1;
						walk.loopStart=float.Parse(line.Substring(splitInd, line.Length-15) );
					}else if((line.Length>9)&&(line.Substring(0,9).CompareTo("walk.vel=") == 0)){
						splitInd = line.IndexOf("=")+1;
						walk.vel=float.Parse(line.Substring(splitInd, line.Length-9) );
					}else if((line.Length>14)&&(line.Substring(0,14).CompareTo("walk.maxspeed=") == 0)){
						splitInd = line.IndexOf("=")+1;
						walk.maxSpeed=float.Parse(line.Substring(splitInd, line.Length-14) );
					}else if((line.Length>15)&&(line.Substring(0,15).CompareTo("walk.shuffletime=") == 0)){ 
						splitInd = line.IndexOf("=")+1;
						walk.shuffle= new STimer(float.Parse(line.Substring(splitInd, line.Length-15) ) );//tmr
					}else if((line.Length>12)&&(line.Substring(0,12).CompareTo("pivot.time=") == 0)){
						splitInd = line.IndexOf("=")+1;
						pivot.pTmr=new STimer(float.Parse(line.Substring(splitInd, line.Length-12) ) ); //tmr
					}else if((line.Length>11)&&(line.Substring(0,11).CompareTo("jump.delay=") == 0)){
						splitInd = line.IndexOf("=")+1;
						jump.tmr=new STimer(float.Parse(line.Substring(splitInd, line.Length-11) ) );//tmr
					}else if((line.Length>9)&&(line.Substring(0,9).CompareTo("jump.vel=") == 0)){
						splitInd = line.IndexOf("=")+1;
						jump.vel=float.Parse(line.Substring(splitInd, line.Length-9) );
					}else if((line.Length>16)&&(line.Substring(0,16).CompareTo("jump.airjumpvel=") == 0)){
						splitInd = line.IndexOf("=")+1;
						jump.airJumpVel=float.Parse(line.Substring(splitInd, line.Length-16) );
					}else if((line.Length>10)&&(line.Substring(0,10).CompareTo("land.time=") == 0)){
						splitInd = line.IndexOf("=")+1;
						land=new STimer(float.Parse(line.Substring(splitInd, line.Length-10) ) );//tmr
					}else	if((line.Length>11)&&(line.Substring(0,11).CompareTo("dodge.time=") == 0)){
						splitInd = line.IndexOf("=")+1;
						dodge=new STimer(float.Parse(line.Substring(splitInd, line.Length-11) ));//tmr
					}else if((line.Length>7)&&(line.Substring(0,7).CompareTo("size.y=") == 0)){
						splitInd = line.IndexOf("=")+1;
						size.y=float.Parse(line.Substring(splitInd, line.Length-7) );
					}else if((line.Length>7)&&(line.Substring(0,7).CompareTo("size.x=") == 0)){
						splitInd = line.IndexOf("=")+1;
						size.x=float.Parse(line.Substring(splitInd, line.Length-7) );
					}else if((line.Length>8)&&(line.Substring(0,8).CompareTo("defence=") == 0)){
						splitInd = line.IndexOf("=")+1;
						defence=float.Parse(line.Substring(splitInd, line.Length-8) );
					}else if((line.Length>5)&&(line.Substring(0,5).CompareTo("grav=") == 0)){
						splitInd = line.IndexOf("=")+1;
						grav=float.Parse(line.Substring(splitInd, line.Length-5) );
					}else	if((line.Length>5)&&(line.Substring(0,5).CompareTo("fric=") == 0)){
						splitInd = line.IndexOf("=")+1;
						fric=float.Parse(line.Substring(splitInd, line.Length-5));
					}
			//	}
			}
		}catch(FileNotFoundException e){
			return false;
		}

		return hr;
	}

	public void GroundSpeedReduce(float v){
		//reduces ground speed by specified amount in v
		//and always such that v brings groundspeed closer to zero
		//whether v or groundspeed are negative does not matter
		float a=Mathf.Abs(v);//input sanitization
		if(Mathf.Abs(walk.gndSpeed)<a)
			walk.gndSpeed=0;
		else if(walk.gndSpeed>0)
			walk.gndSpeed-=a;
		else if(walk.gndSpeed<0)
			walk.gndSpeed+=a;
	}

}
