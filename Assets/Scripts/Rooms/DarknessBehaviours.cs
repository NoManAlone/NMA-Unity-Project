using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DarknessBehaviours : MonoBehaviour
{
	public SpriteRenderer darknessRenderer;
	bool fadedIn, fadedOut;
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
		transform.parent.FindChild("Console View Canvas").FindChild("Lights Button").GetComponent<Button>().onClick.AddListener(LightsButtonClickEvent);
	}

	void OnJoinedRoom()
	{
			//Fades out the darkness on awake if the room is meant to start off lit.
			if(preLit)
			{
				powerManager.AlterThreshold(-10);
				StartCoroutine(FadeOut());
            }
            else
                fadedIn = true;
	}

	//Called via the room's lights button.
	public void LightsButtonClickEvent()
	{
		if(gameManager.myPlayer.GetComponent<PlayerControl>().usingConsole)
		{
			photonView.RPC("DarknessFadesCall", PhotonTargets.AllBuffered);
		}
	}
	
	[PunRPC]
	void DarknessFadesCall()
	{
		if(fadedOut)
		{
			powerManager.AlterThreshold(10);
			StartCoroutine(FadeIn());
		}
		else if(fadedIn && powerManager.power>=10)
		{
			powerManager.AlterThreshold(-10);
			StartCoroutine(FadeOut());
		}
	}

	IEnumerator FadeIn()
	{
		fadedOut = false;
		fadedIn = true;
		darknessRenderer.enabled = true;
		Color placeholderColor = darknessRenderer.color;
		float startTime = Time.time;

		placeholderColor.a += 0.2f;
		while(placeholderColor.a < 0.98f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 1f, ((Time.time-startTime)/placeholderColor.a)*Time.deltaTime*10);
			darknessRenderer.color = placeholderColor;
			if(fadedOut)
				break;
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		fadedIn = false;
		fadedOut = true;
		Color placeholderColor = darknessRenderer.color;
		float startTime = Time.time;
		
		placeholderColor.a -= 0.2f;
		while(placeholderColor.a > 0.02f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 0f, ((Time.time-startTime)/(placeholderColor.a))*Time.deltaTime*10);
			darknessRenderer.color = placeholderColor;
            if(fadedIn)
                break;
            yield return null;
		}

		if(!fadedIn)
			darknessRenderer.enabled = false;
	}
}
