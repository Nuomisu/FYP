using UnityEngine;
using System.Collections;

public class mouseClick : MonoBehaviour {
	RaycastHit hit;
	/*
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 1000)) {
			if (Input.GetMouseButtonDown(0)){
				Debug.Log(hit.collider.name);
			}
		}

		Debug.DrawRay (ray.origin, ray.direction*1000, Color.yellow);
	} */
}
