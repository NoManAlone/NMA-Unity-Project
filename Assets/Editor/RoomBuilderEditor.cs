using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoomBuilderScript))]
public class RoomBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GUIStyle boldFont = new GUIStyle();
		boldFont.fontStyle = FontStyle.Bold;
		RoomBuilderScript builderScript = (RoomBuilderScript)target;

		//Room Name
		GUILayout.Label("Name", boldFont);
		builderScript.roomName = EditorGUILayout.TextField(builderScript.roomName);

		//Background
		string[] BGList = new string[]{"Andrew's BG", "Darkness"};
		builderScript.selectedBGArt = EditorGUILayout.Popup(builderScript.selectedBGArt, BGList);

		//Foreground

		//Dimensions
		GUILayout.Label("");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Dimensions:", boldFont);
		builderScript.roomDimensions = EditorGUILayout.Vector2Field("", builderScript.roomDimensions, GUILayout.MaxWidth(150));
		EditorGUILayout.EndHorizontal();

		//Position
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Position:", boldFont);
		builderScript.roomPosition = EditorGUILayout.Vector2Field("", builderScript.roomPosition, GUILayout.MaxWidth(150));
		EditorGUILayout.EndHorizontal();

		//Walls
		GUILayout.Label("Walls:", boldFont);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Top");
		builderScript.wallTop = EditorGUILayout.Toggle(builderScript.wallTop);
		GUILayout.Label("Bottom");
		builderScript.wallBottom = EditorGUILayout.Toggle(builderScript.wallBottom);
		GUILayout.Label("Left");
		builderScript.wallLeft = EditorGUILayout.Toggle(builderScript.wallLeft);
		GUILayout.Label("Right");
		builderScript.wallRight = EditorGUILayout.Toggle(builderScript.wallRight);
		EditorGUILayout.EndHorizontal();
		
		//Darkened and Prelit
		GUILayout.Label("");
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Darkened:", boldFont);
		builderScript.darkened = EditorGUILayout.Toggle(builderScript.darkened);
		GUILayout.Label ("Prelit:");
		builderScript.preLit = EditorGUILayout.Toggle(builderScript.preLit);
		GUILayout.EndHorizontal();

		//Doors
		GUILayout.Label("");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Doors:", boldFont);
		builderScript.doorCount = EditorGUILayout.IntField(builderScript.doorCount, GUILayout.MaxWidth(100));
		GUILayout.EndHorizontal();
		System.Array.Resize (ref builderScript.doors, builderScript.doorCount);
		for(int counter = 0; counter < builderScript.doorCount; counter++)
		{
			GUILayout.Label("");
			if(builderScript.doors[counter].top)
				GUILayout.Label("Door " + (counter+1) + ": Top", boldFont);
			else if(builderScript.doors[counter].bottom)
				GUILayout.Label("Door " + (counter+1) + ": Bottom", boldFont);
			else if(builderScript.doors[counter].left)
				GUILayout.Label("Door " + (counter+1) + ": Left", boldFont);
			else if(builderScript.doors[counter].right)
				GUILayout.Label("Door " + (counter+1) + ": Right", boldFont);
			else
				GUILayout.Label("Door " + (counter+1) + ": Choose a Wall", boldFont);
			GUILayout.BeginHorizontal();
			if(builderScript.wallTop)
			{
				if(GUILayout.Button("Top", GUILayout.MaxWidth(70)))
				{
					builderScript.doors[counter].top = true;
					builderScript.doors[counter].bottom = false;
					builderScript.doors[counter].left = false;
					builderScript.doors[counter].right = false;
				}
			}
			else builderScript.doors[counter].top = false;
			if(builderScript.wallBottom)
			{
				if(GUILayout.Button("Bottom", GUILayout.MaxWidth(70)))
				{
					builderScript.doors[counter].top = false;
					builderScript.doors[counter].bottom = true;
					builderScript.doors[counter].left = false;
					builderScript.doors[counter].right = false;
				}
			}
			else builderScript.doors[counter].bottom = false;
			if(builderScript.wallLeft)
			{
				if(GUILayout.Button("Left", GUILayout.MaxWidth(70)))
				{
					builderScript.doors[counter].top = false;
					builderScript.doors[counter].bottom = false;
					builderScript.doors[counter].left = true;
					builderScript.doors[counter].right = false;
				}
			}
			else builderScript.doors[counter].left = false;
			if(builderScript.wallRight)
			{
				if(GUILayout.Button("Right", GUILayout.MaxWidth(70)))
				{
					builderScript.doors[counter].top = false;
					builderScript.doors[counter].bottom = false;
					builderScript.doors[counter].left = false;
					builderScript.doors[counter].right = true;
				}
			}
			else builderScript.doors[counter].right = false;

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Offset:");
			builderScript.doors[counter].offset = EditorGUILayout.FloatField(builderScript.doors[counter].offset, GUILayout.MaxWidth(70));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Powered:");
			builderScript.doors[counter].prePowered = EditorGUILayout.Toggle(builderScript.doors[counter].prePowered, GUILayout.MaxWidth(70));
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
		}

		//Switchables
		if(GUILayout.Button("Build Room"))
		{
			builderScript.BuildRoom();
		}
	}
}