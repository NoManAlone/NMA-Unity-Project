using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public bool testing;
	public static bool test;
	public bool debugging;
	public static bool debug;

	public Transform spawn;

	public GameObject myPlayer;

	void Awake () 
	{
		//These variables can be called directly from any script
		test = testing;

		debug = debugging;

		if(testing)
		{
			Instantiate(Resources.Load ("Player 1 Test") , spawn.position, Quaternion.identity);
		}

		//else
		//{
		//	PhotonNetwork.ConnectUsingSettings("v4.2");
		//}
	}

    void Start()
    {
        PhotonNetwork.isMessageQueueRunning = true;

        if (PhotonNetwork.isMasterClient)
        {
            SpawnPlayer("Player 1", spawn.position);
        }

        else
        {
            SpawnPlayer("Player 2", spawn.position + new Vector3(-5, 0, 0));
        }
    }

    //void OnConnectedToPhoton()
    //{
    //	Debug.Log("Connected to Photon");
    //}

    //void OnFailedToConnectToPhoton()
    //{
    //	Debug.Log("Failed to connect to Photon");
    //}

    ////Called if Auto-Join Lobby is true in PhotonServerSettings asset
    //void OnJoinedLobby()
    //{
    //	Debug.Log("Joined Lobby");

    //	RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
    //	PhotonNetwork.JoinOrCreateRoom("Game", roomOptions, TypedLobby.Default);
    //}

    //void OnJoinedRoom()
    //{	
    //	Debug.Log("Joined Room");

    //	if(PhotonNetwork.countOfPlayersInRooms == 0)
    //		SpawnPlayer("Player 1", spawn.position);

    //	else
    //		SpawnPlayer("Player 2", spawn.position + new Vector3(-5,0,0));
    //}

    void SpawnPlayer(string playerName, Vector3 spawnPosition)
    {
        GameObject player = PhotonNetwork.Instantiate(playerName, spawnPosition, Quaternion.identity, 0);

        //Enable player scripts
        player.GetComponent<PlayerControl>().enabled = true;
        player.GetComponent<PlayerAudio>().enabled = true;
        player.transform.FindChild("Camera").gameObject.SetActive(true);
        //player.transform.FindChild("Particle Emitter").gameObject.SetActive(true);

        myPlayer = player;

        if (PhotonNetwork.isMasterClient)
        {

            GameObject[] enemySpawners = GameObject.FindGameObjectsWithTag("EnemySpawner");

            foreach (GameObject enemySpawner in enemySpawners)
            {
                enemySpawner.GetComponent<EnemySpawnScript>().SpawnEnemy();
            }
        }
    }
}
