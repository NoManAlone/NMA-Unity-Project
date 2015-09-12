using UnityEngine;
using System.Collections;

public class PowerManagerNew : MonoBehaviour
{
	public int power, startingMaxPower, currentMaxPower, totalMaxPower;

	void Start()
	{
		totalMaxPower = startingMaxPower;
	}

	IEnumerator RechargePower()
	{
		if(power<currentMaxPower)
			power = power + Time.deltaTime;
		yield return null;
	}

	IEnumerator DepletePower(float depletionRate)
	{
		if(power<currentMaxPower)
			power = power - depletionRate*Time.deltaTime;
		yield return null;
	}
}
