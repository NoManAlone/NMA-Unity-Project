#if UNITY_EDITOR
// Editor specific code here
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/* ROOM BUILDER
 * Used with the RoomBuilderEditor.cs script.
 * Add to an empty GameObject and craft rooms in the inspector.
 * Most functionality is automated; just keep door offsets within your room's dimensions.
 * Set dimensions and position of the room.
 * Select which walls to include in the room.
 * Set whether or not to include a console.
 * Select the number of doors to spawn.
 * Select the wall on which each door is situated, and its offset from the center of that wall.
 * Build!
 */

public class RoomBuilderScript : MonoBehaviour 
{
	//Prefab Objects
	GameObject room, roomArea, console, levelObject;
	GameObject lightsButton, consoleViewCanvas, darknessOverlay, consoleViewRoom;

	//Room Details
	public string roomName;
	public Vector2 roomDimensions;
	public Vector2 roomPosition;
	Bounds roomBounds;
	public bool darkened, preLit;

	//BG Art
	public int selectedBGArt;
	public GameObject backgroundArtCanvas;
	public Sprite backgroundArtSprite;

	//Walls
	public bool wallTop, wallBottom, wallLeft, wallRight;

	//Doors
	public int doorCount;
	public struct DoorProperties
	{
		public bool prePowered;
		public bool top, bottom, left, right; //Orientation in walls.
		public float offset; //Offset from centre axis of room.
	}
	public DoorProperties[] doors = new DoorProperties[0];
	
	//Console
	public bool consoleRoom;
	public float consoleOffset;

	//Prefabs
	Object roomAreaPrefab;
	Object platformPrefab;
	Object doorPrefab;
	Object consolePrefab;
	Object consoleViewCanvasPrefab;
	Object lightsButtonPrefab;
	Object darknessPrefab;
	Object backgroundArtPrefab;
	Object consoleViewRoomPrefab;
	
	//Called via Editor to make a room.
    public void BuildRoom()
	{
		//Checks for Level object, and creates one if not found.
		if(!GameObject.Find ("Level"))
		{
			levelObject = new GameObject();
			levelObject.name = "Level";
			levelObject.AddComponent<LevelDetails>();
		}
		else
			levelObject = GameObject.Find ("Level");

		roomAreaPrefab = Resources.Load ("RoomArea");
		room = new GameObject();
		room.name = roomName;
		room.tag = "Room";
		room.transform.SetParent(levelObject.transform); //Sets Level as parent object.
		room.transform.localPosition = roomPosition;
		
		//Create Room Size
		roomArea = (GameObject)PrefabUtility.InstantiatePrefab(roomAreaPrefab);
		roomArea.transform.parent = room.transform;	
		roomArea.transform.localPosition = Vector2.zero;
		roomArea.transform.localScale = roomDimensions;
		roomBounds = roomArea.GetComponent<Collider2D>().bounds;

		//Creates Console View Canvas
		CreateConsoleViewCanvas();

		//Create Darkness Overlay and LightsButton
		if(darkened)
		{
			CreateDarkness();
			CreateLightsButton();
		}

        //Create Doors
		BuildDoors();
		//Create Walls
		BuildWalls();
		//Create Console
		if(consoleRoom)
			CreateConsole();

		//Create Background Art
		CreateBackground();
	}
	
	//Creates Console View Canvas.
	void CreateConsoleViewCanvas()
	{
		consoleViewCanvasPrefab = Resources.Load ("Console View Canvas");
		consoleViewCanvas = (GameObject)PrefabUtility.InstantiatePrefab(consoleViewCanvasPrefab);
		
		consoleViewCanvas.transform.SetParent(room.transform);
		consoleViewCanvas.transform.localPosition = Vector2.zero;
		consoleViewCanvas.GetComponent<RectTransform>().sizeDelta = roomDimensions;
	}

