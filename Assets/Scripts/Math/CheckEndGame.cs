using UnityEngine;
using System.Collections;

public class CheckEndGame : MonoBehaviour {

	GameObject[] dotsList = new GameObject[30];

	CycleCreator cCreator;

	bool playerTurn = false;
	public bool gameEnded = false;
	public GameObject player1;
	public GameObject player2;
	public GameObject win;

	void Start () {
		cCreator = gameObject.GetComponent<CycleCreator> ();
	}
	
	void Update () {
		playerTurn = gameObject.GetComponent<CycleCreator> ().playerTurn;
		if (!gameEnded) {
			if (!playerTurn) {
				player1.GetComponent<SpriteRenderer> ().color = Color.white;
				player2.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
			} else {
				player2.GetComponent<SpriteRenderer> ().color = Color.white;
				player1.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
			}
		} else {
			win.GetComponent<SpriteRenderer> ().color = Color.white;
		}
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
			gameEnded = true;
		
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
					if (dots [dot1].regions [i] == dots [dot2].regions [j] && dots [dot1].regions [i] != -1 && dots [dot2].regions [j] != -1) { // Si dos punts vius que són frontera comparteixen una regió, es poden connectar
						canConnect = true;
					}
				}
			}
		} else {
			canConnect = true;
			for (int i = 0; i < dots [dot1].numOfRegions; i++) {
				for (int j = 0; j < dots [dot2].numOfRegions; j++) {
					if (dots [dot1].regions [i] != dots [dot2].regions [j] && dots [dot1].regions [i] != -1 && dots [dot2].regions [j] != -1) { // Si dos punts vius que no són frontera no comparteixen una regió, no es poden connectar
						canConnect = false;	
					}
				}
			}
		}

		Debug.Log ("Checking dots " + dot1 + " iF: " + dots [dot1].isFrontier + " & " + dot2 + " iF: " + dots [dot2].isFrontier + " Can connect " + canConnect);

		return canConnect;
	}
}
