using UnityEngine;
using System.Collections;

public class STimer  {
	public float tmr;
	protected float len;
	public STimer(){
		len = 0;
		tmr = 0;
	}
	public STimer(float t){

		len=t;
		tmr=0;
	}
	public void SetTimer(float t){
		//initialize at len = t and starts the timer
		len=t;
		tmr=t;
	}
	public void SetTimer(){
		//starts timer at preset length
		tmr=len;
	}
	public void ResetTimer(){
		tmr = 0;
		}
	public float GetLen(){
		return len;
	}
	
	public bool IsReady(){
		if(tmr==0)
			return true;
		else
			return false;
	}
	public bool RunTimer(float dT){
		if(tmr>0){
			tmr-=dT;
			if(tmr<=0){
				tmr=0;
				return true;
			}
		}else if(tmr<0){
			tmr+=dT;
			if(tmr>=0){
				tmr=0;
				return true;
			}
		}
		return false;
	}


}
