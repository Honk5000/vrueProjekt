using UnityEngine;
using System.Collections;

public class VorhangController : MonoBehaviour {

	private Vector3 startPos;
	private Vector3 translation = new Vector3 (0, 0, 2.5f);

	public bool vorhangLinks = false;
	public bool vorhangRechts = false;

	private Vector3 translateLinksRechts = new Vector3 (5f, 0, 0);

	// Use this for initialization
	void Start () {
		//transform.position = transform.position - new Vector3 (0, 0, 5);
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if(vorhangLinks)
		{
			if(startPos.x + translateLinksRechts.x > transform.position.x)
			{
				transform.position += translateLinksRechts * Time.deltaTime / 6f;
			}
		}
		else if(vorhangRechts)
		{
			if(startPos.x - translateLinksRechts.x < transform.position.x)
			{
				transform.position -= translateLinksRechts * Time.deltaTime / 6f;
			}
		}
		else
		{
			if(startPos.z - translation.z < transform.position.z)
			{
				transform.position -= translation * Time.deltaTime;
			}
		}

	}
}
