using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	Transform playerTransform;
	public bool consoleView, movingToConsoleView, cameraMoving;
	
	Vector3 cameraStartPos, cameraTargetPos;
	float cameraDefaultSize, cameraStartSize, cameraTargetSize;
	
	public Vector3 positionVelocity = Vector3.zero;
	public float sizeVelocity = 0;

	float cameraMoveTime = .5f;

	float cameraPanSpeed = 1, cameraZoomSpeed = 1;

	float cameraPanLength, cameraZoomLength;

	float startTime;

	void Start ()
	{
		cameraDefaultSize = Camera.main.orthographicSize;
		playerTransform = transform.parent;
	}

	void Update ()
	{
		//Follow Player
		if(!consoleView)
		{
			transform.position = playerTransform.position + new Vector3(0, 4,-10f);
		}

		else if(cameraMoving)
		{
			//ZOOM

			//Distance moved = time * speed.
			float zoomDistCovered = (Time.time - startTime) * cameraZoomSpeed;

			// Fraction of journey completed = current distance divided by total distance.
			float zoomFracJourney = zoomDistCovered / cameraZoomLength;

			// Set our position as a fraction of the distance between the markers.
			Camera.main.orthographicSize = Mathf.Lerp(cameraStartSize, cameraTargetSize, zoomFracJourney);

			//PAN

			// Distance moved = time * speed.
			float distCovered = (Time.time - startTime) * cameraPanSpeed;
			
			// Fraction of journey completed = current distance divided by total distance.
			float fracJourney = distCovered / cameraPanLength;
			
			// Set our position as a fraction of the distance between the markers.
			transform.position = Vector3.Lerp(cameraStartPos, cameraTargetPos, fracJourney);

			if(transform.position == cameraTargetPos)
			{
				cameraMoving = false;

				//moving from console back to player
				if(!movingToConsoleView)
				{
					consoleView = false;
					//transform.parent.GetComponent<PlayerControl>().interacting = false;
				}
			}
		}
	}
	
	public void setConsoleView(Vector2 targetPosition, float targetSize)
	{

		cameraStartPos = transform.position;
		cameraTargetPos = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);	

		cameraStartSize = Camera.main.orthographicSize;
		cameraTargetSize = targetSize;

		cameraPanLength = Vector3.Distance(transform.position, cameraTargetPos);
		cameraZoomLength = cameraTargetSize - cameraStartSize;

		cameraPanSpeed = cameraPanLength/cameraMoveTime;
		cameraZoomSpeed = cameraZoomLength/cameraMoveTime;

		startTime = Time.time;

		movingToConsoleView = true;
		cameraMoving = true;
		consoleView = true;
	}
	
	public void setPlayerView()
	{
		cameraStartPos = transform.position;
		cameraTargetPos = playerTransform.position + new Vector3(0, 4,-10f);

		cameraStartSize = Camera.main.orthographicSize;
		cameraTargetSize = cameraDefaultSize;

		cameraPanLength = Vector3.Distance(transform.position, cameraTargetPos);
		cameraZoomLength = cameraStartSize - cameraTargetSize;
		
		cameraPanSpeed = cameraPanLength/cameraMoveTime;
		cameraZoomSpeed = cameraZoomLength/cameraMoveTime;
		
		startTime = Time.time;

		movingToConsoleView = false;
		cameraMoving = true;
	}

	public void snapToConsoleView(Vector2 targetPosition, float targetSize, GameObject player)
	{

		consoleView = true;

		transform.position = new Vector3(targetPosition.x, targetPosition.y, -10);

		player.transform.Find("Camera").GetComponent<Camera>().orthographicSize = targetSize;
	}

	public void snapToPlayerView(GameObject player)
	{
		consoleView = false;

		transform.position = playerTransform.position + new Vector3(0, 6,-10f);
		player.transform.Find("Camera").GetComponent<Camera>().orthographicSize = cameraDefaultSize;

		//transform.parent.GetComponent<PlayerControl>().interacting = false;
	}
}
