using UnityEngine;
using System.Collections;

public class lineAwake : MonoBehaviour {

	public GameObject parent;
	public GameObject endParent;

	// Use this for initialization
	void Start () {
		parent.GetComponent<LinesController> ().lines [parent.GetComponent<LinesController> ().activeLine] = gameObject;
		gameObject.GetComponent<LineRenderer> ().SetPosition (0, Camera.main.ScreenToWorldPoint (Input.mousePosition));

		gameObject.GetComponent<LineRenderer> ().SetPosition (0, parent.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
