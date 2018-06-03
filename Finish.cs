using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour {
	public int i;
	void OnTriggerEnter(Collider arg){
		if (arg.gameObject.tag == "Player") {
			Application.LoadLevel (i);
		}
	}
}
