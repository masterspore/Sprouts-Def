using UnityEngine;
using System.Collections;

public class RegionCollider : MonoBehaviour {

	public int regionNumber = -1;
	public Vertice[] cycle = new Vertice[17];
	public int count = 0;

	public bool firstTime = true;
	bool dotsChanged = false;

	public GameObject gameManager;
	CycleCreator cycleCreator;
	RegionCreator regionCreator;

	public GameObject[] dots = new GameObject[30];

	void Start () {
		cycleCreator = gameManager.GetComponent<CycleCreator> ();
		regionCreator = gameManager.GetComponent <RegionCreator> ();

		for (int i = 0; i < cycle.Length; i++) {
			bool regionAdded = false;
			int j = 0;
			while (!regionAdded && j < cycleCreator.dotsCount) {
				LinesController dot = cycleCreator.dotList [j].GetComponent<LinesController> ();
				if (cycle [i].number == dot.dotNumber) {
					dot.regions [dot.numOfRegions] = regionNumber;
					dot.numOfRegions++;
					dots [i] = cycleCreator.dotList [j];
					regionAdded = true;
				} else {
					j++;
				}
			}
		}
	}
	
	void Update () {
		if (count == 0 && !dotsChanged) {
			firstTime = false;
			if (regionCreator.insideARegion && regionCreator.numberOfRegions == regionNumber) {
				dotsChanged = true;
			}
		} else if (count >= 0) {
			count--;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "ball") {
			if (firstTime) {
				int numOfConnections = other.gameObject.GetComponent<LinesController> ().activeLine;
				if (numOfConnections == 0) {
					if (other.gameObject.GetComponent<LinesController> ().regions [0] == 0) {
						other.gameObject.GetComponent<LinesController> ().regions [0] = regionNumber;
					} else {
						other.gameObject.GetComponent<LinesController> ().regions [1] = regionNumber;
						other.gameObject.GetComponent<LinesController> ().numOfRegions++;
					}
				}
			} else {
				regionCreator.insideARegion = true;
				if (other.gameObject.GetComponent<LinesController> ().regions [0] == 0) {
					other.gameObject.GetComponent<LinesController> ().regions [0] = regionNumber;
				} else {
					other.gameObject.GetComponent<LinesController> ().regions [other.gameObject.GetComponent<LinesController> ().numOfRegions] = regionNumber;
					other.gameObject.GetComponent<LinesController> ().numOfRegions++;	
				}
			}
		}
	}
}
