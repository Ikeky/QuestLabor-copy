using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInventory : MonoBehaviour
{
	public GraphicRaycaster m_Raycaster;
	PointerEventData m_PointerEventData;
	public EventSystem m_EventSystem;
    [Header("Нужные экземпляры")]

	public Image HitHair;
	public Image CrossHair;
	public Transform TestSlot;
    public GameObject CellGrid;// Это слоты*UI инвентаря
    public Text Detected;// Текст с именем предмета на которую наводищь
    public GameObject NotificationsManager;// Это сетка в которой стоят сообщения
    public GameObject Notification;// сообщение
    public Transform Panel;// Панель с выбором режима предмета
    public GameObject ModelPanel;// Панель с подробностями предмета
    public Transform InfoModel;// Ссылка на обьект с текстом панели подробности предмета
    public Transform ItemPlace;// Место где стоит предмет
    public Item GameInstaller;

	// Плохой метод
	public Animator Mirror;
	// конец Плохого метода
	[HideInInspector]
	public bool ImportingSlot = false;
	[HideInInspector]
	public int Slotid = 0;
    [HideInInspector]
    public Transform Slot;

    [Header("Настройки Инвентаря")]
    //public LayerMask[] layers; 
    public KeyCode KeyShowInventory; //Кнопка открытия инвентаря
    public int EquipDistance = 200; // Расстояние поднятие предмета
    [SerializeField] public List<Subject> Inventory; // Инвентарь с обьектом

    bool ScriptNeeding = false;
    Transform Player;
    int setId;

	bool timer_enabled;
	float timer_period;
	int timer_makeid;
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Inventory = new List<Subject>();
        for (int i = 0; i < CellGrid.transform.childCount; i++)
        {
            Inventory.Add(new Subject());
        }
        for (int i = 0; i < Inventory.Count; i++)
        {
            Transform row = CellGrid.transform.GetChild(i);
            row.GetComponent<Slot>().id = i;
		}
        ChangeInventory();
    }
    void Update()
    {
		if (timer_enabled) {
			Timer ();
		}
        if (ImportingSlot)
        {
            CheckImportingSlot();
        }
        ShowInventory();
		DetectItem();
    }
	void Timer(){
		if (timer_period > 0) {
			timer_period -= Time.deltaTime;
		} else {
			timer_enabled = false;
			timerfunctions ();
		}
	}
	public void ChangeTimer(bool _enabled,int _id,float _time){
		timer_enabled = _enabled;
		timer_makeid = _id;
		timer_period = _time;
		if (_id == 0) {
			Slot[] Slots = CellGrid.transform.GetComponentsInChildren<Slot> ();
			for (int i = 0; i < Slots.Length; i++) {
				Slots [i].blocked = true;
			}
		} else if (_id == 1) {
			Player.GetComponent<Animator>().SetBool("diminish",true);
			Player.GetComponent<FirstPersonController>().enabled = false;
			Player.GetComponent<SubjectUse>().enabled = false;
		} else if (_id == 2) {
			Player.GetComponent<Animator>().SetBool("twist",true);
			Player.transform.GetChild(0).GetChild(0).localRotation = Quaternion.Euler (Vector3.zero);
			Player.transform.localRotation = Quaternion.Euler (new Vector3(0,87.52f,0));
			Player.transform.localPosition = new Vector3 (-2.16f,4.074f,0);//Vector3.Lerp (transform.position, new Vector3 (-2.16f,4.074f,0.01f),1 * Time.deltaTime);
			Player.GetComponent<FirstPersonController>().enabled = false;
			Player.GetComponent<SubjectUse>().enabled = false;
		}
	}
	void timerfunctions(){
		if(timer_makeid == 0){
			Slot[] Slots = CellGrid.transform.GetComponentsInChildren<Slot> ();
			for(int i=0;i<Slots.Length;i++){
				Slots [i].blocked = false;
			}
		} else if(timer_makeid == 1){
			Player.GetComponent<SubjectUse> ().MyModel = null;
			Player.GetComponent<SubjectUse> ().UsingModel = false;
			Destroy(transform.GetChild (0).GetChild(0).GetChild (0).GetChild (0).gameObject);
			Destroy (Player.GetComponent<SubjectUse> ());
			Player.GetComponent<FirstPersonController>().enabled = true;
			Player.GetComponent<FirstPersonController>().m_WalkSpeed = 2;
			Player.GetComponent<FirstPersonController>().m_RunSpeed = 5;
			//Player.GetComponent<SubjectUse>().enabled = true;
		} else if(timer_makeid == 2){
			Player.GetComponent<Animator>().SetBool("twist",false);
			Player.GetComponent<FirstPersonController>().enabled = true;
			Player.GetComponent<SubjectUse>().enabled = true;
			Mirror.SetBool ("m-opened", true);
		} else if(timer_makeid == 3){
			Player.GetComponent<Animator>().SetBool("punch",false);
			HitHair.gameObject.SetActive (false);
		}
	}
	void CheckImportingSlot(){
		Slot.localPosition = Input.mousePosition-new Vector3(1100,580,0);
		if (Input.GetMouseButtonDown (0)) {
			m_PointerEventData = new PointerEventData(m_EventSystem);
			m_PointerEventData.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			m_Raycaster.Raycast(m_PointerEventData, results);
			foreach (RaycastResult result in results)
			{
				Slot example = result.gameObject.transform.GetComponent<Slot> ();
				if (example) {
					if (Inventory[example.id].id == 0)
					{
						Inventory[example.id] = Inventory[Slotid];
						Inventory[Slotid] = new Subject();
					}
					else
					{
						Subject SaveComponent = Inventory[example.id];
						Inventory[example.id] = Inventory[Slotid];
						Inventory[Slotid] = SaveComponent;
					}
                    // ======================= ОШИБКА ================================= //
					ChangeInventory();
					ChangeTimer(true,0,0.3f);
					ImportingSlot = false;
					GameObject TestSlot = GameObject.FindGameObjectWithTag("TestSlot");
					if (TestSlot != null)
					{
						Destroy(TestSlot);
					}

				}
			}
		}
    }
    // Функция* Работающая с перемещением слота
    
    // Функция* Изменение UI слотов для крафта на нынешний
    public void ShowInventory()
    {
		if (Input.GetKeyDown(KeyShowInventory) && Player.GetComponent<FirstPersonController>().m_PreviouslyGrounded && !timer_enabled)
        {
            if (CellGrid.activeSelf)
            {
                CloseReadMorePanel();
                CellGrid.SetActive(false);
                Panel.gameObject.SetActive(false);
				Player.GetComponent<FirstPersonController>().enabled = true;
				if (Player.GetComponent<SubjectUse> ()) {
					Player.GetComponent<SubjectUse>().enabled = true;
				}
            }
            else
            {
                CellGrid.SetActive(true);
				Player.GetComponent<FirstPersonController>().enabled = false;
				if (Player.GetComponent<SubjectUse> ()) {
					Player.GetComponent<SubjectUse> ().enabled = false;
				}
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else if (ScriptNeeding)
        {
            if (CellGrid.activeSelf)
            {
                CloseReadMorePanel();
				CellGrid.SetActive(false);
                Panel.gameObject.SetActive(false);
				Player.GetComponent<FirstPersonController>().enabled = true;
				if (Player.GetComponent<SubjectUse> ()) {
					Player.GetComponent<SubjectUse> ().enabled = true;
				}
            }
            else
            {
                CloseReadMorePanel();
                CellGrid.SetActive(true);
				Player.GetComponent<FirstPersonController>().enabled = false;
				if (Player.GetComponent<SubjectUse> ()) {
					Player.GetComponent<SubjectUse> ().enabled = false;
				}
				Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            ScriptNeeding = false;
        }
    }
    // Функция* открытия и закрытия инвентаря
    public void ChangeInventory()
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            Transform LayOut = CellGrid.transform.GetChild(i);
            Image Icon = LayOut.GetChild(1).GetComponent<Image>();
            Text Count = Icon.transform.GetChild(0).GetComponent<Text>();
            Text Name = LayOut.GetChild(2).GetComponent<Text>();
            if (Inventory[i].id == 0 && Inventory[i].countItem == 0)
            {
                Name.enabled = false;
                Icon.enabled = false;
                Count.text = "";
            }
            else
            {
                Name.enabled = true;
                Icon.enabled = true;
                Name.text = Inventory[i].name;
                Icon.sprite = Resources.Load<Sprite>(Inventory[i].pathIcon);
                if (Inventory[i].countItem == 1)
                {
                    Count.text = "";
                }
                else
                {
                    Count.text = Inventory[i].countItem.ToString();
                }
            }
        }
    }
    // Функция* Изменение UI слотов для инвентаря на нынешний
    public void ShowPanel(Vector2 Dis, int id)
    {
        setId = id;
        if (Inventory[setId].id != 0)
        {
            Panel.gameObject.SetActive(true);
            Panel.GetComponent<RectTransform>().anchoredPosition = Dis + new Vector2(280, 210);
        }
    }
    // Функция* Показ панель предмета 
    void DetectItem()
    {
        RaycastHit pickHit;
	    Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
		if (Physics.Raycast (ray, out pickHit, 3, LayerMask.GetMask ("Door"))) {
			CrossHair.color = Color.blue;
			if (Input.GetMouseButtonDown (0)) {
				if (pickHit.transform.tag == "brakebale") {
					if (GetComponent<SubjectUse> ().MyModel.GetComponent<Item> ().item.id == 201) {
						if (pickHit.transform.GetComponent<DoorScript> ()) {
							DoorScript _script = pickHit.transform.GetComponent<DoorScript> ();
							_script.hp -= 20;
							if (_script.hp <= 0) {
								_script.Locked = false;
								_script.OpenDoor ();
							}
							Player.GetComponent<Animator> ().SetBool ("punch", true);
							HitHair.gameObject.SetActive (true);
							ChangeTimer (true, 3, 0.2f);
							return;
						}
					}
				}
			}
		} else if (Physics.Raycast (ray, out pickHit, EquipDistance, LayerMask.GetMask ("Door"))) {
			CrossHair.color = Color.green;
			if (Input.GetKeyDown (KeyCode.E)) {
				if (pickHit.transform.GetComponent<MechanicDoorScript> ()) {
					MechanicDoorScript _script = pickHit.transform.GetComponent<MechanicDoorScript> ();
					if (_script.Locked) {
						_script.OpenLockedDoor ();
					} else {
						_script.OpenDoor ();
					}
				} else if (pickHit.transform.GetComponent<DoorScript> ()) {
					DoorScript _script = pickHit.transform.GetComponent<DoorScript> ();
					if (!_script.Locked) {
						_script.OpenDoor ();
					}
				}
			}
			if (GetComponent<SubjectUse> ().MyModel) {
				Subject MyModel = GetComponent<SubjectUse> ().MyModel.GetComponent<Item> ().item;
				if (MyModel.id == 2 && Input.GetKeyDown (KeyCode.E)) {
					if (Player.GetComponent<FirstPersonController> ().m_PreviouslyGrounded) {
						if (pickHit.transform.GetComponent<DoorScript> ()) {
							if (pickHit.transform.GetComponent<DoorScript> ()) {
								DoorScript script = pickHit.transform.GetComponent<DoorScript> ();
								if (script.Locked) {
									script.OpenLockedDoor (MyModel.value);
								} else {
									script.OpenDoor ();
								}
							}
						}
					}
				}
			}
		} 
		else if (Physics.Raycast (ray, out pickHit, EquipDistance, LayerMask.GetMask ("Static"))) {
			
		}
		else if (Physics.Raycast(ray, out pickHit, EquipDistance, LayerMask.GetMask("Interactible")))
		{
			CrossHair.color = Color.green;
			if (Input.GetKeyDown (KeyCode.E)) {
				if (GetComponent<SubjectUse> ().MyModel) {
					if (GetComponent<SubjectUse> ().MyModel.GetComponent<Item> ().item.id == 103) {
						ChangeTimer (true, 2, 1.5f);
						pickHit.transform.gameObject.layer = LayerMask.GetMask ("Default");
						pickHit.transform.parent.GetChild (1).gameObject.layer = LayerMask.GetMask ("Default");
					}
				}
				if(pickHit.transform.tag == "box"){
					pickHit.transform.tag = "Untagged";
					NotificationIns(pickHit.transform.GetChild(0).GetComponent<Item>(), false);
					AddItem(pickHit.transform.GetChild(0).GetComponent<Item>().gameObject);
				}
				if(pickHit.transform.tag == "button"){
					GameInstaller.item.name = "Что-то прозвучало около поезда, нужно проверить";
					NotificationIns(GameInstaller, true);
					GameInstaller.GetComponent<GameInstaller> ().OpentwoDoor ();
				}
			}
		}
		else if (Physics.Raycast(ray, out pickHit, EquipDistance, LayerMask.GetMask("Pickable")))
		{
			if (pickHit.transform.GetComponent<Item>() != null)
			{
				CrossHair.color = Color.green;
				Item itemEq = pickHit.transform.GetComponent<Item>();
				if (!Input.GetKeyDown(KeyCode.E))
				{
					Detected.text = itemEq.item.name;
				}
				else
				{
					NotificationIns(itemEq, false);
					AddItem(itemEq.gameObject);
				}
			}
		}
		else if (Physics.Raycast(ray, out pickHit, EquipDistance, LayerMask.GetMask("Item")))
		{
			Item itemEq = pickHit.transform.GetComponent<Item>();
			if (itemEq != null)
			{
				CrossHair.color = Color.green;
				if (!Input.GetKeyDown(KeyCode.E))
				{
					Detected.text = itemEq.item.name;
				}
				else
				{
					NotificationIns(itemEq, false);
					AddItem(itemEq.gameObject);
				}
			}
			else
			{
				itemEq = pickHit.transform.parent.GetComponent<Item>();
				if (itemEq)
				{
					CrossHair.color = Color.green;
					if (!Input.GetKeyDown(KeyCode.E))
					{
						Detected.text = itemEq.item.name;
					}
					else
					{
						NotificationIns(itemEq, false);
						AddItem(itemEq.gameObject);
					}
				}
			}
			return;
		}
		else
		{
			CrossHair.color = Color.red;
		    if (Detected.text != "")
		    {
				Detected.text = "";
		    	return;
			}
		}
    }
    // Функция* Отправляющая Луч, которая проверяет обьект на который смотрит камера
    public void NotificationIns(Item currentItem, bool isText)
    {
        GameObject notifObj = Instantiate(Notification);
        notifObj.transform.SetParent(NotificationsManager.transform);
        notifObj.transform.localPosition = Vector3.zero;
        notifObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        notifObj.transform.localScale = new Vector3(1, 1, 1);
        if (!isText)
        {
			notifObj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(currentItem.item.pathIcon);
        }
        else
        {
            notifObj.transform.GetChild(1).GetComponent<Image>().enabled = false;
            notifObj.transform.GetChild(0).gameObject.SetActive(false);
			notifObj.transform.GetChild (2).transform.localPosition = new Vector3 (-14.5f,0,0);
			notifObj.transform.GetChild(2).GetComponent<Text>().color = Color.white;
			notifObj.transform.GetChild(2).GetComponent<Text>().fontSize = 28;
        }
		notifObj.transform.GetChild(2).GetComponent<Text>().text = currentItem.item.name;
        Destroy(notifObj, 2);
    }
    // Функция* Создающая оповещения на экране
	public void AddItem(GameObject arg)
    {
		if (arg.transform.GetComponent<Item>().item.isStackable)
        {
            AddStackableItem(arg);
        }
        else{
            AddUnstackableItem(arg);
        }
    }
    // Функция* Ссылающая предмет на подфункции (стакающиеся,ондослойные)
	void AddStackableItem(GameObject currentItem)
    {
        foreach (Subject itemer in Inventory)
        {
			if (itemer.id == currentItem.transform.GetComponent<Item>().item.id)
            {
                itemer.countItem += currentItem.transform.GetComponent<Item>().item.countItem;
                ChangeInventory();
                Destroy(currentItem);
                return;
            }
        }
        AddUnstackableItem(currentItem);
    }
    // Функция* блаблабла
	void AddUnstackableItem(GameObject currentItem)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].id == 0)
            {
                Inventory[i] = currentItem.transform.GetComponent<Item>().item;
                ChangeInventory();
                Destroy(currentItem);
                break;
            }
        }
    }
    // Функция* блаблабла
    public void ShowReadMorePanel()
    {
        DeleteModel(false);
        if (Inventory[setId].id != 0)
        {
            ModelPanel.gameObject.SetActive(true);
            DeleteModel(false);
            GameObject OBJECT = Instantiate(Resources.Load(Inventory[setId].pathPrefab) as GameObject);
            OBJECT.tag = "Model";
            if (OBJECT.GetComponent<Rigidbody>() != null)
            {
                OBJECT.GetComponent<Rigidbody>().isKinematic = true;
                OBJECT.GetComponent<Rigidbody>().useGravity = true;
			}
			OBJECT.AddComponent<ViewObjects>();
			OBJECT.transform.rotation = Player.GetChild(0).GetChild(0).GetChild(0).rotation;
			OBJECT.transform.localScale /= 40;
			if (Inventory [setId].brokenPivot) {
				if (Inventory [setId].id == 101) {
					OBJECT.transform.SetParent (transform.GetChild (0).GetChild(0).GetChild (3).GetChild(0));
				} else {
					OBJECT.transform.SetParent (transform.GetChild (0).GetChild(0).GetChild (3));
				}
				OBJECT.transform.localPosition = Vector3.zero;
				OBJECT.GetComponent<ViewObjects> ().ChosenObjRotate = transform.GetChild (0).GetChild(0).GetChild (3);
				OBJECT.GetComponent<ViewObjects> ().baking = false;
			} else {
				OBJECT.transform.SetParent(Player.GetChild(0).GetChild(0));
				OBJECT.GetComponent<ViewObjects> ().ChosenObjRotate = OBJECT.transform;
				OBJECT.GetComponent<ViewObjects> ().baking = false;
				OBJECT.transform.position = Player.GetChild(0).GetChild(0).GetChild(0).position;
			}
            if (OBJECT.transform.GetComponent<BoxCollider>() != null)
            {
                OBJECT.transform.GetComponent<BoxCollider>().enabled = false;
            }
            InfoModel.GetChild(0).GetComponent<Text>().text = Inventory[setId].name;
            InfoModel.GetChild(1).GetComponent<Text>().text = Inventory[setId].category;
            InfoModel.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = "Подробности: " + Inventory[setId].descriptionItem;

        }
        Panel.gameObject.SetActive(false);
    }
    //Функция* Открывающая информационный панель предмета
    public void CloseReadMorePanel()
    {
        ModelPanel.SetActive(false);
        DeleteModel(false);
    }
    //Функция* Закрывающая информационный панель предмета
    public void EquipAndUsePanel()
    {
        DeleteModel(true);
        ScriptNeeding = true;
        ShowInventory();
        if (Inventory[setId].id != 0)
        {
			/*for (int i = 0; i < Inventory.Count; i++){Inventory[i].active = false;}Inventory[setId].active = true;*/
            GameObject ItemModel = Instantiate(Resources.Load(Inventory[setId].pathPrefab) as GameObject);
			if (ItemModel)
            {
                ItemModel.tag = "PickableModel";
                Debug.Log(Inventory[setId].name);
                ItemModel.transform.GetComponent<Item>().item = Inventory[setId];
                if (ItemModel.transform.GetComponent<BoxCollider>() != null)
                {
                    ItemModel.transform.GetComponent<BoxCollider>().enabled = false;
                }
                if (ItemModel.GetComponent<Rigidbody>() != null)
                {
                    ItemModel.GetComponent<Rigidbody>().isKinematic = true;
                    ItemModel.GetComponent<Rigidbody>().useGravity = true;
                }
                //ItemModel.transform.rotation = Player.GetChild(0).GetChild(2).rotation;
				GetComponent<SubjectUse> ().MyModel = ItemModel.transform;
				GetComponent<SubjectUse>().UsingModel = true;
				GetComponent<SubjectUse>().UsingOriginPosition = true;
				GetComponent<SubjectUse> ().DontPickable = !ItemModel.GetComponent<Item>().item.isPickable;
				GetComponent<SubjectUse>().SetComponent(transform.GetChild(0).GetChild(0).GetChild(0), false);
				ItemModel.transform.localPosition = Vector3.zero;
				if (Inventory [setId].id == 103) {
					ItemModel.transform.localRotation = Quaternion.Euler (Vector3.zero);
				} else if (Inventory [setId].id == 201) {
					ItemModel.transform.localRotation = Quaternion.Euler (new Vector3 (-113,122,160));
					ItemModel.transform.localPosition = new Vector3(0.8f,0,0);
				} else if (Inventory [setId].id == 2) {
					ItemModel.transform.localRotation = Quaternion.Euler (new Vector3 (270,270,6));
					ItemModel.transform.localPosition = new Vector3(0,0,0);
				}
                if (Inventory[setId].countItem == 1)
                {
                    Inventory.Remove(Inventory[setId]);
                }
                else
                {
                    Inventory[setId].countItem--;
                }
                ChangeInventory();
            }
            else
            {
                GameInstaller.item.name = "Заблокировано";
                NotificationIns(GameInstaller, true);
                ScriptNeeding = true;
            }
        }
    }
    //Функция* блаблабл
	public void DeleteModel(bool isPickable)
    {
		if (isPickable) {
			GameObject Modeles = GameObject.FindGameObjectWithTag ("PickableModel");
			if (Modeles != null) {
				AddItem (Player.GetComponent<SubjectUse> ().MyModel.gameObject);
				Player.GetComponent<SubjectUse> ().UsingModel = false;
				Destroy (Modeles);
			}
		} else {
			GameObject Modeles = GameObject.FindGameObjectWithTag ("Model");
			if (Modeles != null) {
				Destroy (Modeles);
			}
		}
    }
    //Функция* Удаляющая ненужные обьекты(кеши)
}