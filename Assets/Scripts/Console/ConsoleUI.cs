using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConsoleUI : MonoBehaviour 
{
	GameObject canvas;
	public Sprite modulePowered, moduleUnpowered;

	int moduleCount;
	float yPos = -5.5f;

	List<Image> powerModules;

	void Start()
	{	
		moduleCount = GetComponent<PowerManager>().powerModuleCount;

		canvas = GameObject.Find("ConsoleUI");
		powerModules = new List<Image>();

		for(int i = 0; i < moduleCount; i ++)
		{
			CreatePowerModule();
		}

		SetPowered(GameObject.Find ("Console").GetComponent<PowerManager>().availablePower);

		//Set ConsoleUI Inactive
		canvas.SetActive(false);
	}

	void CreatePowerModule()
	{

		GameObject powerModule = (GameObject)Instantiate(Resources.Load("Button_PowerModule"), transform.position, Quaternion.identity);

		Vector3 screenPos = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width, -Screen.height, 0));

		powerModule.transform.position = new Vector2(screenPos.x - (powerModule.GetComponent<Collider2D>().bounds.size.x - 1) , yPos);
		
		yPos += 2.2f;

		//powerModule.transform.parent = canvas.transform;
		powerModule.transform.SetParent(canvas.transform, true);
		
		Image module = powerModule.GetComponent<Image>();	
		powerModules.Add(module);
	}

	public void SetPowered(int powered)
	{
		for(int i = 0; i < powered; i ++)
		{
			powerModules[i].sprite = modulePowered;
		}
	}

	public void SetModulePower(int module, bool power)
	{
		if(power)
			powerModules[module].sprite = modulePowered;

		else
			powerModules[module].sprite = moduleUnpowered;
	}
}
