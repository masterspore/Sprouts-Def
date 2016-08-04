using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CycleCreator : MonoBehaviour {

	public bool[,] dotMatrix = new bool[30,30];
	public int dotsCount = 0;
	public GameObject[] dotList = new GameObject[30];

	RegionCreator rCreator;

	public bool playerTurn = false;

	public bool checkEndGame = false;

	void Start () {
		rCreator = gameObject.GetComponent<RegionCreator> ();
	}

	void Update () {
		if (checkEndGame) {
			gameObject.GetComponent<CheckEndGame> ().CanEnd ();
			checkEndGame = false;
		}
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
			checkEndGame = true;
		}

		playerTurn = !playerTurn;

		return number;
	}

	void MakeCycle (int dot0, int dot1, int dot2) {
		Vertice[] cycle = new Vertice[30];
		for (int i = 3; i < 30; i++) {
			cycle [i] = new Vertice(-1, false, false);
		}
		cycle [0] = new Vertice(dot0, false, false);
		cycle [1] = new Vertice(dot1, false, false);
		cycle [2] = new Vertice(dot2, false, false);

		if (ConnectionsOfDot(cycle[2].number) == 3) {
			cycle[2].hasThreeConnections = true;
			Debug.Log (cycle[2].number + " marked for possible revision.");
		}

		if (ConnectionsOfDot (cycle [2].number) > 1 && ConnectionsOfDot (cycle [0].number) > 1) { // Si el punt té més d'una connexió
			//Debug.Log("Possible cycle with dot " + cycle[1].number);

			bool cycleFound = false;
			int targetDot = 0;

			bool dot1found = false;
			int i = 0;
			while (!dot1found) {
				if (dotMatrix [i, cycle [2].number]) {
					if (ConnectionsOfDot (i) > 1 && i != cycle[1].number) {
						dot1found = true;
						targetDot = i;
						//Debug.Log ("Target " + targetDot);

						if (targetDot == cycle [0].number) {
							cycleFound = true;
							rCreator.makeCycle = true;
							rCreator.dots = cycle;
							SetFrontierOf (cycle);
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

						/*if (searchState == 0) {
							Debug.Log ("Looking @ dot " + num + ". Trying to find " + cycle [0].number + ", from " + targetDot);
						} else {
							Debug.Log ("Looking @ dot " + targetDot);
						}*/

						if (searchState == 0 && num == cycle [0].number) { // Hem tancat el cicle
						
							cycle [cycleLength].number = targetDot;
							dotFound = true;
							cycleFound = true;
							rCreator.makeCycle = true;
							rCreator.dots = cycle;
							SetFrontierOf (cycle);
							Debug.Log ("Region found");

						} else if (searchState == 1 && num != cycle [cycleLength - 1].number && !CycleContains(cycle, num)) { // Hem trobat el següent punt

							if (ConnectionsOfDot (num) > 1) {
								//Debug.Log ("Found dot " + targetDot);
								cycle [cycleLength].number = targetDot;
								dotFound = true;
								targetDot = num;
								cycleLength++;
							}

						} 
					}
				
					if (num == dotsCount - 1) { // No existeix un cicle
						if (searchState == 0) { // Hem acabat de buscar el punt de connexió
							//Debug.Log ("Into state 1");
							searchState = 1;
							num = 0;
						} else if (searchState == 1) {
							Debug.Log ("No region found " + ReviseCycle<bool> (cycle, 2));
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

	T ReviseCycle <T> (Vertice[] cycle, int option) { // 0 = cycleLenght, 1 = The actual cycle, 2 = May have other cycles

		Debug.Log ("Revise cycle");

		bool foundDot = false;
		int lastDotFound = 0;
		
		for (int i = cycle.Length-1; i >= 0; i--) {
			if (cycle[i].number != -1) {
				if (cycle[i].hasThreeConnections && !cycle[i].used && !foundDot) {
					cycle [i].toUse = true;
					foundDot = true;
					lastDotFound = i;
				}
			}
		}

		if (foundDot) {
			Debug.Log ("Dot " + cycle[lastDotFound].number + " is at pos " + lastDotFound);
		}

		if (option == 2) {
			return (T) Convert.ChangeType(foundDot, typeof(T));
		} else if (option == 0) {
			return (T) Convert.ChangeType(lastDotFound, typeof(T));
		} else {
			for (int i = lastDotFound+1; i < cycle.Length; i++) {
				cycle [i] = new Vertice (-1, false, false);
			}
			return (T) Convert.ChangeType(cycle, typeof(T));
		}
	} 

	bool CycleContains (Vertice[] cycle, int num) {
		bool contains = false;
		foreach (Vertice i in cycle) {
			if (i.number == num)
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

	void SetFrontierOf (Vertice[] cycle) {
		for (int i = 0; i < dotsCount; i++) {
			for (int j = 0; j < cycle.Length; j++) {
				if (dotList [i].GetComponent<LinesController> ().dotNumber == cycle [j].number) {
					dotList [i].GetComponent<LinesController> ().isFrontier = true;
				}
			}
		}
	}
}