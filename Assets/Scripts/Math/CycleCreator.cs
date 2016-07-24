using UnityEngine;
using System.Collections;

public class CycleCreator : MonoBehaviour {

	public bool[,] dotMatrix = new bool[30,30];
	public int dotsCount = 0;
	public GameObject[] dotList = new GameObject[30];

	public int cycleCount = 0;
	public int[,] cycleList = new int[200, 30];

	RegionCreator rCreator;

	public bool playerTurn = false;

	void Start () {
		rCreator = gameObject.GetComponent<RegionCreator> ();
	}

	public int RequestNumber (GameObject connectingDot1, GameObject connectingDot2, bool initialDot, GameObject dot) {

		int number = 0; // Quan demanem un nombre també actualitzem la matriu i els cicles.

		for (int i = 0; i < dotList.Length; i++) {
			if (dotList [i] == null) {
				dotList [i] = dot;
				dotsCount++;
				number = i;
				break;
			}
		}

		dot.GetComponent<LinesController> ().dotNumber = number;

		if (initialDot) { // Initial matrix set up.
			
			for (int i = 0; i < dotsCount; i++) {
				for (int j = 0; j < dotsCount; j++) {
					dotMatrix [i, j] = false;
				}
			}
				
		} else { // We can start creating the matrix connections.
			int cd1number = connectingDot1.GetComponent<LinesController>().dotNumber;
			int cd2number = connectingDot2.GetComponent<LinesController>().dotNumber;

			dotMatrix [cd1number, number] = true;
			dotMatrix [cd2number, number] = true;

			dotMatrix [number, cd1number] = true;
			dotMatrix [number, cd2number] = true;

			MakeCycle (cd1number, number, cd2number);
			gameObject.GetComponent<CheckEndGame> ().CanEnd ();
		}

		playerTurn = !playerTurn;

		return number;
	}

	void MakeCycle (int dot0, int dot1, int dot2) {
		int[] cycle = new int[30];
		for (int i = 3; i < 30; i++) {
			cycle [i] = -1;
		}
		cycle [0] = dot0;
		cycle [1] = dot1;
		cycle [2] = dot2;

		if (ConnectionsOfDot (cycle [2]) > 1 && ConnectionsOfDot (cycle [0]) > 1) { // Si el punt té més d'una connexió
			Debug.Log("Possible cycle with dot " + cycle[1]);

			bool cycleFound = false;
			int targetDot = 0;

			bool dot1found = false;
			int i = 0;
			while (!dot1found) {
				if (dotMatrix [i, cycle [2]]) {
					if (ConnectionsOfDot (i) > 1 && i != cycle[1]) {
						dot1found = true;
						targetDot = i;
						Debug.Log ("Target " + targetDot);

						if (targetDot == cycle [0]) {
							cycleFound = true;
							rCreator.makeCycle = true;
							rCreator.dots = cycle;

							Debug.Log ("Region found");
						}
					}
				}
				i++;
			}

			int cycleLength = 3;

			int c = 0;
			while (!cycleFound && c < dotsCount) {

				bool dotFound = false;
				int searchState = 0; // 0 = punt per tancar el cicle; 1 = punt per fer cicle

				int num = 0;
				while (!dotFound) {

					if (dotMatrix[num, targetDot]) {

						if (searchState == 0) {
							Debug.Log ("Looking @ dot " + num + ". Trying to find " + cycle [0] + ", from " + targetDot);
						} else {
							Debug.Log ("Looking @ dot " + targetDot);
						}

						if (searchState == 0 && num == cycle [0]) { // Hem tancat el cicle
						
							cycle [cycleLength] = targetDot;
							dotFound = true;
							cycleFound = true;
							rCreator.makeCycle = true;
							rCreator.dots = cycle;
							SetFrontierOf (cycle);
							Debug.Log ("Region found");

						} else if (searchState == 1 && num != cycle [cycleLength - 1] && !CycleContains(cycle, num)) { // Hem trobat el següent punt

							if (ConnectionsOfDot (num) > 1) {
								Debug.Log ("Found dot " + targetDot);
								cycle [cycleLength] = targetDot;
								dotFound = true;
								targetDot = num;
								cycleLength++;
							}

						} 
					}
				
					if (num == dotsCount - 1) { // No existeix un cicle
						if (searchState == 0) { // Hem acabat de buscar el punt de connexió
							Debug.Log ("Into state 1");
							searchState = 1;
							num = 0;
						} else if (searchState == 1) {
							Debug.Log ("No region found");
							dotFound = true;
							cycleFound = true;
						}
					} else {
						num++;
					}
				}

				c++;
			}

		}
	}

	bool CycleContains (int[] cycle, int num) {
		bool contains = false;
		foreach (int i in cycle) {
			if (i == num)
				contains = true;
		}
		return contains;
	}

	public int ConnectionsOfDot (int dot) {
		int count = 0;
		for (int i = 0; i < dotsCount; i++) {
			if (dotMatrix [i, dot])
				count++;
		}
		return count;
	}

	void SetFrontierOf (int[] cycle) {
		for (int i = 0; i < dotsCount; i++) {
			for (int j = 0; j < cycle.Length; j++) {
				if (dotList [i].GetComponent<LinesController> ().dotNumber == cycle [j]) {
					dotList [i].GetComponent<LinesController> ().isFrontier = true;
				}
			}
		}
	}
}