	//Creates the darkness overlay in the room for lighting effects.
	void CreateDarkness()
	{
		darknessPrefab = Resources.Load ("Darkness");
		darknessOverlay = (GameObject)PrefabUtility.InstantiatePrefab(darknessPrefab);
		
		darknessOverlay.transform.SetParent(room.transform);
		darknessOverlay.transform.localPosition = Vector2.zero;
		darknessOverlay.transform.localScale = roomDimensions + new Vector2(2,2);
		
		if(preLit)
			darknessOverlay.GetComponent<RoomLightingBehaviours>().preLit = true;
		else
			darknessOverlay.GetComponent<RoomLightingBehaviours>().preLit = false;
	}

	//Creates room's lighting button on the button canvas for console view.
	void CreateLightsButton()
	{
		lightsButtonPrefab = Resources.Load ("Lights Button");
		lightsButton = (GameObject)PrefabUtility.InstantiatePrefab(lightsButtonPrefab);
		
		lightsButton.transform.SetParent(consoleViewCanvas.transform);
		RectTransform lightsButtonRect = lightsButton.GetComponent<RectTransform>();
		lightsButton.transform.localPosition = new Vector2((roomBounds.extents.x - lightsButtonRect.rect.width/2), (roomBounds.extents.y - lightsButtonRect.rect.height/2));
	}

	//Builds the doors of the room.
	void BuildDoors()
	{
		doorPrefab = Resources.Load ("Door");
		for(int counter = 0; counter < doors.Length; counter++) //Cycles through all doors defined in the editor.
		{
			GameObject door = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
			door.name = "Door";
			door.transform.SetParent(room.transform);

			if(doors[counter].prePowered) //Sets door to be powered.
				door.GetComponent<Power>().powered = true;

			if(doors[counter].top||doors[counter].bottom) //Sets rotation of the door.
				door.transform.Rotate(0, 0, 90);
			if(doors[counter].top) //Sets position based on which wall was selected for the door.
				door.transform.localPosition = new Vector2(doors[counter].offset, roomBounds.extents.y + door.GetComponent<Collider2D>().bounds.extents.y);
			else if(doors[counter].bottom)
				door.transform.localPosition = new Vector2(doors[counter].offset, -roomBounds.extents.y - door.GetComponent<Collider2D>().bounds.extents.y);
			else if(doors[counter].left)
				door.transform.localPosition = new Vector2(-roomBounds.extents.x - door.GetComponent<Collider2D>().bounds.extents.x, doors[counter].offset);
			else if(doors[counter].right)
				door.transform.localPosition = new Vector2(roomBounds.extents.x + door.GetComponent<Collider2D>().bounds.extents.x, doors[counter].offset);
			else
				DestroyImmediate(door); //Called if no wall was selected for the door.
		}
	}
	
	//Builds wall segments of the room based on already-spawned doors' positions.
	void BuildWalls()
	{
		platformPrefab = Resources.Load ("Platform");

		if(wallTop)
		{
			DoorProperties[] orderedDoors = new DoorProperties[0];
			//First, gets an array of all walls on this side and sorts them by offset.
			int nextPos = 0;
			for(int counter = 0; counter < doors.Length; counter++)
			{
				if(doors[counter].top)
				{
					System.Array.Resize(ref orderedDoors, orderedDoors.Length+1);
					orderedDoors[nextPos] = doors[counter];
					nextPos++;
				}
			}
			for(int counter = 0; counter < orderedDoors.Length; counter++) //Brute force (suboptimal but works). Does the full cycle as many times as there are entries in the array, to ensure full completion.
			{
				for(int counter1 = 0; counter1 < orderedDoors.Length; counter1++) //Cycles through array.
				{
					for(int counter2 = counter1+1; counter2 < orderedDoors.Length; counter2++) //Iterates through remainder of array, comparing to current entry of cycle.
					{
						if(orderedDoors[counter1].offset > orderedDoors[counter2].offset) //If any other entries are found with a smaller offset, the current entry is swapped with them.
						{
							DoorProperties placeholder = orderedDoors[counter2];
							orderedDoors[counter2] = orderedDoors[counter1];
							orderedDoors[counter1] = placeholder;
						}
					}
				}
			}

			//Second, spawns walls for before first door and after last door.
			if(orderedDoors.Length == 0) //If no doors, simply places the full wall segment.
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Top Platform";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2(0, roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y);
				wall.transform.localScale = new Vector3 (roomBounds.size.x+2, 1, 1);
			}
			else
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Top-Left Platform";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((orderedDoors[0].offset-4f) - (((orderedDoors[0].offset-4f) + (roomBounds.extents.x+1))/2), roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y);
				wall.transform.localScale = new Vector3((roomBounds.extents.x+1) + (orderedDoors[0].offset-4f), 1, 1);

				wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Top-Right Platform";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((roomBounds.extents.x-1) - (((roomBounds.extents.x-3) - (orderedDoors[orderedDoors.Length-1].offset+4))/2), roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y);
				wall.transform.localScale = new Vector3((roomBounds.extents.x+1) - (orderedDoors[orderedDoors.Length-1].offset+4), 1, 1);
			}

