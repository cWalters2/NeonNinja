using UnityEngine;
using System.Collections;

public class MoveInstruct  {
    
	public const int BURST = 1;
	public const int AIRBURST = 2;
	public const int SLIDE = 3;
	public const int BRAKE = 4;
	public const int AIRBRAKE = 5;
    public const int AIRHOVER = 6;
    public const int WAIT = 10;
	protected int cInd;
	protected STimer[] iTmr;
	protected int[] act;
	protected float[] val;
    protected SPoint[] valSp;
    protected float[] valEnd;
    protected SPoint[] valSpEnd; //for a second parameter
    protected int len = 0;


	public MoveInstruct(){
		len = 0;

		cInd = -1;
    }
	public MoveInstruct(int l){
		len = l;
		iTmr = new STimer[l];
		act = new int[l];
		val = new float[l];
        valEnd = new float[l];
        valSp = new SPoint[l];
        valSpEnd = new SPoint[l];
        cInd = -1;
	}
	public void SetInstruct(int acInd, float ti, string ins, SPoint s){
		iTmr [acInd] = new STimer(ti);
		valSp [acInd] = new SPoint (s.x, s.y);
        valSpEnd[acInd] = new SPoint(s.x, s.y);
        act[acInd] = ActToInd (ins);
        
    }
    public void SetInstructEnd(int acInd, SPoint s)//used for additional information
        //to be tacked onto the last instruction given
    {
       //iTmr[acInd] = new STimer(ti);
        valSpEnd[acInd] = new SPoint(s.x, s.y);
        //valSpEnd[acInd] = new SPoint(s.x, s.y);
       // act[acInd] = ActToInd(ins);

    }
    public void SetInstruct(int acInd, float ti, string ins, float v){
		iTmr [acInd] = new STimer(ti);
		val [acInd] = v;
        valEnd[acInd] = v;
		act[acInd] = ActToInd (ins);

	}
	public void StartAttack(){
		cInd = 0;
		for(int i=0;i<iTmr.Length;i++)
			iTmr [i].SetTimer ();
	}
	public void RunTimers(float timeLapsed, NinjaHelper fh, Stats st){
		if (cInd >= 0) {
			if (iTmr [cInd].RunTimer (timeLapsed)) {
				ExecAction(fh, st, cInd);

				if (cInd < len-1)
					cInd++;
				else
					cInd = -1;
			}else{
				if(act[cInd]==SLIDE){
					ExecAction(fh, st, cInd);

				}
                else if (act[cInd] == AIRHOVER)
                {
                    fh.airborne = true;
                    ExecAction(fh, st, cInd);

                }

            }
		}
	}

	protected int ActToInd(string action){
		if (action.CompareTo ("BURST")==0)
			return BURST;
		if (action.CompareTo ("AIRBURST")==0)
			return AIRBURST;
		if (action.CompareTo ("SLIDE") == 0)
			return SLIDE;
		if (action.CompareTo ("BRAKE")==0)
			return BRAKE;
		if (action.CompareTo ("AIRBRAKE")==0)
			return AIRBRAKE;
		if (action.CompareTo ("AIRHOVER")==0)
			return AIRHOVER;
        if (action.CompareTo("WAIT") == 0)
            return WAIT;
        return 0;//nothing found

	}

	public void ExecAction(NinjaHelper fh, Stats stats, int ind){
		if (act[ind] == BURST) {
			stats.walk.gndSpeed=val[ind]*fh.IntFacingRight();
		}
		if (act[ind] == AIRBURST) {
			stats.motion.vel.x =valSp[ind].x*fh.IntFacingRight();
			stats.motion.vel.y =valSp[ind].y;
			fh.airborne=true;
        }
		if (act[ind] == BRAKE) {
			stats.walk.gndSpeed=stats.walk.gndSpeed*val[ind];
            
        }
        if (act[ind] == AIRBRAKE) {
            stats.motion.vel.x = (1.0f-valSp[ind].x)*stats.motion.vel.x;
			stats.motion.vel.y =(1.0f-valSp[ind].y)*stats.motion.vel.y;

        }
		if (act[ind] == AIRHOVER) {
            if ((valSp[ind].x == valSpEnd[ind].x) && (valSp[ind].y == valSpEnd[ind].y)) {
                stats.motion.vel.x = valSp[ind].x * fh.IntFacingRight();
                stats.motion.vel.y = valSp[ind].y + stats.grav;
            }else{
                float a = iTmr[cInd].tmr/ iTmr[cInd].GetLen();
                float b = 1.0f - a;
                stats.motion.vel.x = a*valSp[ind].x + b*valSpEnd[ind].x * fh.IntFacingRight();
                stats.motion.vel.y = a*valSp[ind].y + b*valSpEnd[ind].y + stats.grav;
            }
		}
		if (act[ind] == SLIDE) {
			stats.walk.gndSpeed=val[ind]*fh.IntFacingRight();
			
        }

    }
}
