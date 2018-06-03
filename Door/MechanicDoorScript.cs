using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class MechanicDoorScript : MonoBehaviour {
	public Transform CodeInputPanel;
	public Item ToolItem;
	[Header("Настройки двери")]
	public int DoorId;
	public string KeyFish;
	public bool Locked = true;

	public bool opened = false;
	public bool opening = false;

	public bool blocked; // НеОбязательнаяКрассмотрению

	Animator AnimeContoller;
	Transform Player;
	void Awake(){
		Player = GameObject.FindGameObjectWithTag ("Player").transform;
		AnimeContoller = transform.parent.GetComponent<Animator> ();
	}
	void Update(){
		if (!Locked && opening && !blocked) {
			if (!opened) {
				opened = true;
				AnimeContoller.SetBool ("Opened",true);
				AnimeContoller.SetBool ("Closed",false);
			} else if (opened) {
				opened = false;
				AnimeContoller.SetBool ("Closed",true);
				AnimeContoller.SetBool ("Opened",false);
			}
			opening = false;
		}
	}
	public void OpenLockedDoor(){
		if (!blocked) {
			CodeInputPanel.gameObject.SetActive (true);
			ToolItem.transform.GetComponent<ButtonFunctions> ().X = DoorId;
			Player.GetComponent<FirstPersonController> ().enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
	public void CheckCode(string Code){
		if (Code == KeyFish) {
			Locked = false;
			opening = true;
		} else {
			ToolItem.name = "Закрыто!";
			Player.GetComponent<PlayerInventory>().NotificationIns (ToolItem,true);
		}
	}
	public void OpenDoor(){
		if (!blocked) {
			opening = true;
		}
	}
}
