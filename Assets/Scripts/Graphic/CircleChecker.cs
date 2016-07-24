using UnityEngine;
using System.Collections;

public class CircleChecker : MonoBehaviour {

	public GameObject currentParent;

	void Start () {
	
	}
	
	void Update () {

	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "ball" && other.gameObject != currentParent && other.gameObject.GetComponent<LinesController>().activeLine < 3) {
			currentParent.GetComponent<LinesController>().colliding = true;
			currentParent.GetComponent<LinesController>().lines[currentParent.GetComponent<LinesController>().activeLine].GetComponent<lineAwake>().endParent = other.gameObject;
			other.gameObject.GetComponent<LinesController>().activeLine++;
		}

		if (other.tag == "line") {
			if (currentParent != other.GetComponent<lineCollider>().parent && currentParent.GetComponent<LinesController>().CCImmunity > 1) {
				currentParent.GetComponent<LinesController>().lineTouched = true;
			}
		}
	}
}
