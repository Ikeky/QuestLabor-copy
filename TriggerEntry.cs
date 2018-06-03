using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEntry : MonoBehaviour {
	public string Entry_tag;
	public Transform Send_er;
	void OnTriggerEnter(Collider arg){
		if (arg.gameObject.tag == Entry_tag) {
			Send_er.GetComponent<MechanicDoorScript> ().blocked = true;
		}
	}
	void OnTriggerExit(Collider arg){
		if (arg.gameObject.tag == Entry_tag) {
			Send_er.GetComponent<MechanicDoorScript> ().blocked = false;
		}
	}
}
