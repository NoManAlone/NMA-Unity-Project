using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMenuUI : MonoBehaviour
{
    public Transform menuPanel;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(menuPanel.gameObject.activeSelf)           
                CloseMenu();
            
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuPanel.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        PhotonNetwork.Disconnect();
        Application.LoadLevel("Start Screen");
    }
}
