using UnityEngine;
using System.Collections;

public class SPoint {
	public float x, y;

	float m;

	public SPoint(){
		x = 0;
		y = 0;
	}
	public SPoint(float x0, float y0){
		x = x0;
		y = y0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void AddTo(SPoint p, float scl){
		//adds scl*p to this point's coords
		x+=(scl*p.x);
		y+=(scl*p.y);
		
	}
	public float Dot(SPoint p){
		return x*p.x + y*p.y;
	}
	public SPoint GetNormal(){
		float dist = Mathf.Sqrt(SqDistFromOrigin());
		if(dist==0)
			return new SPoint(0,0);
		//else
		return new SPoint(x/dist, y/dist);
	}
	public float SqDistFromOrigin(){
		/*DO NOT TAKE ROOTS OF NUMBERS
	IF YOU CAN AT ALL AVOID IT
	IT IS SLOOOOOOOOOOW*/
		return (x*x+y*y);
	}
	public float GetDir(){
		//returns the direction (in radians) of a vector from the origin to SPoint coordinates.
		//returns 0 when x&&y==0
		if((x==0)&&(y==0))
			return 0;
		else
			return Mathf.Atan2(y, x);
	}
	public void Add(SPoint p){
		x += p.x;
		y += p.y;
	}
}
