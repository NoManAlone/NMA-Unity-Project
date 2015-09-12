using UnityEngine;
using System.Collections;

public class PowerManagerOld : MonoBehaviour 
{
	public int availablePower = 0;
	public int powerModuleCount = 0;

	// Use this for initialization
	void Start () 
	{

	}

	public void IncreaseAvailablePower(int powerCost)
	{
		for(int i = 0; i < powerCost; i++)
		{
			availablePower += 1;
			GetComponent<ConsoleUI>().SetModulePower(availablePower - 1, true);
		}
	}

	public void DecreaseAvailablePower(int powerCost)
	{
		for(int i = 0; i < powerCost; i++)
        {
			availablePower -= 1;
			GetComponent<ConsoleUI>().SetModulePower(availablePower, false);
		}
	}
}
