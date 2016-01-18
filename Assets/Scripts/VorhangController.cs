using UnityEngine;
using System.Collections;

public class VorhangController : MonoBehaviour {

	private Vector3 startPos;
	private Vector3 translation = new Vector3 (0, 0, 2.5f);

	// Use this for initialization
	void Start () {
		//transform.position = transform.position - new Vector3 (0, 0, 5);
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(startPos.z - translation.z < transform.position.z)
		{
			transform.position -= translation * Time.deltaTime;
		}
	}
}
