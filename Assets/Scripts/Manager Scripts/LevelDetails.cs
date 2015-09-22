using UnityEngine;
using System.Collections;

public class LevelDetails : MonoBehaviour
{
	/* For use with console camera.
	 * Console camera uses levelWidth and levelHeight to set its size, and levelCentre to set its position.
	 */

	public Vector2 levelDimensions;
	public Vector3 levelCentre;
	
	float Xmin = 0, Xmax = 0, Ymin = 0, Ymax = 0;

	public struct RoomDetails
	{
		public Vector2 roomPosition, roomScale;
	}
	public RoomDetails[] roomDetailsList;

	void Awake()
	{
		roomDetailsList = new RoomDetails[0];
		FindAllRooms(); //Find all rooms.
		GetLevelBounds(); //Get level's boundaries.
		levelCentre = new Vector2((Xmin+Xmax)/2, (Ymin+Ymax)/2); //Set the centre of the whole level.
		levelDimensions = new Vector2(Xmax-Xmin, Ymax-Ymin); //Set the Dimensions of the whole level.
	}

	void FindAllRooms() //Populates the array roomDetailsList.
	{
		GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");
		for(int counter = 0; counter < allRooms.Length; counter++)
		{
			System.Array.Resize(ref roomDetailsList, roomDetailsList.Length+1);
			roomDetailsList[counter].roomPosition = allRooms[counter].transform.position;
			roomDetailsList[counter].roomScale = allRooms[counter].transform.FindChild("RoomArea").lossyScale;
		}
	}

	void GetLevelBounds() //Gets the edges of the level based on the rooms in it.
	{
		for(int counter = 0; counter < roomDetailsList.Length; counter++)
		{
			if(counter == 0) //Set for first case.
			{
				Xmin = roomDetailsList[counter].roomPosition.x - roomDetailsList[counter].roomScale.x;
				Xmax = roomDetailsList[counter].roomPosition.x + roomDetailsList[counter].roomScale.x;
				Ymin = roomDetailsList[counter].roomPosition.y - roomDetailsList[counter].roomScale.y;
				Ymax = roomDetailsList[counter].roomPosition.y + roomDetailsList[counter].roomScale.y;
			}
			else //Check for every other case.
			{
				if ((roomDetailsList[counter].roomPosition.x - roomDetailsList[counter].roomScale.x) < Xmin)
					Xmin = roomDetailsList[counter].roomPosition.x - roomDetailsList[counter].roomScale.x;
				if ((roomDetailsList[counter].roomPosition.x + roomDetailsList[counter].roomScale.x) > Xmax)
					Xmax = roomDetailsList[counter].roomPosition.x + roomDetailsList[counter].roomScale.x;
				if ((roomDetailsList[counter].roomPosition.y - roomDetailsList[counter].roomScale.y) < Ymin)
					Ymin = roomDetailsList[counter].roomPosition.y - roomDetailsList[counter].roomScale.y;
				if ((roomDetailsList[counter].roomPosition.y + roomDetailsList[counter].roomScale.y) > Ymax)
					Ymax = roomDetailsList[counter].roomPosition.y + roomDetailsList[counter].roomScale.y;
			}
		}
	}
}
