using UnityEngine;
using System.Collections;

public class UIButtonFunctions : MonoBehaviour
{
	public void ExitGame()
	{
		Application.Quit();
	}

	public void SelectMenu(string menuName)
	{
		if(menuName == "Main")
		{
			transform.GetChild(2).gameObject.SetActive(true);
			transform.GetChild(3).gameObject.SetActive(false);
			transform.GetChild(4).gameObject.SetActive(false);
		}
		else if(menuName == "Create")
		{
			transform.GetChild(2).gameObject.SetActive(false);
			transform.GetChild(3).gameObject.SetActive(true);
			transform.GetChild(4).gameObject.SetActive(false);
		}
		else if(menuName == "Join")
		{
			transform.GetChild(2).gameObject.SetActive(false);
			transform.GetChild(3).gameObject.SetActive(false);
			transform.GetChild(4).gameObject.SetActive(true);
		}
	}
}
