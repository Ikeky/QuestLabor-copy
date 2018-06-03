using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ButtonFunctions : MonoBehaviour {
	public Transform CodeInputPanel;
	public int X;

	Transform Input;
	Transform Player;
	GameObject[] Doors;
	void Awake(){
		Player = GameObject.FindGameObjectWithTag ("Player").transform;
		Doors = GameObject.FindGameObjectsWithTag ("Door");
		Input = CodeInputPanel.GetChild (0);
	}
	public void CloseCodePanel(){
		CodeInputPanel.gameObject.SetActive (false);
		Player.GetComponent<FirstPersonController> ().enabled = true;
		foreach (GameObject Door in Doors) {
			if (Door.transform.GetComponent<MechanicDoorScript> ()) {
				if (Door.transform.GetComponent<MechanicDoorScript> ().DoorId == X) {
					Door.transform.GetComponent<MechanicDoorScript> ().CheckCode (Input.GetComponent<InputField>().text);
				}
			}
		}
		Input.GetComponent<InputField> ().text = "";
	}
}
