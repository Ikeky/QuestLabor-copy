using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameInstaller : MonoBehaviour {
	public GameObject HelpPanel;
	public GameObject Player;
	public Animator TwoDoors;
	public bool StopMove;
	void Start(){
		if (StopMove && Player) {
			Player.GetComponent<PlayerInventory> ().enabled = false;
			Player.GetComponent<FirstPersonController> ().enabled = false;
			Player.GetComponent<SubjectUse> ().enabled = false;
		}
	}
	public void GoToPlay(){
		Application.LoadLevel (1);
	}
	public void ExitGame(){
		Application.Quit ();
	}
	public void OpentwoDoor(){
		if(TwoDoors){
			TwoDoors.SetBool ("open",true);
		}
	}
	public void CloseHelpPanel(){
		if (HelpPanel && Player) {
			HelpPanel.SetActive (false);
			Player.GetComponent<PlayerInventory> ().enabled = true;
			Player.GetComponent<FirstPersonController> ().enabled = true;
			Player.GetComponent<SubjectUse> ().enabled = true;
		}
	}
}