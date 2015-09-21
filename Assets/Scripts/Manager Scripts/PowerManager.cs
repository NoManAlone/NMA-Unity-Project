using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerManager : MonoBehaviour
{
	public float startingMaxPower;
	public float power, threshold, maxPower;
	public float depletionRate = 0;
	Text powerText, thresholdText, maxPowerText;

	public bool p1Jetpacking = false, p2Jetpacking = false;

	PhotonView photonView;

	void Awake()
	{
		photonView = GetComponent<PhotonView>();

		maxPowerText = transform.GetChild(3).GetComponent<Text>();
		thresholdText = transform.GetChild(4).GetComponent<Text>();
		powerText = transform.GetChild(5).GetComponent<Text>();

		maxPower = startingMaxPower;
		threshold = maxPower;
		power = threshold;

		maxPowerText.text = maxPower.ToString();
		thresholdText.text = threshold.ToString();
		powerText.text = power.ToString();
	}

	void Update()
	{
		if(!p1Jetpacking&&!p2Jetpacking)
			RechargePower();
		else
			DepletePower();
	}

	public void AlterMaxPower(int alterValue)
	{
		maxPower = maxPower + alterValue;
		threshold = threshold + alterValue;
		power = power + alterValue;

		if(power>threshold)
			power=threshold;

		maxPowerText.text = maxPower.ToString();
		thresholdText.text = threshold.ToString();
		powerText.text = power.ToString();
	}

	public void AlterThreshold(int alterValue)
	{
		threshold = threshold + alterValue;
		power = power + alterValue;

		if(power>threshold)
			power=threshold;
		
		thresholdText.text = threshold.ToString();
		powerText.text = power.ToString();
	}

	public void RechargePower()
	{
		if(power<threshold)
			power += Time.deltaTime;
		else
			power = threshold;

		powerText.text = power.ToString();
	}

	public void DepletePower()
	{
		if(power>0)
			power = power - Time.deltaTime*depletionRate;
		else
			power = 0;
		
		powerText.text = power.ToString();
	}
	
	[PunRPC]
	public void JetpackStart(bool isPlayer1)
	{
		if(isPlayer1)
			p1Jetpacking = true;
		else
			p2Jetpacking = true;
		
		depletionRate += 2;
	}
	
	[PunRPC]
	public void JetpackStop(bool isPlayer1)
	{
		if(isPlayer1)
			p1Jetpacking = false;
		else
			p2Jetpacking = false;
		
		depletionRate -= 2;
	}
}
