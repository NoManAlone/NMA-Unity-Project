﻿using UnityEngine;
using System.Collections;

public class ButtonsCanvasEvents : MonoBehaviour
{
	//Turns on lighting in the room. (Need to update this script once new power system is in place)
	public void LightsButton()
	{
		RoomLightingBehaviours roomLights = transform.parent.parent.FindChild("Darkness").GetComponent<RoomLightingBehaviours>();
		print ("lights clicked: " + roomLights.gameObject.name);
		if(roomLights.manualLit)
			roomLights.manualLit = false;
		else
			roomLights.manualLit = true;
	}
}
