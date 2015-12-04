using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	private float nextCheckNotConductingTime = 0;
	private float nextCheckActivatingTime = 0;
	// adjust volume and broadcast it to other players

	private GameObject _instrument = null;
	private NetworkView _iC = null;
	/*private void adjustVolumeAndInformOthers(float vol) {
		if (_instrument == null || _iC == null) {
			List<GameObject> all = InstrumentManager.instance.allInstruments;
			if (all.Count >= 1) {
				// obtain a reference to an instrument controller so we can broadcast the RPC
				_instrument = all [0];
				_iC = _instrument.GetComponent<NetworkView> ();

			}
			else return;
		}
		// set volume on the server
		InstrumentManager.instance.setVolumeForAllInstruments (vol);

		// broadcast an RPC call to others
		_iC.RPC ("setVolumeForAllInstrumentsRPC", RPCMode.Others, vol);
	}*/
	void Update () {
		if (fadeOutDecrementPerSecond==0) fadeOutDecrementPerSecond = 1 / orchestraFadeOutDuration;

		if (Network.isServer) {
			// Look out for the conducting gestures
			if (Input.GetButtonDown (conductKeyName))
			{
				// a conducting gesture was performed.
				nextCheckNotConductingTime = Time.time + orchestraFadeOutDelay;

				if (conductingState == ConductingState.NotConducting || conductingState == ConductingState.FadingOut) {
					// reset volume to 1
					lastVolume = 1;
					InstrumentManager.instance.setVolumeForAllInstruments(1);
				}
				conductingState = ConductingState.Conducting;
			}
			else if (conductingState == ConductingState.Conducting || conductingState == ConductingState.FadingOut && Time.time >= nextCheckNotConductingTime) {
				// we're conducting, but not getting a button press here. also we're over the delay set by orchestraFadeOutDelay.
				// wind down


				float newVolume = lastVolume - fadeOutDecrementPerSecond*Time.deltaTime;
				if (newVolume <= 0) {
					newVolume = 0;
					conductingState = ConductingState.NotConducting;
				}
				else {
					conductingState = ConductingState.FadingOut;

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
					if ( Time.time >= nextCheckActivatingTime) {
						ic.playing = !(ic.playing);
						nextCheckActivatingTime = Time.time + 1; // check again in 2 seconds
					}
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
