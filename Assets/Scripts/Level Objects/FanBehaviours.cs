using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FanBehaviours : MonoBehaviour 
{
	public bool powered;
	PhotonView photonView;
	PowerManager powerManager;
	GameManager gameManager;

	void Awake()
	{
		//Sets up fan's console button.
		transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(FanClickEvent);

		photonView = GetComponent<PhotonView>();
		powerManager = GameObject.Find("PowerMeter").GetComponent<PowerManager>();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void OnJoinedRoom()
	{
		if(powered)
			FanOn();
	}

	public void FanClickEvent()
	{
		if(gameManager.myPlayer.GetComponent<PlayerControl>().usingConsole)
		{
			if(powered && powerManager.power >= 10)
				photonView.RPC("FanOff", PhotonTargets.AllBuffered);
			else
				photonView.RPC("FanOn", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	void FanOn()
	{
		GetComponent<PointEffector2D>().enabled = true;
		transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = true;
		GetComponent<AudioSource>().PlayDelayed(0f);
		powered = true;

		powerManager.AlterThreshold(-10);
	}

	[PunRPC]
	void FanOff()
	{
		GetComponent<PointEffector2D>().enabled = false;
		transform.FindChild("Particles").GetComponent<ParticleSystem>().enableEmission = false;
		GetComponent<AudioSource>().Stop();
		powered = false;

		powerManager.AlterThreshold(10);
	}
}
