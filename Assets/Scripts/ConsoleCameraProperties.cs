using UnityEngine;
using System.Collections;

public class consoleCameraProperties : MonoBehaviour
{
	LevelDetails levelDetails;

	void Awake()
	{
		levelDetails = GameObject.Find("Level").GetComponent<LevelDetails>();
		transform.position = levelDetails.levelCentre;

		if(levelDetails.levelDimensions.x > levelDetails.levelDimensions.y)
			GetComponent<Camera>().orthographicSize = levelDetails.levelDimensions.x/2;
		else
			GetComponent<Camera>().orthographicSize = levelDetails.levelDimensions.y/2;
	}
}
