using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public Subject item;
}
[System.Serializable]
public class Subject
{
    public int id = 0;
    public string name = "";
    public string category = "";
    public bool isStackable;
    public int countItem = 0;
    public int value = 0;
    public bool active;
    public bool isPickable = false;
    [Multiline(5)]
    public string descriptionItem = "";
    public string pathIcon = "";
    public string pathPrefab = "";
	public bool brokenPivot;
}
