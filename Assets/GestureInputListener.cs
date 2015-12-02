using UnityEngine;
using System.Collections;

public class GestureInputListener : MonoBehaviour {
	public string activateInstrumentKeyName = "Fire2";
	public string virtualHandServerName = "VirtualHandServer(Clone)";
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
		if (Input.GetButtonDown (activateInstrumentKeyName)) {
			GameObject vh = this.virtualHand;

			if (vh != null) {
				HomerInteraction h = vh.GetComponent<HomerInteraction>();
				if (h == null) return;
				Debug.Log("Habe HOMER");
				GameObject sel = h.selectedInstrument;
				if (sel == null) return;
				Debug.Log("Habe Selected Object");
				InstrumentController ic = sel.GetComponent<InstrumentController>();
				if (ic == null) return;
				Debug.Log("Toggle PLAYING");
				ic.playing = !(ic.playing);
			}
		}
	}
}
