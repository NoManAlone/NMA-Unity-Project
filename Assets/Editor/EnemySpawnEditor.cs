using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(EnemySpawnScript))]
public class EnemySpawnEditor : Editor
{

    public string[] enemyTypes = new string[] { "Basic", "Orb" };
    public int enemyIndex = 0;

    //EnemyOrb Variables
    public string[] startCorner = new string[] { "Top Left", "Top Right", "Bottom Right", "Bottom Left" };
    public string[] startDirection = new string[] { "Clockwise", "Counterclockwise"};


    SerializedObject myEnemySpawnObj;

    SerializedProperty selectedEnemyType;

    SerializedProperty orbStartCorner;
    SerializedProperty orbDirection;


    void OnEnable()
    {

        myEnemySpawnObj = new SerializedObject(target);// target is the object that you are trying to make editor changes for. In this case Uni.

        selectedEnemyType = myEnemySpawnObj.FindProperty("enemyType");
        orbStartCorner = myEnemySpawnObj.FindProperty("startCorner");
        orbDirection = myEnemySpawnObj.FindProperty("direction");
    }

    public override void OnInspectorGUI()
    {
        myEnemySpawnObj.Update();//this and the apply modified properties are what actualy sends the data (test1) back to the target script, so we use the actual serialized object that we got from target above


        selectedEnemyType.intValue = EditorGUILayout.Popup("Enemy", selectedEnemyType.intValue, enemyTypes);
        

        //Enemy Selection
        switch (selectedEnemyType.intValue)
        {
            case 0:

                break;

            case 1:
                //Enemy_Orb starting corner
                orbStartCorner.intValue = EditorGUILayout.Popup("Start Corner", orbStartCorner.intValue, startCorner);

                //Enemy_Orb direction
                orbDirection.intValue = EditorGUILayout.Popup("Direction", orbDirection.intValue, startDirection);
                break;
        }


        myEnemySpawnObj.ApplyModifiedProperties();
    }
}