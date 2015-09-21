using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorBehaviours : MonoBehaviour 
{
	AudioSource[] sfx;
	BoxCollider2D doorCollider;
	Animator doorAnimator;

	PowerManager powerManager;
	PhotonView photonView;
	GameManager gameManager;

	public bool open;

	void Awake()
	{
		//Sets up door's console button.
		transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(DoorSwitchEvent);

		sfx = GetComponents<AudioSource>();
		doorCollider = GetComponent<BoxCollider2D>();
		doorAnimator = GetComponent<Animator>();

		photonView = GetComponent<PhotonView>();
		powerManager = GameObject.Find("PowerMeter").GetComponent<PowerManager>();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void OnJoinedRoom()
	{
		if(open && powerManager.power >= 5)
			OpenDoor();
	}

	public void DoorSwitchEvent()
	{
		if(gameManager.myPlayer.GetComponent<PlayerControl>().usingConsole)
		{
			if(!open && powerManager.power >= 5)
				photonView.RPC("OpenDoor", PhotonTargets.AllBuffered);
			else
				photonView.RPC("CloseDoor", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	void OpenDoor()
	{
		doorCollider.enabled = false;
		doorAnimator.SetBool("Open", true);
		sfx[0].PlayDelayed(0f);
		open = true;
		
		powerManager.AlterThreshold(-5);
	}

	[PunRPC]
	void CloseDoor()
	{
		doorCollider.enabled = true;
		doorAnimator.SetBool("Open", false);
		sfx[1].PlayDelayed(0f);
		open = false;

		powerManager.AlterThreshold(5);
	}
}
