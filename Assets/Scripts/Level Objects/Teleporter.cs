using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour 
{	
	public GameObject pairedTeleporter;
	public Sprite teleporterOn, teleporterOff;
	public bool powered, teleporterA;
	public int powerCost = 2;

	void Start()
	{

	}

	[PunRPC]
	public void TogglePowered(GameObject o)
	{
		if(powered)
			TeleporterOff(o);
		
        else
			TeleporterOn(o);
    }
	
	void TeleporterOn(GameObject o)
	{
		GetComponent<SpriteRenderer>().sprite = teleporterOn;
		powered = true;

		if(o != pairedTeleporter)//!!! loop prevention	
			pairedTeleporter.GetComponent<Teleporter>().TogglePowered(gameObject);
    }
    
	void TeleporterOff(GameObject o)
    {
		GetComponent<SpriteRenderer>().sprite = teleporterOff;
		powered = false;

		if(o != pairedTeleporter)//!!! loop prevention	
			pairedTeleporter.GetComponent<Teleporter>().TogglePowered(gameObject);
	}
}
