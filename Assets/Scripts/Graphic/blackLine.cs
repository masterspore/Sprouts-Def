using UnityEngine;
using System.Collections;

public class blackLine : MonoBehaviour {

	private float accel = -0.4f;
	private float vel = 4f;

	void Start () {
	
	}
	
	void Update () {
		if (transform.localScale.x < 1) { 
			transform.localScale += new Vector3 (vel * Time.deltaTime, 0, 0);
			vel += accel * Time.deltaTime;
		} else {
			transform.localScale = new Vector3 (1.01f,1,1);
			vel = 0;
			accel = 0;
		}
	}
}
