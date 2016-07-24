using UnityEngine;
using System.Collections;

public class CheckEndGame : MonoBehaviour {

	GameObject[] dotsList = new GameObject[30];

	CycleCreator cCreator;

	void Start () {
		cCreator = gameObject.GetComponent<CycleCreator> ();
	}
	
	void Update () {
	
	}

	public void CanEnd() {

		dotsList = cCreator.dotList;
		int count = cCreator.dotsCount;
		bool canPlay = false;

		for (int i = 0; i < count; i++) {
			if (cCreator.ConnectionsOfDot (i) < 3) {
				for (int j = 0; j < count; j++) {
					if (cCreator.ConnectionsOfDot(j) < 3 && i != j && DotsCanConnect (i, j)) {
						canPlay = true;
					}
				}
			}
		}

		if (!canPlay) 
			Debug.Log ("Game ended!");
		
	}

	bool DotsCanConnect (int dot1, int dot2) {
		bool canConnect = false;

		LinesController[] dots = new LinesController[30];

		for (int i = 0; i < dotsList.Length; i++) {
			if (dotsList [i] != null) {
				dots [i] = dotsList [i].GetComponent<LinesController> ();
			}
		}

		if (dots [dot1].isFrontier || dots [dot2].isFrontier) {
			for (int i = 0; i < dots [dot1].numOfRegions; i++) {
				for (int j = 0; j < dots [dot2].numOfRegions; j++) {
					if (dots [dot1].regions [i] == dots [dot2].regions [j]) {
						canConnect = true;
					}
				}
			}
		} else {
			canConnect = true;
			for (int i = 0; i < dots [dot1].numOfRegions; i++) {
				for (int j = 0; j < dots [dot2].numOfRegions; j++) {
					if (dots [dot1].regions [i] != dots [dot2].regions [j]) {
						canConnect = false;
					}
				}
			}
		}

		return canConnect;
	}
}
