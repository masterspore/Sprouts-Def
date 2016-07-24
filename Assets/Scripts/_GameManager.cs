using UnityEngine;
using System.Collections;

public class _GameManager : MonoBehaviour {

	private GameObject dot;

	void Awake () {

		dot = (GameObject)Resources.Load ("punt");

		dot.GetComponent<LinesController>().activeLine = 0;

		for (int i = 0; i < 6; i++) {
			dot.transform.position = new Vector3(Random.Range(6.0f, -6.0f), Random.Range(3.0f, -3.0f), 0);
			dot.GetComponent<LinesController> ().dotNumber = -2;
			Instantiate (dot);
		}

		// Tornem a dir que el nombre és -1, així el LinesController sap que ja no és un objecte innicial.
		dot.GetComponent<LinesController> ().dotNumber = -1;

	}

	void Start() {

	}
	
	void Update () {
	
	}
}