			//Third, spawns in the spaces between doors if there are any.
			if(orderedDoors.Length > 1)
			{
				for(int counter = 0; counter < orderedDoors.Length-1; counter++)
				{
					GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
					wall.name = "Top-Mid Platform";
					wall.transform.SetParent(room.transform);
					//Sets position of wall segment by taking the next door's offset and subtracting half the distance between it and the previous door.
					float offsetDifference = ((orderedDoors[counter+1].offset) - (orderedDoors[counter].offset));
					wall.transform.localPosition = new Vector2(orderedDoors[counter+1].offset - offsetDifference/2, roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y);
					//Sets X scale of wall segment by taking the distance between the doors' sprites.
					float boundsDifference = offsetDifference - 8f; //Length 8 taken from sprite.
					wall.transform.localScale = new Vector3(boundsDifference, 1, 1);
				}
			}
		}

		if(wallBottom)
		{
			DoorProperties[] orderedDoors = new DoorProperties[0];
			//First, gets an array of all walls on this side and sorts them by offset.
			int nextPos = 0;
			for(int counter = 0; counter < doors.Length; counter++)
			{
				if(doors[counter].bottom)
				{
					System.Array.Resize(ref orderedDoors, orderedDoors.Length+1);
					orderedDoors[nextPos] = doors[counter];
					nextPos++;
				}
			}
			for(int counter = 0; counter < orderedDoors.Length; counter++) //Brute force (suboptimal but works). Does the full cycle as many times as there are entries in the array, to ensure full completion.
			{
				for(int counter1 = 0; counter1 < orderedDoors.Length; counter1++) //Cycles through array.
				{
					for(int counter2 = counter1+1; counter2 < orderedDoors.Length; counter2++) //Iterates through remainder of array, comparing to current entry of cycle.
					{
						if(orderedDoors[counter1].offset > orderedDoors[counter2].offset) //If any other entries are found with a smaller offset, the current entry is swapped with them.
						{
							DoorProperties placeholder = orderedDoors[counter2];
							orderedDoors[counter2] = orderedDoors[counter1];
							orderedDoors[counter1] = placeholder;
						}
					}
				}
			}
			
			//Second, spawns walls for before first door and after last door.
			if(orderedDoors.Length == 0) //If no doors, simply places the full wall segment.
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Bottom Platform";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2(0, -(roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y));
				wall.transform.localScale = new Vector3 (roomBounds.size.x+2, 1, 1);
			}
			else
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Bottom-Left Platform";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((orderedDoors[0].offset-4f) - (((orderedDoors[0].offset-4f) + (roomBounds.extents.x+1))/2), -(roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y));
				wall.transform.localScale = new Vector3((roomBounds.extents.x+1) + (orderedDoors[0].offset-4f), 1, 1);
				
				wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Bottom-Right Platform";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((roomBounds.extents.x-1) - (((roomBounds.extents.x-3) - (orderedDoors[orderedDoors.Length-1].offset+4))/2), -(roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y));
				wall.transform.localScale = new Vector3((roomBounds.extents.x+1) - (orderedDoors[orderedDoors.Length-1].offset+4), 1, 1);
			}
			
			//Third, spawns in the spaces between doors if there are any.
			if(orderedDoors.Length > 1)
			{
				for(int counter = 0; counter < orderedDoors.Length-1; counter++)
				{
					GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
					wall.name = "Bottom-Mid Platform";
					wall.transform.SetParent(room.transform);
					//Sets position of wall segment by taking the next door's offset and subtracting half the distance between it and the previous door.
					float offsetDifference = ((orderedDoors[counter+1].offset) - (orderedDoors[counter].offset));
					wall.transform.localPosition = new Vector2(orderedDoors[counter+1].offset - offsetDifference/2, -(roomBounds.extents.y + wall.GetComponent<Collider2D>().bounds.extents.y));
					//Sets X scale of wall segment by taking the distance between the doors' sprites.
					float boundsDifference = offsetDifference - 8f; //Length 8 taken from sprite.
					wall.transform.localScale = new Vector3(boundsDifference, 1, 1);
				}
			}
		}

		if(wallLeft)
		{
			DoorProperties[] orderedDoors = new DoorProperties[0];
			//First, gets an array of all walls on this side and sorts them by offset.
			int nextPos = 0;
			for(int counter = 0; counter < doors.Length; counter++)
			{
				if(doors[counter].left)
				{
					System.Array.Resize(ref orderedDoors, orderedDoors.Length+1);
					orderedDoors[nextPos] = doors[counter];
					nextPos++;
				}
			}
			for(int counter = 0; counter < orderedDoors.Length; counter++) //Brute force (suboptimal but works). Does the full cycle as many times as there are entries in the array, to ensure full completion.
			{
				for(int counter1 = 0; counter1 < orderedDoors.Length; counter1++) //Cycles through array.
				{
					for(int counter2 = counter1+1; counter2 < orderedDoors.Length; counter2++) //Iterates through remainder of array, comparing to current entry of cycle.
					{
						if(orderedDoors[counter1].offset > orderedDoors[counter2].offset) //If any other entries are found with a smaller offset, the current entry is swapped with them.
						{
							DoorProperties placeholder = orderedDoors[counter2];
							orderedDoors[counter2] = orderedDoors[counter1];
							orderedDoors[counter1] = placeholder;
						}
					}
				}
			}
			
			//Second, spawns walls for before first door and after last door.
			if(orderedDoors.Length == 0) //If no doors, simply places the full wall segment.
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Left Wall";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2(-(roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x), 0);
				wall.transform.localScale = new Vector3 (1, roomBounds.size.y+2, 1);
			}
			else
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Left-Bottom Wall";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2( -(roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x),(orderedDoors[0].offset-4f) - (((orderedDoors[0].offset-4f) + (roomBounds.extents.y+1))/2));
				wall.transform.localScale = new Vector3(1, (roomBounds.extents.y+1) + (orderedDoors[0].offset-4f), 1);
				
				wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Left-Top Wall";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2(-(roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x),(roomBounds.extents.y-1) - (((roomBounds.extents.y-3) - (orderedDoors[orderedDoors.Length-1].offset+4))/2));
				wall.transform.localScale = new Vector3(1, (roomBounds.extents.y+1) - (orderedDoors[orderedDoors.Length-1].offset+4), 1);
			}
			
			//Third, spawns in the spaces between doors if there are any.
			if(orderedDoors.Length > 1)
			{
				for(int counter = 0; counter < orderedDoors.Length-1; counter++)
				{
					GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
					wall.name = "Left-Mid Wall";
					wall.transform.SetParent(room.transform);
					//Sets position of wall segment by taking the next door's offset and subtracting half the distance between it and the previous door.
					float offsetDifference = ((orderedDoors[counter+1].offset) - (orderedDoors[counter].offset));
					wall.transform.localPosition = new Vector2(-(roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x), orderedDoors[counter+1].offset - offsetDifference/2);
					//Sets X scale of wall segment by taking the distance between the doors' sprites.
					float boundsDifference = offsetDifference - 8f; //Length 8 taken from sprite.
					wall.transform.localScale = new Vector3(1, boundsDifference, 1);
				}
			}
		}

		if(wallRight)
		{
			DoorProperties[] orderedDoors = new DoorProperties[0];
			//First, gets an array of all walls on this side and sorts them by offset.
			int nextPos = 0;
			for(int counter = 0; counter < doors.Length; counter++)
			{
				if(doors[counter].right)
				{
					System.Array.Resize(ref orderedDoors, orderedDoors.Length+1);
					orderedDoors[nextPos] = doors[counter];
					nextPos++;
				}
			}
			for(int counter = 0; counter < orderedDoors.Length; counter++) //Brute force (suboptimal but works). Does the full cycle as many times as there are entries in the array, to ensure full completion.
			{
				for(int counter1 = 0; counter1 < orderedDoors.Length; counter1++) //Cycles through array.
				{
					for(int counter2 = counter1+1; counter2 < orderedDoors.Length; counter2++) //Iterates through remainder of array, comparing to current entry of cycle.
					{
						if(orderedDoors[counter1].offset > orderedDoors[counter2].offset) //If any other entries are found with a smaller offset, the current entry is swapped with them.
						{
							DoorProperties placeholder = orderedDoors[counter2];
							orderedDoors[counter2] = orderedDoors[counter1];
							orderedDoors[counter1] = placeholder;
						}
					}
				}
			}
			
			//Second, spawns walls for before first door and after last door.
			if(orderedDoors.Length == 0) //If no doors, simply places the full wall segment.
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Right Wall";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x), 0);
				wall.transform.localScale = new Vector3 (1, roomBounds.size.y+2, 1);
			}
			else
			{
				GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Right-Bottom Wall";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x),(orderedDoors[0].offset-4f) - (((orderedDoors[0].offset-4f) + (roomBounds.extents.y+1))/2));
				wall.transform.localScale = new Vector3(1, (roomBounds.extents.y+1) + (orderedDoors[0].offset-4f), 1);
				
				wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
				wall.name = "Right-Top Wall";
				wall.transform.SetParent(room.transform);
				wall.transform.localPosition = new Vector2((roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x),(roomBounds.extents.y-1) - (((roomBounds.extents.y-3) - (orderedDoors[orderedDoors.Length-1].offset+4))/2));
				wall.transform.localScale = new Vector3(1, (roomBounds.extents.y+1) - (orderedDoors[orderedDoors.Length-1].offset+4), 1);
			}
			
			//Third, spawns in the spaces between doors if there are any.
			if(orderedDoors.Length > 1)
			{
				for(int counter = 0; counter < orderedDoors.Length-1; counter++)
				{
					GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
					wall.name = "Right-Mid Wall";
					wall.transform.SetParent(room.transform);
					//Sets position of wall segment by taking the next door's offset and subtracting half the distance between it and the previous door.
					float offsetDifference = ((orderedDoors[counter+1].offset) - (orderedDoors[counter].offset));
					wall.transform.localPosition = new Vector2((roomBounds.extents.x + wall.GetComponent<Collider2D>().bounds.extents.x), orderedDoors[counter+1].offset - offsetDifference/2);
					//Sets X scale of wall segment by taking the distance between the doors' sprites.
					float boundsDifference = offsetDifference - 8f; //Length 8 taken from sprite.
					wall.transform.localScale = new Vector3(1, boundsDifference, 1);
				}
			}
		}
	}
	
	//Creates the console object in the room.
	void CreateConsole()
	{
		consolePrefab = Resources.Load ("Console");
		console = (GameObject)PrefabUtility.InstantiatePrefab(consolePrefab);
		console.name = "Console";
		console.transform.SetParent(room.transform);
		
		console.transform.localPosition = new Vector2(consoleOffset, -(roomBounds.extents.y) + console.GetComponent<Collider2D>().bounds.extents.y);
	}

	//Creates the background art canvas of the room.
	void CreateBackground()
	{
		backgroundArtPrefab = Resources.Load ("BG Canvas");
		backgroundArtCanvas = (GameObject)PrefabUtility.InstantiatePrefab(backgroundArtPrefab);

		backgroundArtCanvas.transform.SetParent(room.transform);
		backgroundArtCanvas.transform.localPosition = Vector2.zero;
		backgroundArtCanvas.GetComponent<RectTransform>().sizeDelta = roomDimensions;

		switch(selectedBGArt)
		{
		case 0:
			backgroundArtCanvas.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Andrew's BG");
			break;
		case 1:
			backgroundArtCanvas.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Darkness");
			break;
		default:
			backgroundArtCanvas.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Darkness");
			break;
		}
	}
}
#endif

