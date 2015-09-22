using UnityEngine;
using System.Collections;

public class ConsoleCameraProperties : MonoBehaviour
{
	LevelDetails levelDetails;
	Camera consoleCamera;

	public void SetPositionAndSize()
	{
		levelDetails = GameObject.Find("Level").GetComponent<LevelDetails>();
		transform.position = levelDetails.levelCentre + new Vector3(0,0,-1);

		consoleCamera = GetComponent<Camera>();
		// Note that orthographic size is half the y-axis length of the camera viewport.
		// X-axis of the viewport is based on aspect ratio.
		if(levelDetails.levelDimensions.x > levelDetails.levelDimensions.y)
			consoleCamera.orthographicSize = (levelDetails.levelDimensions.x/consoleCamera.aspect)/2;
		else
			consoleCamera.orthographicSize = levelDetails.levelDimensions.y/2;
	}
}
