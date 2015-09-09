using UnityEngine;
using System.Collections;

public class UIMeters : MonoBehaviour
{
	RectTransform powerMeter;
	RectTransform oxygenMeter;
	float powerMeterXPosition;

	void Awake()
	{
		oxygenMeter = transform.GetChild(4).GetComponent<RectTransform>();
		powerMeter = transform.GetChild(1).GetComponent<RectTransform>();
		powerMeterXPosition = powerMeter.localPosition.x;
	}

	public void UpdateOxygenMeter(float oxygen)
	{
		oxygenMeter.rotation = Quaternion.Euler(0f, 0f, oxygen - 100f);
	}
	
	public void UpdatePowerMeter(float power)
	{
		powerMeter.localPosition = new Vector3(powerMeterXPosition - ((100f - power)*2.92f), powerMeter.localPosition.y);
		powerMeter.localScale = new Vector3(power*0.012f, 1.2f, 1f);
	}
}
