using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    //Store list of created games
    RoomInfo[] gamesList;

    public GameObject lobbyUI;

    // Use this for initialization
    void Start ()
    {
        PhotonNetwork.ConnectUsingSettings("v4.2");      
    }

    void OnConnectedToPhoton()
    {
        Debug.Log("Connected to Photon");
    }

    void OnFailedToConnectToPhoton()
    {
        Debug.Log("Failed to connect to Photon");
    }

    ////Called if Auto-Join Lobby is true in PhotonServerSettings asset
    //void OnJoinedLobby()
    //{
    //    Debug.Log("Joined Lobby");

    //    RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
    //    PhotonNetwork.JoinOrCreateRoom("Game", roomOptions, TypedLobby.Default);
    //}


    public void CreateRoom(GameObject inputField)
    {
        string gameName = inputField.GetComponent<InputField>().text;

        RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 2 };
        PhotonNetwork.CreateRoom(gameName, roomOptions, TypedLobby.Default);
    }

    public void JoinRoom(GameObject inputField)
    {
        string gameName = inputField.GetComponent<InputField>().text;

        PhotonNetwork.JoinRoom(gameName);
    }

    //Called after OnJoinedLobby
    void OnReceivedRoomListUpdate()
    {
        //Refresh games list on joined lobby
        lobbyUI.GetComponent<LobbyUIManager>().PopulateGamesList();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        PhotonNetwork.isMessageQueueRunning = false;

        Application.LoadLevel("Donnchadh Test");
    }
}
