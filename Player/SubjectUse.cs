using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SubjectUse : MonoBehaviour {
    public Transform PickOffset;
    public Transform OriginOffset;
	public int EquipDistance = 5;
	public float throwPower = 10;
	public int grabPower = 10;

	//[HideInInspector]
	public bool UsingModel = false;
	//[HideInInspector]
	public bool UsingOriginPosition = false;
	//[HideInInspector]
	public Transform MyModel;
	//[HideInInspector]
	public bool DontPickable = false;

    Transform newPlace;
	bool GrabingObject = false;
    void FixedUpdate()
    {
		if (GrabingObject) {
			Grabing ();
		}
		if (newPlace != null) {
			Animate (newPlace);
		} else {
			if (UsingModel) {
				if (MyModel) {
                    if (Input.GetMouseButtonDown (0)) {
						if (!DontPickable) {
							if (UsingOriginPosition) {
								Animate (PickOffset);
							} else {
								UsingModel = false;
								MyModel.GetComponent<Rigidbody> ().velocity = transform.forward * throwPower;
								//MyModel.gameObject.tag = "Untagged";
								SetComponent (null, true);
								throwPower = 10;
								grabPower = 10;
								MyModel = null;
								if (GrabingObject) {
									GrabingObject = false;
								}
							}
						} else {
							if (MyModel.GetComponent<Item>().item.id == 102) {
								transform.GetComponent<PlayerInventory>().ChangeTimer(true,1,1.5f);
							}
						}
					} else if (Input.GetMouseButtonDown (1) && !GrabingObject) {
						if (UsingOriginPosition) {
							transform.GetComponent<PlayerInventory> ().AddItem (MyModel.gameObject);
							transform.GetComponent<PlayerInventory> ().NotificationIns (MyModel.GetComponent<Item>(), false);
							Destroy (MyModel);
							MyModel = null;
							UsingModel = false;
						} else {
							Animate (OriginOffset);
						}
					}
                    else
                    {
                        if (!GrabingObject)
                        {
                            RaycastHit pickHit;
                            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                            if (Physics.Raycast(ray, out pickHit, EquipDistance, LayerMask.GetMask("Door")))
                            {
                                if (MyModel.GetComponent<Item>().item.active && Input.GetKeyDown(KeyCode.E))
                                {
                                    if (transform.GetComponent<FirstPersonController>().m_PreviouslyGrounded)
                                    {
                                        if (pickHit.transform.GetComponent<DoorScript>())
                                        {
                                            if (pickHit.transform.GetComponent<DoorScript>())
                                            {
                                                DoorScript script = pickHit.transform.GetComponent<DoorScript>();
                                                if (script.Locked)
                                                {
                                                    script.OpenLockedDoor(MyModel.GetComponent<Item>().item.value);
                                                }
                                                else
                                                {
                                                    script.OpenDoor();
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    
				}
			} else {
			    RaycastHit pickHit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out pickHit, EquipDistance, LayerMask.GetMask("Pickable")))
                {
                    if (Input.GetMouseButtonDown(1))
                    {
						newPlace = null;
						UsingModel = true;
						MyModel = pickHit.transform;
						DontPickable = false;
                        UsingOriginPosition = false;
						GrabingObject = true;
						if (MyModel.gameObject.tag == "Hanger") {
							throwPower = 0;
							grabPower = 3;
						}
						Grabing();
                    }
				}
                else if(Physics.Raycast(ray, out pickHit, EquipDistance, LayerMask.GetMask("Door"))){
					if (Input.GetKeyDown(KeyCode.E))
					{
						if (pickHit.transform.GetComponent<MechanicDoorScript>())
						{
							MechanicDoorScript _script = pickHit.transform.GetComponent<MechanicDoorScript>();
							if (_script.Locked)
							{
								_script.OpenLockedDoor();
							}
							else
							{
								_script.OpenDoor();
							}
						}
					}
				}
			}
		}
    }
    void Animate(Transform pos)
    {
        if(newPlace == null)
        {
            newPlace = pos;
        }
        if (MyModel.position != pos.position)
        {
            MyModel.position = Vector3.Lerp(MyModel.position, pos.position,0.5f);
            MyModel.rotation = Quaternion.Lerp(MyModel.rotation, pos.rotation, 0.6f);
        } else
        {
            if (UsingOriginPosition)
            {
                UsingOriginPosition = false;
            }
            else
            {
                UsingOriginPosition = true;
            }
            newPlace = null;
        }
    }
    public void CheckModelObject()
    {
        MyModel = GameObject.FindGameObjectWithTag("Model").transform;
        if(MyModel != null)
        {
            UsingModel = true;
            UsingOriginPosition = true;
            SetComponent(transform.GetChild(0), false);
        } else
        {
            UsingModel = false;
        }
    }
    public void SetComponent(Transform Elparent,bool ElEnable)
    {
        MyModel.SetParent(Elparent);
        if (ElEnable)
        {
            //MyModel.GetComponent<Rigidbody>().useGravity = true;
            if (MyModel.GetComponent<BoxCollider>())
            {
                MyModel.GetComponent<BoxCollider>().enabled = true;
            } else if (MyModel.GetComponent<CapsuleCollider>())
            {
                MyModel.GetComponent<CapsuleCollider>().enabled = true;
            }
            if (MyModel.GetComponent<Rigidbody>() != null)
            {
                MyModel.GetComponent<Rigidbody>().isKinematic = false;
            }
        } else
        {
            //MyModel.GetComponent<Rigidbody>().useGravity = true;
            if (MyModel.GetComponent<BoxCollider>())
            {
                MyModel.GetComponent<BoxCollider>().enabled = false;
            }
            else if (MyModel.GetComponent<CapsuleCollider>())
            {
                MyModel.GetComponent<CapsuleCollider>().enabled = false;
            }
            if (MyModel.GetComponent<Rigidbody>() != null)
            {
                MyModel.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
    void Grabing()
    {
		if(MyModel.GetComponent<Rigidbody>() && transform.GetChild(0).GetChild(0).eulerAngles.x < 60 || transform.GetChild(0).GetChild(0).eulerAngles.x > 269){
			MyModel.GetComponent<Rigidbody>().velocity = (PickOffset.position - (MyModel.position + MyModel.GetComponent<Rigidbody> ().centerOfMass)) * grabPower;
		} else if(MyModel.GetComponent<Rigidbody>() && transform.GetChild(0).eulerAngles.x > 70){
			if (MyModel.gameObject.tag == "Hanger") {
				UsingModel = false;
				MyModel = null;
				GrabingObject = false;
			} else {
				MyModel.gameObject.tag = "Untagged";
				UsingModel = false;
				MyModel = null;
				GrabingObject = false;
			}
		}
    }
}
// Короче, смотри у нас проблема с UsingOriginPosition нужно подумать. Так что нужно там пыры мыры думать. Седня лень давай работай!!!