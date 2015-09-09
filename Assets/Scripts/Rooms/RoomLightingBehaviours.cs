using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomLightingBehaviours : MonoBehaviour
{
	public SpriteRenderer darknessRenderer;
	bool fadingIn, fadingOut;
	public bool preLit, manualLit;

	void Awake()
	{
		//Sets up room's lighting button.
		if(!preLit)
		{
			transform.parent.FindChild("Console View Canvas").FindChild("Lights Button").GetComponent<Button>().onClick.AddListener(lightSwitchEvent);
		}
	}

	//Fades out the darkness overlay if player enters room.
	void OnTriggerEnter2D(Collider2D colliderCheck)
	{
		if(colliderCheck.tag == "Player")
		{
			if(!fadingOut)
				StartCoroutine(FadeOut());
		}
	}

	//Fades in the darkness overlay if player exits room.
	void OnTriggerExit2D(Collider2D colliderCheck)
	{
		if(colliderCheck.tag == "Player")
		{
			if(!fadingIn)
				StartCoroutine(FadeIn());
		}
	}

	//Called via the room's lights button.
	public void lightSwitchEvent()
	{
		print("Switch clicked");
		if(manualLit)
			manualLit = false;
		else
			manualLit = true;
	}

	IEnumerator FadeIn()
	{
		fadingOut = false;
		fadingIn = true;
		darknessRenderer.enabled = true;
		Color placeholderColor = darknessRenderer.color;
		float startTime = Time.time;

		placeholderColor.a += 0.2f;
		while(placeholderColor.a < 0.98f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 1f, ((Time.time-startTime)/placeholderColor.a)*Time.deltaTime*10);
			darknessRenderer.color = placeholderColor;
			if(fadingOut)
				break;
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		fadingIn = false;
		fadingOut = true;

		//If room is not pre-lit or powered, the lights only go up a tiny bit.
		if(preLit||manualLit)
		{
			Color placeholderColor = darknessRenderer.color;
			float startTime = Time.time;
			
			placeholderColor.a -= 0.2f;
			while(placeholderColor.a > 0.02f)
			{
				placeholderColor.a = Mathf.Lerp(placeholderColor.a, 0f, ((Time.time-startTime)/(placeholderColor.a))*Time.deltaTime*10);
				darknessRenderer.color = placeholderColor;
                if(fadingIn)
                    break;
                yield return null;
			}

			if(!fadingIn)
				darknessRenderer.enabled = false;
        }
        else
		{
			Color placeholderColor = darknessRenderer.color;
			float startTime = Time.time;
			
			placeholderColor.a -= 0.2f;
			while(placeholderColor.a > 0.85f)
			{
				placeholderColor.a = Mathf.Lerp(placeholderColor.a, 0f, ((Time.time-startTime)/(placeholderColor.a))*Time.deltaTime*10);
				darknessRenderer.color = placeholderColor;
				if(fadingIn)
                    break;
                yield return null;
            }
        }
	}
}
