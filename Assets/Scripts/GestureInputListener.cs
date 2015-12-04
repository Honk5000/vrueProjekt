using UnityEngine;
using System.Collections;

public class GestureInputListener : MonoBehaviour {
	public string activateInstrumentKeyName = "ActivateInstrument";
	public string conductKeyName = "Conduct";
	public string virtualHandServerName = "VirtualHandServer(Clone)";
	public float orchestraFadeOutDelay = 2; // the amount of seconds before the orchestra starts winding down
	public float orchestraFadeOutDuration = 4; // the amount of time it takes the orchestra to wind down (reach zero volume)

	public NetworkViewID selectedViewID;

	// Use this for initialization
	GameObject _virtualHand;
	GameObject virtualHand {
		get {
			if (_virtualHand == null) {
				_virtualHand = GameObject.Find (virtualHandServerName);
				Debug.Log ("Habe VIRTUAL HAND");
			}
			return _virtualHand;
		}

	}

	void Start () {
	
	}
	 

	// Update is called once per frame
	private enum ConductingState {Conducting, FadingOut, NotConducting};
	private ConductingState conductingState = ConductingState.NotConducting;
	private float fadeOutDecrementPerSecond =0;
	private float lastVolume = 1;

	void Update () {
		if (fadeOutDecrementPerSecond==0) fadeOutDecrementPerSecond = 1 / orchestraFadeOutDuration;

		if (Network.isServer) {
			// Look out for the conducting gestures
			if (Input.GetButtonDown (conductKeyName))
			{
				// a conducting gesture was performed.
				if (conductingState == ConductingState.NotConducting) {
					// reset volume to 1
					lastVolume = 1;
					InstrumentManager.instance.setVolumeForAllInstruments(1);
				}
				conductingState = ConductingState.Conducting;
			}
			else if (conductingState == ConductingState.Conducting) {
				// we're conducting, but not getting a button press here.
				// wind down
				float newVolume = lastVolume - fadeOutDecrementPerSecond*Time.deltaTime;
				if (newVolume <= 0) {
					newVolume = 0;
					conductingState = ConductingState.NotConducting;
				}
				InstrumentManager.instance.setVolumeForAllInstruments(newVolume);
				lastVolume = newVolume;
			}

			GameObject vh = this.virtualHand;

			if (vh != null) {
				HomerInteraction h = vh.GetComponent<HomerInteraction>();
				if (h == null) return;
				//Debug.Log("Habe HOMER");
				GameObject sel = h.selectedInstrument;

				if(sel != null)
					selectedViewID = sel.networkView.viewID;
				else
					selectedViewID = NetworkViewID.unassigned;

				//Debug.Log ("Sending NetwrokViewID: " + selectedViewID);
				networkView.RPC("SyncSelectedRPC", RPCMode.Others, selectedViewID);
				if (sel == null) return;

				//Debug.Log("Habe Selected Object");
				InstrumentController ic = sel.GetComponent<InstrumentController>();

				if (ic == null) return;

				//erst jetzt abfragen, damit auch nur angewählte Objekte markiert werden können
				if (Input.GetButtonDown (activateInstrumentKeyName))
				{
					Debug.Log("Toggle PLAYING");
					ic.playing = !(ic.playing);
				}
			}


		}
	}

	[RPC]
	public void SyncSelectedRPC(NetworkViewID selViewID)
	{
		//Debug.Log ("Getting NetwrokViewID: " + selViewID);
		selectedViewID = selViewID;
	}
}
