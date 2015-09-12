using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomLightingBehaviours : MonoBehaviour
{
	public SpriteRenderer darknessRenderer;
	bool fadeIn, fadeOut;
	public bool preLit, manualLit;
	GameManager gameManager;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

		//Sets up room's lighting button.
		transform.parent.FindChild("Console View Canvas").FindChild("Lights Button").GetComponent<Button>().onClick.AddListener(lightSwitchEvent);

		//Fades out the darkness on awake if the room is meant to start off lit.
		if(preLit)
			StartCoroutine(FadeOut());
		else
			fadeIn = true;
	}

	//Called via the room's lights button.
	[PunRPC]
	public void lightSwitchEvent()
	{
		if(gameManager.myPlayer.GetComponent<PlayerControl>().usingConsole)
		{
			if(fadeOut)
				StartCoroutine(FadeIn());
			else if(fadeIn)
				StartCoroutine(FadeOut());
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
