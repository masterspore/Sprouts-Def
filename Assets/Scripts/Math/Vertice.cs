using UnityEngine;
using System.Collections;

public class Vertice {

	public int number;
	public bool hasThreeConnections;
	public bool toUse;
	public bool used;

	public Vertice (int num, bool has3, bool use) {
		number = num;
		hasThreeConnections = has3;
		used = use;
	}
}
