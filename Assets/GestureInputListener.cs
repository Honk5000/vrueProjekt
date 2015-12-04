using UnityEngine;
using System.Collections;

public class GestureInputListener : MonoBehaviour {
	public string activateInstrumentKeyName = "ActivateInstrument";
	public string conductKeyName = "Conduct";
	public string virtualHandServerName = "VirtualHandServer(Clone)";

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
	void Update () {
		if (Network.isServer) {
			// Look out for the conducting gestures
			if (Input.GetButtonDown (conductKeyName))
			{
				// a conducting gesture was performed.

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
