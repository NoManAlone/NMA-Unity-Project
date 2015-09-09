using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour 
{
	void Start()
	{
		transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = false;

		if(GetComponent<Power>().powered)
			FanOn();
		
		else
			FanOff();
	}

	public void SetPowered(bool power)
	{
		if(power)
			FanOn();
		
		else
			FanOff();
	}
	
	void FanOn()
	{
		GetComponent<PointEffector2D>().enabled = true;
		transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = true;
		GetComponent<AudioSource>().PlayDelayed(0f);
	}
	
	void FanOff()
	{
		GetComponent<PointEffector2D>().enabled = false;
		transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = false;
		GetComponent<AudioSource>().Stop();
	}
}
