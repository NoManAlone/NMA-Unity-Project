using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LobbyUIManager : MonoBehaviour
{

    public GameObject createGamePanel, joinGamePanel;
    public Transform contentPanelGamesList;

    public GameObject sampleText;

    RoomInfo[] gamesList;
	// Use this for initialization
	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (createGamePanel.activeSelf)
            {

                createGamePanel.SetActive(false);
            }

            else if (joinGamePanel.activeSelf)
            {
                joinGamePanel.SetActive(false);
            }

            else
            {
                PhotonNetwork.Disconnect();
                Application.LoadLevel("Start Screen");
            }
        }
    }

    public void ShowPopup(GameObject panel)
    {
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(panel.transform.GetChild(0).gameObject, null);
    }

    public void ClosePopup(GameObject panel)
    {
        panel.transform.GetChild(0).GetComponent<InputField>().interactable = false;
        panel.SetActive(false);
    }

    public void EnableButtonInteraction(GameObject button)
    {
        button.GetComponent<Button>().interactable = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void DisableButtonInteraction(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
    }
    
    public void PopulateGamesList()
    {

        foreach (Transform child in contentPanelGamesList)
        {
            GameObject.Destroy(child.gameObject);
        }

        gamesList = PhotonNetwork.GetRoomList();

        foreach (RoomInfo game in gamesList)
        {
            GameObject newTextField = Instantiate(sampleText) as GameObject;
            newTextField.GetComponent<Text>().text = game.name;
            newTextField.transform.SetParent(contentPanelGamesList);
        }
    }

    private void HideIfClickedOutside(GameObject panel)
    {
        if (Input.GetMouseButton(0) && panel.activeSelf &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main))
        {
            panel.SetActive(false);
        }
    }
}
