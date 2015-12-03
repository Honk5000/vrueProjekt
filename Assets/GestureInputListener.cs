using UnityEngine;
using System.Collections;

public class GestureInputListener : MonoBehaviour {
	public string activateInstrumentKeyName = "Fire2";
	public string virtualHandServerName = "VirtualHandServer(Clone)";

	public GameObject oldSelected = null;

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
			GameObject vh = this.virtualHand;

			if (vh != null) {
				HomerInteraction h = vh.GetComponent<HomerInteraction>();
				if (h == null) return;
				//Debug.Log("Habe HOMER");
				GameObject sel = h.selectedInstrument;
				//tell the gameObject, that it is selected!
				if(sel == null && oldSelected != null)
				{
					//deselect the oldSelected
					oldSelected.GetComponent<InstrumentController>().DeselectObject();
					oldSelected = null;
				}

				if (sel == null) return;

				//Debug.Log("Habe Selected Object");
				InstrumentController ic = sel.GetComponent<InstrumentController>();

				if(sel != oldSelected && sel != null)
				{
					//Debug.Log ("SELECTED");
					//new selection
					if(ic != null)
					{
						sel.GetComponent<InstrumentController>().SelectObject();
					}
					//deselect the old, if it exists
					if(oldSelected != null)
					{
						oldSelected.GetComponent<InstrumentController>().DeselectObject();
					}
					oldSelected = sel;
				}

				if (ic == null) return;

				//erst jetzt abfragen, damit auch nur angewählte Objekte markiert werden können
				if (Input.GetButtonDown (activateInstrumentKeyName))
				{
					//Debug.Log("Toggle PLAYING");
					ic.playing = !(ic.playing);
				}
			}
		}
	}
}
