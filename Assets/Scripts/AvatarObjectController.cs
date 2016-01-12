using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatarObjectController : UserManagementObjectController {

	public bool lastIsSelected = false;
	public bool isCurrentlySelected = false;
	//private List<GameObject> nearbyInstruments;

	public GameObject selectedInstrument = null;

	// Use this for initialization
	void Start () {
		//nearbyInstruments = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		isCurrentlySelected = rigidbody.isKinematic;

		if(!isCurrentlySelected && lastIsSelected)
		{
			//the avatar was selected previously and is now set in place:
			//we have to select the nearest instrument that is within the reach of the avatar

			selectedInstrument = null;
			float shortestDistance = 9999999;

			//the Update method always occures after the OnTrigger mehtods, so the nearbyInstruments list is already updated

			GameObject[] allInstruments = GameObject.FindGameObjectsWithTag("SpawnedInstrument");

			foreach(GameObject instrument in allInstruments)
			{
				//check if this instrument is the one with the shortest distance to the avatar
				float newDistance = Vector3.Distance(instrument.transform.position, transform.position);
				if(newDistance < shortestDistance)
				{
					//save the newly found instrument
					selectedInstrument = instrument;
					shortestDistance = newDistance;
				}
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
				newPosition += new Vector3(0, 0, -1);
			}

			if(selectedInstrument.name.Contains("guitar"))
			{
				newPosition += Vector3.zero;
			}

			if(selectedInstrument.name.Contains("pauke"))
			{
				newPosition += Vector3.zero;
			}

			if(selectedInstrument.name.Contains("Keyboard"))
			{
				newPosition += Vector3.zero;
			}
		}

		//set this at the end
		lastIsSelected = isCurrentlySelected;
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
