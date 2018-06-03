using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {
	[Header("Настройки двери")]
	public int KeyId;
	public int hp = 100;
	public bool Locked = true;

	public bool opened = false;
	public bool opening = false;

	Animator AnimeContoller;
	void Awake(){
		AnimeContoller = transform.parent.GetComponent<Animator> ();
	}
	void Update(){
		if (!Locked && opening) {
			if (!opened) {
				opening = false;
				opened = true;
				AnimeContoller.SetBool ("Opened",true);
				AnimeContoller.SetBool ("Closed",false);
			} else if (opened) {
				opening = false;
				opened = false;
				AnimeContoller.SetBool ("Closed",true);
				AnimeContoller.SetBool ("Opened",false);
			}
		}
	}
	public void OpenLockedDoor(int item_id){
		Debug.Log ("блаблабла");
		if (item_id == KeyId) {
			Locked = false;
			OpenDoor ();
		}
	}
	public void OpenDoor(){
		opening = true;
	}
}
