using UnityEngine;
using System.Collections;


public class  AttkVtx {
 
	public float frame;
	public SPoint pos;
	public float dmg; //damage related to attack
	public float dir; //direction the attack will hit towards
	public float mag; //velocity the attack will confer
	public float wgt; //weight used in the hit function
	public float rad; //weight used in the hit function
	public float pri;//ONLY FOR CENTERPOINT
	public bool brk;//separates hitboxes
	public AttkVtx next;
	
	public AttkVtx( ){
		dmg=0;
		dir=0;
		wgt=0;
		mag=0;
		rad=0;
		pri = 0;
		pos = new SPoint(0,0);
		next = null;
		frame = -1;
		brk = false;
	}
	
	public AttkVtx(SPoint p, float m, float d, float dm, float w){
		next = null;
		frame = -1;
		pos = new SPoint(p.x, p.y);
		mag = m;
		dmg = dm;
		dir = d;
		pri = 0;
		brk = false;
		wgt = w;
		rad = 0;
	}
	
	public AttkVtx(AttkVtx a){
		next = null;
		frame = -1;
		mag = a.mag;
		dmg = a.dmg;
		dir = a.dir;
		brk = a.brk;
		wgt = a.wgt;
		rad = a.rad;
	}
	
	public void CopyFrom(AttkVtx s){
		if (this == s)
			return ;
		next = s.next;
		frame = s.frame;
		mag = s.mag;
		dmg = s.dmg;
		dir = s.dir;
		brk = s.brk;
		wgt = s.wgt;
		pos = s.pos;
		rad = s.rad;
	
	}
	

} 
