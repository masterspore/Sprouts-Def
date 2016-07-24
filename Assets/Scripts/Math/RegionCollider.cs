using UnityEngine;
using System.Collections;

public class RegionCollider : MonoBehaviour {

	public int regionNumber = -1;
	public int[] cycle = new int[17];
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
				if (cycle [i] == dot.dotNumber) {
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
					other.gameObject.GetComponent<LinesController> ().regions [0] = regionNumber;
					other.gameObject.GetComponent<LinesController> ().numOfRegions++;
				}
			} else {
				regionCreator.insideARegion = true;
				other.gameObject.GetComponent<LinesController> ().regions [0] = regionNumber;
				other.gameObject.GetComponent<LinesController> ().numOfRegions = 1;	
			}
		}
	}
}
