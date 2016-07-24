using UnityEngine;
using System.Collections;

public class lineCollider : MonoBehaviour {

	public GameObject parent;
	public int start;
	public int end;
	public int order;
	public int lastCCI;

	void Start () {
		for (int i = 0; i < 300; i++) {
			if (parent.GetComponent<LinesController>().colliders[i] == null) {
				parent.GetComponent<LinesController>().colliders[i] = gameObject;
				break;
			}
		}
	}
	
	void Update () {
		if (parent != null && parent.GetComponent<LinesController> ().CCImmunity - lastCCI > 2) {
			parent = null;
		}
	}
}
