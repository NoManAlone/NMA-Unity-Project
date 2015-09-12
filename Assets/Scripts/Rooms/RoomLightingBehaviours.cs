using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomLightingBehaviours : MonoBehaviour
{
	public SpriteRenderer darknessRenderer;
	bool fadeIn, fadeOut;
	public bool preLit, manualLit;
	GameManager gameManager;
	PowerManager powerManager;
	PhotonView photonView;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		powerManager = GameObject.Find("PowerMeter").GetComponent<PowerManager>();
		photonView = GetComponent<PhotonView>();

		//Sets up room's lighting button.
		transform.parent.FindChild("Console View Canvas").FindChild("Lights Button").GetComponent<Button>().onClick.AddListener(lightSwitchEvent);

		//Fades out the darkness on awake if the room is meant to start off lit.
		if(preLit)
		{
			powerManager.AlterThreshold(-10);
			StartCoroutine(FadeOut());
		}
		else
			fadeIn = true;
	}

	//Called via the room's lights button.
	public void lightSwitchEvent()
	{
		photonView.RPC("lightSwitchCall", PhotonTargets.AllBuffered);
	}
	
	[PunRPC]
	void lightSwitchCall()
	{
		if(gameManager.myPlayer.GetComponent<PlayerControl>().usingConsole)
		{
			if(fadeOut)
			{
				powerManager.AlterThreshold(10);
				StartCoroutine(FadeIn());
			}
			else if(fadeIn)
			{
				powerManager.AlterThreshold(-10);
				StartCoroutine(FadeOut());
			}
		}
	}

	IEnumerator FadeIn()
	{
		fadeOut = false;
		fadeIn = true;
		darknessRenderer.enabled = true;
		Color placeholderColor = darknessRenderer.color;
		float startTime = Time.time;

		placeholderColor.a += 0.2f;
		while(placeholderColor.a < 0.98f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 1f, ((Time.time-startTime)/placeholderColor.a)*Time.deltaTime*10);
			darknessRenderer.color = placeholderColor;
			if(fadeOut)
				break;
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		fadeIn = false;
		fadeOut = true;
		Color placeholderColor = darknessRenderer.color;
		float startTime = Time.time;
		
		placeholderColor.a -= 0.2f;
		while(placeholderColor.a > 0.02f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 0f, ((Time.time-startTime)/(placeholderColor.a))*Time.deltaTime*10);
			darknessRenderer.color = placeholderColor;
            if(fadeIn)
                break;
            yield return null;
		}

		if(!fadeIn)
			darknessRenderer.enabled = false;
	}
}
