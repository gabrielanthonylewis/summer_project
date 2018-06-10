using UnityEngine;
using System.Collections;

public class GetMainCameraEvent : MonoBehaviour {

    public ID.SubMenu subMenu = ID.SubMenu.NULL;

    public void Event(GameObject slot)
    {
        Camera.main.GetComponent<Inventory>().OnUICraftClick(slot);
    }

    public void EventUISubMenuClick(GameObject subMenuSlot)
    {
        Camera.main.GetComponent<Inventory>().OnUISubMenuClick(subMenuSlot, subMenu);
    }
}
