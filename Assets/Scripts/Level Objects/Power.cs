using UnityEngine;
using System.Collections;

public class Power : MonoBehaviour 
{
	public bool powered;
	public int powerCost;

	[PunRPC]
	public void TogglePowered(string switchableTag)
	{
		if(powered)
			powered = false;
		
		else
			powered = true;

		switch(switchableTag)
		{

			case "Door":
				GetComponent<Door>().SetPowered(powered);
				break;
			case "Fan":
				GetComponent<Fan>().SetPowered(powered);
				break;
		}
	}
}
