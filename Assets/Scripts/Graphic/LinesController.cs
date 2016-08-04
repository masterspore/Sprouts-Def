using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LinesController : MonoBehaviour {

	public Color player1Color;
	public Color player2Color;

	public GameObject[] lines = new GameObject[3];
	public int activeLine = 0;

	public GameObject circleChecker;
	public int CCImmunity = 0;
	public bool colliding;
	public bool lineTouched = false;
	public GameObject[] colliders = new GameObject[3000];

	GameObject line;
	GameObject lineColl;
	GameObject circle;

	GameObject gameManager;
	// The parents' objects
	public GameObject go1object;
	public GameObject go2object;

	public int vCounts = 1;
	Vector3 lastMousePos = new Vector3(0,0,0);
	Vector3 updatedCurrMousePos;

	bool thisCircle = false;

	public Vector3[] positions = new Vector3[3000];

	bool blackLineDrawn = false;
	GameObject blackLineGO;

	bool drawingLine = false;

	public LayerMask layer;

	public int dotNumber = -1;
	public bool isFrontier = false;
	public int numOfRegions = 0;

	public int[] regions = new int[30];

	void Awake () {
		line = (GameObject)Resources.Load ("line");
		lineColl = (GameObject)Resources.Load ("lineColl");
		circle = (GameObject)Resources.Load ("punt");
		blackLineGO = (GameObject)Resources.Load ("blackLine");

		circleChecker = GameObject.Find ("circleChecker");

		gameManager = GameObject.Find ("_GM");
	}

	void Start () {

		for (int i = 0; i < regions.Length; i++) {
			regions [i] = -1;
		}

		if (dotNumber == -2) /* -2 is for initial objects. */ {
			dotNumber = gameManager.GetComponent<CycleCreator> ().RequestNumber (null, null, true, gameObject);
		} else if (dotNumber == -1) /* -1 is for non initial objects. */ {
			dotNumber = gameManager.GetComponent<CycleCreator> ().RequestNumber (go1object, go2object, false, gameObject);
		}

		regions [0] = 0;
		numOfRegions = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("main");
		}

		if (Input.GetMouseButtonDown (0)) {

			drawingLine = true;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity, layer);

			if (hit && hit.collider.gameObject == gameObject && activeLine < 3) {

				if (gameManager.GetComponent<CycleCreator> ().playerTurn) {
					line.GetComponent<LineRenderer> ().SetColors (player1Color, player1Color);
				} else {
					line.GetComponent<LineRenderer> ().SetColors (player2Color, player2Color);
				}

				line.GetComponent<lineAwake>().parent = gameObject;
				Instantiate(line);
				lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				lastMousePos.z = 0;
				thisCircle = true;
				circleChecker.GetComponent<CircleChecker>().currentParent = gameObject;

				lineColl.GetComponent<lineCollider>().parent = gameObject;

				positions[0] = lastMousePos;
			}
		}  

		if (Input.GetMouseButton (0) && activeLine < 3 && lines[activeLine] != null && thisCircle && drawingLine) {

			updatedCurrMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			updatedCurrMousePos.z = 0;

			circleChecker.transform.position = updatedCurrMousePos;
			
			if(Vector2.Distance(updatedCurrMousePos, lastMousePos) > 0.05){

				vCounts++;
				lines[activeLine].GetComponent<LineRenderer>().SetVertexCount(vCounts);
				lines[activeLine].GetComponent<LineRenderer>().SetPosition(vCounts-1, updatedCurrMousePos);
				CCImmunity++;

				if (vCounts > 2) {
					CreateCollider(positions[vCounts-3], positions[vCounts-2]);
				}

				lastMousePos = updatedCurrMousePos;
				positions[vCounts-1] = lastMousePos;

				if (colliding) {
					thisCircle = false;
				}

				if (lineTouched) {
					lineTouched = false;

					ResetLine();
				}
			}
		}

		if (Input.GetMouseButtonUp (0) && activeLine < 3 && lines [activeLine] != null) {
			lines [activeLine].GetComponent<LineRenderer> ().SetColors (Color.black, Color.black);
			ResetLine();
		}

		if (activeLine == 3 && !blackLineDrawn) {
			Vector3 vecter = gameObject.transform.position;
			vecter.x -= 0.25f;
			vecter.z = 1;
			blackLineGO.transform.position = vecter;
			Instantiate(blackLineGO);
			blackLineDrawn = true;
		}
	}

	void ResetLine () {

		drawingLine = false;
		CCImmunity = 0;

		if (lines [activeLine].GetComponent<lineAwake> ().endParent == null) {
			Destroy(lines[activeLine]);

			for (int i = 0; i < colliders.Length; i++) {
				if (colliders[i] != null) {
					Destroy(colliders[i]);
				}
			}

		} else {
				
			circle.GetComponent<LinesController> ().go1object = gameObject;
			circle.GetComponent<LinesController> ().go2object = lines [activeLine].GetComponent<lineAwake> ().endParent;
			circle.GetComponent<LinesController> ().dotNumber = -1;

			int middle = vCounts / 2;
			int dot = gameManager.GetComponent<CycleCreator> ().dotsCount;

			int order = 0;
			for (int i = 0; i < colliders.Length; i++) {
				if (colliders[i] != null) {
					if (i < middle) {
						colliders [i].GetComponent<lineCollider> ().start = dotNumber;
						colliders [i].GetComponent<lineCollider> ().end = dot;
					} else {
						colliders [i].GetComponent<lineCollider> ().start = dot;
						colliders [i].GetComponent<lineCollider> ().end = lines [activeLine].GetComponent<lineAwake> ().endParent.GetComponent<LinesController>().dotNumber;
					}

					if (i == middle) 
						order = 0;
					
					colliders [i].GetComponent<lineCollider> ().order = order;
					order++;

					colliders [i] = null;
				}
			}

			activeLine++;
			circle.transform.position = positions[middle];
			circle.GetComponent<LinesController>().activeLine = 2;

			Instantiate(circle);
		}
		
		for (int i = 0; i < positions.Length; i++) {
			positions[i] = Vector3.zero;
			vCounts = 1;
		}
		
		colliding = false;
		circleChecker.transform.position = new Vector3(1000, 1000);
	}

	void CreateCollider(Vector3 startPoint, Vector3 endPoint) {
		float angle = Mathf.Rad2Deg * Mathf.Atan2 (endPoint.y - startPoint.y, endPoint.x - startPoint.x);

		lineColl.transform.localEulerAngles = new Vector3 (0, 0, angle);
		lineColl.transform.position = new Vector3 ((startPoint.x + endPoint.x) / 2, (startPoint.y + endPoint.y) / 2);
		lineColl.transform.localScale = new Vector3 (Vector2.Distance (startPoint, endPoint), 1, 1);
		lineColl.tag = "line";

		Instantiate (lineColl);
	}
}

