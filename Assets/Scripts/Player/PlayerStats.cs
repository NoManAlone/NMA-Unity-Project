using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour 
{
	public bool noOxygen, usingPower, restoringOxygen, restoringPower;
	public float health, oxygen = 100, power = 100;

	float oxygenDepletionRate = 5, powerDepletionRate = 10;

	public UIMeters uiMeters;

	// Use this for initialization
	void Start () 
	{
		if(GameManager.t2p)
		{
			uiMeters = transform.parent.Find("ResourcesUI/OxygenMeterAndPowerMeter").GetComponent<UIMeters>();
		}

		else
		{
			uiMeters = GameObject.Find("OxygenMeterAndPowerMeter").GetComponent<UIMeters>();
		}

	}

	// Update is called once per frame
	void Update () 
	{
		Oxygen();
		Power();
	}

	void Oxygen()
	{
		if(noOxygen)
		{
			if(oxygen > 0)
				oxygen -= Time.deltaTime * oxygenDepletionRate;
			
			else 
				oxygen = 0;

			if(oxygen > 10)
				uiMeters.UpdateOxygenMeter(oxygen);
		}
		
		else if(restoringOxygen)
		{
			if(oxygen < 100)
				oxygen += Time.deltaTime;
			
			else
				oxygen = 100;

			if(oxygen > 10)
				uiMeters.UpdateOxygenMeter(oxygen);
		}


	}

	void Power()
	{
		if(usingPower)
		{
			if(power > 0)
			{
				power -= Time.deltaTime * powerDepletionRate;
				uiMeters.UpdatePowerMeter(power);
			}
			
			else 
				power = 0;
		}
		
		else if(restoringPower)
		{
			if(power < 100)
			{
				power += Time.deltaTime;
				uiMeters.UpdatePowerMeter(power);
			}
			
			else
				power = 100;
		}
	}

	public void DecreaseOxygen(bool decreaseOxygen)
	{
		if(decreaseOxygen)
		{
			noOxygen = true;
//			GetComponent<PlayerAudio>().OxygenAlert(true);
		}

		else
		{
			noOxygen = false;
//			GetComponent<PlayerAudio>().OxygenAlert(false);
		}
	}
	
	public void DecreasePower(bool decreasePower)
	{
		if(decreasePower)
		{
			usingPower = true;
		}
		
		else
		{
			usingPower = false;
		}
	}

//	void OnGUI()
//	{		
////		GUI.Label (new Rect (10, 10, 1000, 20), "Oxygen: " + oxygen.ToString("0"));
////		GUI.Label (new Rect (10, 40, 1000, 20), "Power: " + power.ToString("0"));
//	}
}
