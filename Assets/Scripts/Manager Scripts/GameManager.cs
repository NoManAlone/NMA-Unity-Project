using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public bool testing_1p, testing_2p;

	public static bool t1p, t2p;

	public Transform p1Spawn,p2Spawn;
	public GameObject splitScreenDivider;

	public GameObject myPlayer;

	void Awake () 
	{
		//These variables can be called directly from any script
		t1p = testing_1p;
		t2p = testing_2p;

		if(testing_1p)
		{
			Instantiate(Resources.Load ("Player 1 Test") , p1Spawn.position, Quaternion.identity);
		}

		else if(testing_2p)
		{
			splitScreenDivider.SetActive(true);

			Instantiate(Resources.Load ("Player 1 Split") , p1Spawn.position, Quaternion.identity);
			Instantiate(Resources.Load ("Player 2 Split") , p2Spawn.position, Quaternion.identity);
		}

		else
		{
			PhotonNetwork.ConnectUsingSettings("v4.2");
		}
	}

	void OnConnectedToPhoton()
	{
		Debug.Log("Connected to Photon");
	}

	void OnFailedToConnectToPhoton()
	{
		Debug.Log("Failed to connect to Photon");
	}

	//Called if Auto-Join Lobby is true in PhotonServerSettings asset
	void OnJoinedLobby()
	{
		Debug.Log("Joined Lobby");

		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
		PhotonNetwork.JoinOrCreateRoom("Game", roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom()
	{	
		Debug.Log("Joined Room");

		if(PhotonNetwork.countOfPlayersInRooms == 0)
			SpawnPlayer("Player 1", p1Spawn.position);

		else
			SpawnPlayer("Player 2", p1Spawn.position);

	}

	void SpawnPlayer(string playerName, Vector3 spawnPosition)
	{
		GameObject player = PhotonNetwork.Instantiate(playerName , spawnPosition, Quaternion.identity, 0);

		//Enable player scripts
		player.GetComponent<PlayerControl>().enabled = true;
		player.GetComponent<PlayerAudio>().enabled = true;
		player.transform.FindChild("Camera").gameObject.SetActive(true);
		//player.transform.FindChild("Particle Emitter").gameObject.SetActive(true);

		myPlayer = player;
	}
}
