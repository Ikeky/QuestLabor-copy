using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewObjects : MonoBehaviour {
	//public bool closed;
	public bool baking = true;
	public Transform ChosenObjRotate;

	GameObject[] ScrBr;
	GameObject Player;
	Vector3 ScrollRotate = new Vector3(0,0,0);
	Vector3 Scrolloriginrotation;
	void Awake(){
		ScrBr = GameObject.FindGameObjectsWithTag ("ScrollBars");
		Player = GameObject.FindGameObjectWithTag ("Player");
	}
	void Update(){
		if (!baking) {
			Scrolloriginrotation = ScrollRotate;
			ScrollRotate = new Vector3 (ScrBr [0].GetComponent<Scrollbar> ().value * 360, ScrBr [1].GetComponent<Scrollbar> ().value * 360, 0);
			if (Scrolloriginrotation != ScrollRotate) {
				//transform.RotateAround(Point.position, ScrollRotate, 100 * Time.deltaTime);
				ChosenObjRotate.rotation = Quaternion.Euler (ScrollRotate);
				Scrolloriginrotation = ScrollRotate;
			}
		}
	}
}
