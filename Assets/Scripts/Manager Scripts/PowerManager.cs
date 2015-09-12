using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerManager : MonoBehaviour
{
	public float startingMaxPower;
	public float power, threshold, maxPower;
	Text powerText, thresholdText, maxPowerText;

	public bool depleting;

	public IEnumerator jetpackDepletion;

	PhotonView photonView;

	void Start()
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
		if(!depleting)
			RechargePower();
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

	[PunRPC]
	public void RechargePower()
	{
		if(power<threshold)
			power += Time.deltaTime;
		else
			power = threshold;

		powerText.text = power.ToString();
	}

	[PunRPC]
	public void JetpackDepletion(float depletionRate)
	{
		jetpackDepletion = DepletePower(depletionRate);
		StartCoroutine(jetpackDepletion);
	}

	public IEnumerator DepletePower(float depletionRate)
	{
		depleting = true;
		while(power>0 && depleting)
		{
			yield return null;
			if(power>0)
				power = power - Time.deltaTime*depletionRate;
			else
				power = 0;
			
			powerText.text = power.ToString();
		}
	}
}
