using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnMusicInstruments : MonoBehaviour {

	public GameObject instrumentPrefab;
	private bool isClient = false;

	private List<Color> colourList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnConnectedToServer()
	{
		//I am the client
		isClient = true;
	}

	void OnServerInitialized() 
	{
		//I am the server
		isClient = false;
	}
	
	public void OnTriggerStay (Collider other)
	{
		//If the virtual Hand is in the trigger range
		if(isClient && other.gameObject.name.Equals("VirtualHand(Clone)"))
		{
			if(Input.GetButtonDown("Fire1"))
			{
				//just spawn it in front of this game Object
				Network.Instantiate(instrumentPrefab,transform.position + new Vector3(0, 0, -1) * 1.5f,Quaternion.identity,0);


			}
		}
	}

	public void OnTriggerEnter (Collider other)
	{
		//If the virtual Hand enters the trigger
		if(isClient && other.gameObject.name.Equals("VirtualHand(Clone)"))
		{
			colourList = changeGameObjectColour(gameObject, Color.black);
		}
	}

	public void OnTriggerExit (Collider other)
	{
		//If the virtual Hand exits the trigger
		if(isClient && other.gameObject.name.Equals("VirtualHand(Clone)"))
		{
			resetGameObjectColour(gameObject, colourList);
		}
	}

	public List<Color> changeGameObjectColour(GameObject obj, Color colour)
	{
		List<Color> retList = new List<Color>();
		
		if (obj.GetComponent<Renderer>().Equals(null))
		{
			// do changeGameObjectsColour for all Childs!
			foreach (Transform child in obj.transform)
			{
				List<Color> newList = changeGameObjectColour(child.gameObject, colour);
				retList.AddRange(newList);
			}
			
		}
		else
		{
			//save the old Colour!
			retList.Add(obj.renderer.material.GetColor("_Color"));
			
			//Change the colour of this Objects!
			obj.renderer.material.SetColor("_Color", colour);
		}
		
		return retList;
	}
	
	public int resetGameObjectColour(GameObject obj, List<Color> colourList, int iterator = 0)
	{
		int newIterator = iterator;
		
		if (obj.GetComponent<Renderer>().Equals(null))
		{
			// do changeGameObjectsColour for all Childs!
			foreach (Transform child in obj.transform)
			{
				int addIterator = resetGameObjectColour(child.gameObject, colourList, newIterator);
				//retList.AddRange(newList);
				
				newIterator = addIterator;
			}
			
		}
		else
		{
			
			//Change the colour of this Objects!
			obj.renderer.material.SetColor("_Color", colourList[newIterator]);
			
			newIterator ++;
		}
		
		return newIterator;
	}
}
