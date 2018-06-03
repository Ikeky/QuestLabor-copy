using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler{
	public int id;
	public Color Activity;
	public Color Idle;
	public PlayerInventory Player;
	public bool blocked = false;

    Image Slot_img;
	void Awake(){
		Slot_img = GetComponent<Image> ();
	}
	public void OnPointerEnter(PointerEventData eventData){
		Slot_img.color = Color.Lerp (Slot_img.color, Activity, 30 * Time.deltaTime);
	}
	public void OnPointerExit(PointerEventData eventData){
		Slot_img.color = Color.Lerp (Slot_img.color,Idle, 30 * Time.deltaTime);
		
	}
	public void OnPointerClick(PointerEventData eventData){
		if (!Player.ImportingSlot) {
			if (eventData.button == PointerEventData.InputButton.Left && !blocked) {
				if (!Player.Panel.gameObject.activeSelf) {
					Player.ShowPanel (GetComponent<RectTransform> ().anchoredPosition, id);
				} else {
					Player.Panel.gameObject.SetActive (false);
				}
			} else if (eventData.button == PointerEventData.InputButton.Right && !blocked) {
               /* if(CraftingSlot && Player.CraftingSlots[id].id == 0 || !CraftingSlot && Player.Inventory[id].id == 0)
                {
                    return;
                }*/
                Player.Panel.gameObject.SetActive(false);
				Transform item = Instantiate (Player.TestSlot);
				item.SetParent (transform.parent.parent);
				item.localRotation = Quaternion.Euler (Vector3.zero);
				item.localScale = new Vector3 (0.7f, 0.7f, 0.7f);
				item.localPosition = Vector3.zero;
				item.gameObject.tag = "TestSlot";
				Image Icon = item.GetChild (1).GetComponent<Image> ();
				Text Count = Icon.transform.GetChild (0).GetComponent<Text> ();
				Text Name = item.GetChild (2).GetComponent<Text> ();
				if (Player.Inventory [id].id == 0 && Player.Inventory [id].countItem == 0) {
					Name.enabled = false;
					Icon.enabled = false;
					Count.text = "";
				} else {
					Name.enabled = true;
					Icon.enabled = true;
					Name.text = Player.Inventory [id].name;
					Icon.sprite = Resources.Load<Sprite> (Player.Inventory [id].pathIcon);
					if (Player.Inventory [id].countItem == 1) {
						Count.text = "";
					} else {
						Count.text = Player.Inventory [id].countItem.ToString ();
					}
				}
                Player.Slot = item;
                Player.ImportingSlot = true;
				Player.Slotid = id;
				Destroy (item.GetComponent<Slot> ());
				//item.position = Input.mousePosition.normalized;
			}
		}/* else {
			Player.Inventory [id] = Player.Inventory [Player.Slot.GetComponent<Slot>().id];
			Debug.Log (Player.Inventory [id].name);
			//Player.Inventory [Player.Slotid] = new Subject ();
            Player.ChangeInventory ();
			Debug.Log ("Работает");
		}*/
	}
}
