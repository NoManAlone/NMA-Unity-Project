using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFades : MonoBehaviour
{
	Image myImage;

	void Awake()
	{
		myImage = GetComponent<Image>();
	}

	public IEnumerator FadeIn(float startDelay)
	{
		yield return new WaitForSeconds(startDelay);

		myImage.enabled = true;
		Color placeholderColor = myImage.color;
		placeholderColor.a = 0.01f;
		float startTime = Time.time;

		while(placeholderColor.a < 0.98f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 1f, ((Time.time-startTime)/placeholderColor.a)*Time.deltaTime);
			myImage.color = placeholderColor;
			yield return null;
		}
	}

	public IEnumerator FadeOut(float startDelay)
	{
		yield return new WaitForSeconds(startDelay);

		Color placeholderColor = myImage.color;
		placeholderColor.a = 1f;
		float startTime = Time.time;

		while(placeholderColor.a > 0.02f)
		{
			placeholderColor.a = Mathf.Lerp(placeholderColor.a, 0f, ((Time.time-startTime)/placeholderColor.a)*Time.deltaTime);
			myImage.color = placeholderColor;
			yield return null;
		}
		myImage.enabled = false;
	}

	public IEnumerator FadeInAndOut(float startDelay, float stayDelay)
	{
		yield return StartCoroutine(FadeIn(startDelay));
		StartCoroutine(FadeOut(stayDelay));
	}

	public void BeginFadeInAndOut(float startDelay, float stayDelay)
	{
		StartCoroutine(FadeInAndOut(startDelay, stayDelay));
	}
}
