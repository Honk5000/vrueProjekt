using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatarObjectController : UserManagementObjectController {

	private bool lastIsSelected = false;
	private bool isCurrentlySelected = false;
	//private List<GameObject> nearbyInstruments;

	private GameObject leftHandObject = null;

	public GameObject selectedInstrument = null;
	private GameObject lastSelectedInstrument = null;
	public float selectionRadius;

	private Vector3 guitarPosition = new Vector3 (0.351f, 0.147f, 0.375f);
	private Vector3 guitarRotation = new Vector3 (0, 130, 340);
	private Vector3 guitarScale = new Vector3 (0.3f, 0.3f, 0.3f);
	private string guitarHandPoint = "MM R Finger0";

	// Use this for initialization
	void Start () {
		//nearbyInstruments = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		isCurrentlySelected = rigidbody.isKinematic;

		if(isCurrentlySelected)
		{
			//no instrument should be selected, if the avatar is selected by the spacemouse
			selectedInstrument = null;
		}

		if(!isCurrentlySelected && lastIsSelected)
		{
			//the avatar was selected previously and is now set in place:
			//we have to select the nearest instrument that is within the reach of the avatar

			selectedInstrument = null;
			float shortestDistance = selectionRadius;

			//the Update method always occures after the OnTrigger mehtods, so the nearbyInstruments list is already updated

			List<GameObject> allInstruments = InstrumentManager.instance.allInstruments;//GameObject.FindGameObjectsWithTag("SpawnedInstrument");


			foreach(GameObject instrument in allInstruments)
			{
				//check if this instrument is the one with the shortest distance to the avatar
				//float newDistance = Vector3.Distance(instrument.transform.position, transform.position);

				//better: calculate the distance from the avatar position to the closest point of the instruments collider
				Collider instColl = instrument.GetComponent<Collider>();

				if(instColl == null)
					continue;

				Vector3 closestPoint = instColl.ClosestPointOnBounds(transform.position);

				float newDistance = Vector3.Distance(closestPoint, transform.position);

				if(newDistance < shortestDistance)
				{
					//save the newly found instrument
					selectedInstrument = instrument;
					shortestDistance = newDistance;
				}
			}

			if(selectedInstrument != null && selectedInstrument.name.Contains("guitar"))
			{
				//if we selected the guitar, we have to find the left Hand object
				leftHandObject = GameObject.Find(guitarHandPoint);

				// and attach the guitar to the hand
				selectedInstrument.transform.parent = leftHandObject.transform;

				selectedInstrument.rigidbody.useGravity = false;
			}

		}

		//reset the avatar position each update, so he is still attached to the instrument, if the instrument is moved
		if(selectedInstrument != null)
		{
			//we have an instrument: now we need to attach the avatar to it.
			//place him infront of the instrument

			//different for each Instrument
			Vector3 newPosition = selectedInstrument.transform.position;

			if(selectedInstrument.name.Contains("piano"))
			{
				Vector3 forward = selectedInstrument.transform.up * -1;
				Vector3 up = selectedInstrument.transform.forward;
				newPosition += (forward * 1.5f + up * 0.5f);
			}

			if(selectedInstrument.name.Contains("guitar"))
			{
				//different approach:
				//the guitar should be attached to the avatar model

				//dont change avatar position
				newPosition = transform.position;

				selectedInstrument.transform.localPosition = guitarPosition;
				selectedInstrument.transform.localEulerAngles = guitarRotation;

				/*
				Vector3 forward = selectedInstrument.transform.forward * -1;
				Vector3 up = selectedInstrument.transform.right;
				newPosition += (forward * 0.5f + up * 0.5f);
				*/
			}

			if(selectedInstrument.name.Contains("pauke"))
			{
				Vector3 forward = selectedInstrument.transform.forward;
				Vector3 up = selectedInstrument.transform.up;
				newPosition += (forward * 1f + up * 0.5f);
			}

			if(selectedInstrument.name.Contains("Keyboard"))
			{
				Vector3 forward = selectedInstrument.transform.up;
				Vector3 up = selectedInstrument.transform.forward * -1;
				newPosition += (forward * 1f + up * 0.5f);
			}

			transform.position = newPosition;

			// put the selected instrument into "player controlled" mode
			lastSelectedInstrument.networkView.RPC ("setMode", RPCMode.All, InstrumentMode.PlayerControlled);
		}

		//if the instrument is no longer selected
		if(selectedInstrument == null && lastSelectedInstrument != null)
		{
			lastSelectedInstrument.networkView.RPC ("setMode", RPCMode.All, InstrumentMode.AIControlled);
			if  (lastSelectedInstrument.name.Contains("guitar")) {
				//detach guitar from hand!
				lastSelectedInstrument.transform.parent = null;
				
				lastSelectedInstrument.rigidbody.useGravity = true;
			}

		}

		//set this at the end
		lastIsSelected = isCurrentlySelected;
		lastSelectedInstrument = selectedInstrument;
	}

	/*
	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.tag.Equals("SpawnedInstrument"))
		{
			//add the instrument to the nearbyInstruments list
			if(!nearbyInstruments.Contains(coll.gameObject))
			{
				nearbyInstruments.Add(coll.gameObject);
			}
		}
	}

	void OnTriggerExit(Collider coll)
	{
		if(coll.gameObject.tag.Equals("SpawnedInstrument"))
		{
			//remove the instrument from the nearbyInstruments list
			if(nearbyInstruments.Contains(coll.gameObject))
			{
				nearbyInstruments.Remove(coll.gameObject);
			}
		}
	}
	*/
}
