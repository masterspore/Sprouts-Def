using UnityEngine;
using System.Collections;

public class RegionCreator : MonoBehaviour {

	public int[] dots;
	public bool makeCycle = false;

	public int precision = 4;
	int vCounts;
	Vector2[] path = new Vector2[150];

	public int[] lastCreatedRegion = new int[17];

	public int numberOfRegions = 0;

	public bool insideARegion = false;

	void Start () {
		
	}
	
	void Update () {
		if (makeCycle) {
			CreateRegion (dots);
			makeCycle = false;
		}
	}

	public void CreateRegion (int[] dots) {
		lastCreatedRegion = dots;
		GameObject region = new GameObject ();

		region.AddComponent<PolygonCollider2D> ();
		region.name = "Region";

		int count = 0;
		for (int i = 0; i < dots.Length; i++) {
			if (dots [i] != -1)
				count++;
		}

		vCounts = 0;
		path = new Vector2[300];

		for (int step = 0; step < count; step++) {
			
			int start = dots [step];
			int end;
			if (step != count - 1) {
				end = dots [step + 1];
			} else {
				end = dots [0];
			}
			RaycastHit2D[] hits = Physics2D.CircleCastAll (gameObject.transform.position, 2000, new Vector3 (1, 0, 0));
			GameObject[] colls = new GameObject[hits.Length];

			bool viceVersa = false;

			int count2 = 0;
			foreach (RaycastHit2D hit in hits) {
				if (hit.collider.tag == "line") {
					if (hit.collider.gameObject.GetComponent<lineCollider> ().start == start && hit.collider.gameObject.GetComponent<lineCollider> ().end == end) {
						colls [count2] = hit.collider.gameObject;
						count2++;
					} else if (hit.collider.gameObject.GetComponent<lineCollider> ().start == end && hit.collider.gameObject.GetComponent<lineCollider> ().end == start) {
						colls [count2] = hit.collider.gameObject;
						count2++;
						viceVersa = true;
					}
				}
			}


			GameObject[] orderedColliders = new GameObject[count2];
			int pos = 0;

			if (!viceVersa) {
				for (int i = 0; i < orderedColliders.Length; i++) {
			
					bool posFound = false;
					int num = 0;

					while (!posFound) {
						if (colls [num].GetComponent<lineCollider> ().order == i) {
							orderedColliders [pos] = colls [num];
							posFound = true;
							pos++;
						} else {
							num++;
						}
					}

				}
			} else {
				for (int i = orderedColliders.Length - 1; i >= 0; i--) {

					bool posFound = false;
					int num = 0;

					while (!posFound) {
						if (colls [num].GetComponent<lineCollider> ().order == i) {
							orderedColliders [pos] = colls [num];
							posFound = true;
							pos++;
						} else {
							num++;
						}
					}

				}
			}

			int presCount = 0;
			foreach (GameObject gObj in orderedColliders) {
				if (presCount == precision) {
					path [vCounts] = gObj.transform.position;
					vCounts++;
					presCount = 0;
				} else {
					presCount++;
				}
			}

		}
		for (int i = vCounts; i < path.Length; i++) {
			path [i] = path [0];
		}

		region.AddComponent<RegionCollider> ();

		numberOfRegions++;
		region.GetComponent<RegionCollider> ().regionNumber = numberOfRegions;
		region.GetComponent<RegionCollider> ().cycle = dots;
		region.GetComponent<RegionCollider> ().count = count;
		region.GetComponent<RegionCollider> ().gameManager = gameObject;

		region.tag = "region";

		region.GetComponent<PolygonCollider2D> ().points = path;
	}


}
