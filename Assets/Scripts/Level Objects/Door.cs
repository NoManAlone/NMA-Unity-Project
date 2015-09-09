using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	AudioSource[] sfx;
	void Start()
	{
		sfx = GetComponents<AudioSource>();
		if(GetComponent<Power>().powered)
			OpenDoor();
		
		else
			CloseDoor();
	}

	public void SetPowered(bool powered)
	{
		if(powered)
			OpenDoor();

		else
			CloseDoor();
	}

	void OpenDoor()
	{
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<Animator>().SetBool("Open", true);
		sfx[0].PlayDelayed(0f);
	}

	void CloseDoor()
	{
		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<Animator>().SetBool("Open", false);
		sfx[1].PlayDelayed(0f);
	}
}
