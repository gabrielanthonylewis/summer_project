using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    //Serialised Fields
    [SerializeField]
    private bool scaleSlots = true;
    [SerializeField]
    private int totalRows = 3;
    [SerializeField]
    private int totalColombs = 6;
    // private int rowsBeforeExtend
    // private int columnsBeforeExtend // ammount of columns to be scaled basiaclly so that scroll bar will kick in if more than casn see!

    //Component References
    private InventoryManager _InvManager = null;
    private CameraMovement _CameraMovementA = null;
    private CameraMovement _CameraMovementB = null;
    private Raycast _Raycast = null;
    private Toggle _ShowAllToggle = null;
    private Camera _MainCamera = null;
    private PauseMenu _PauseMenu = null;
    private SubMenuReference _CraftContSubMenuRef = null;

    //Object References
    private GameObject _InventoryUI = null;
    private GameObject _ProximityCheckGO = null;
    private GameObject _CraftOptionPrefab = null;
    private GameObject _SlotPrefab = null;
    private GameObject _ProximityOptionPrefab;
    private GameObject _CraftContent = null;
    private GameObject _SlotContent = null;
    private GameObject _ProximityContent = null;
    private GameObject[] Prefabs;
    
    //Private
    public List<GameObject> ItemHolders = new List<GameObject>();
    private List<CraftSlot> _CraftSlots = new List<CraftSlot>();
    private List<ProximitySlot> _ProximitySlots = new List<ProximitySlot>();
    private Slot[] _Slots;
    private List<ResourceList> _ResourceList = new List<ResourceList>();
    private Color highlightedColor;
    private Color idleColor;



    private float slotRectHeight;
    private float slotRectWidth;

    //Structures
    public struct Item
    {
        public ID.Type type;
        public ID.Specification specification;
        public ID.SubMenu subMenu;
        public GameObject prefab;
        public GameObject go;
        public int quantity; //REMOVE
        private ID _ID;
        public int variation;

        public void SetID(ID id) { _ID = id; }
        public ID GetID() { return _ID; }
    };
    struct CraftSlot
    {
        public int index;
        public bool selected;
        public bool isPreview;
        public Color originalColor;
        public RectTransform rectTransform;
        public List<Item> resources;
        public List<int> resourceQuantity;
        public Item item;
    }

    // INHERIT COMMONSLOT FROM CLASSES SUCH AS PROXIMITY SLOT
    class CommonSlot
    {

    }

    class ProximitySlot
    {
        public Color originalColor;
        public RectTransform rectTransform;
        public Item item;
        public bool occupied;
        public Text textName;

        public void HideAndDefault()
        {
            occupied = false;
            item = new Item();
            textName.text = "";
            rectTransform.transform.gameObject.SetActive(false);
        }
    };
    struct Slot
    {
        public int index;
        public bool occupied;
        public Item item;
        public Color originalColor;
        public int quantity;
        public List<GameObject> otherGO;
        public RectTransform rectTransform;
        public bool primary;
        public Vector2 sizeDelta;
    };
    struct ResourceList
    {
        public Item craftedItem;
        public List<Item> items;
        public List<int> quantities;
    }

    void Awake()
    {
        Prefabs = Resources.LoadAll<GameObject>("Prefabs");

        highlightedColor = new Color(1f, 1f, 1f, 0.75f);
        idleColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

        _InvManager = GameObject.FindGameObjectWithTag("InventoryHolder").GetComponent<InventoryManager>();
        if (_InvManager == null) Debug.LogError("!! INVMANAGER == NULL!!");
        _InventoryUI = _InvManager.gameObject;
        _InventoryUI.SetActive(false);
        _ShowAllToggle = _InvManager.refShowAllToggle;
        _CraftContent = _InvManager.refCraftContent;
        _CraftContSubMenuRef = _CraftContent.GetComponent<SubMenuReference>();
        _SlotContent = _InvManager.refSlotContent;
        _ProximityContent = _InvManager.refProximityContent;
        _ProximityCheckGO = _InvManager.refProximityCheckGO;
        _CraftOptionPrefab = _InvManager.craftOptionPrefab;
        _SlotPrefab = _InvManager.slotPrefab;
        _ProximityOptionPrefab = _InvManager.proximityOptionPrefab;

        _PauseMenu = this.GetComponent<PauseMenu>();

        _MainCamera = Camera.main;

        _CameraMovementA = _MainCamera.transform.parent.GetComponent<CameraMovement>();
        _CameraMovementB = _MainCamera.GetComponent<CameraMovement>();
        _Raycast = _MainCamera.GetComponent<Raycast>();

        if (!InitialiseSlots()) Debug.LogError("Slot initialisation FAILED!");
        if (!InitialiseCraftingResources()) Debug.LogError("Crafting resources initialisation FAILED!");
        if (!InitialiseProximitySlots()) Debug.LogError("ProximitySlot initialisation FAILED!");

        RefreshCraftingMenu();
    }

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_PauseMenu.PauseMenuGO.activeSelf)
            {

                _InventoryUI.SetActive(!_InventoryUI.activeSelf);

                _ProximityCheckGO.SetActive(!_ProximityCheckGO.activeSelf);
                _CameraMovementA.enabled = !_CameraMovementA.enabled; // have a local var to return (in update)
                _CameraMovementB.enabled = !_CameraMovementB.enabled;
                _Raycast.enabled = !_Raycast.enabled;

                Cursor.visible = !Cursor.visible;
                if (_ProximityCheckGO.activeSelf)
                {
                    ResetProximityContent();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GetPrimaryIndex() > -1)
                DropItem(_Slots[GetPrimaryIndex()].item.go, true);
        }


        if (tempSlotGO != null)
            tempSlotGO.transform.position = Input.mousePosition + tempSlotOccupiedSpace;
    }

    public void OnProximitySlotClick(GameObject slot)
    {
        for (int i = 0; i < _ProximitySlots.Count; i++)
        {
            if (_ProximitySlots[i].rectTransform.gameObject == slot)
            {
                if (AddDeleteItem(_ProximitySlots[i].item.specification, _ProximitySlots[i].item.type, _ProximitySlots[i].item.variation, _ProximitySlots[i].item.go.transform, _ProximitySlots[i].item.subMenu, _ProximitySlots[i].item.GetID().Prefab))
                {
                    _ProximitySlots[i].HideAndDefault();
                    RefreshCraftingMenu();
                }

                break;
            }
        }
    }

    public bool RemoveProximityItem(GameObject localGO)
    {
        for (int i = 0; i < _ProximitySlots.Count; i++)
        {
            if (localGO == _ProximitySlots[i].item.go)
            {
                _ProximitySlots[i].HideAndDefault();
                RefreshCraftingMenu();
                return true;
            }

        }
        return false;
    }


    private void ResetProximityContent()
    {
        for (int i = 0; i < _ProximitySlots.Count; i++)
        {
            _ProximitySlots[i].HideAndDefault();
        }
    }


    public void AddProximityItem(ID.Specification spec, ID.Type type, int var, GameObject localGameobject, ID.SubMenu subMenu, GameObject prefab)
    {
        Item tempItem = new Item();

        if (prefab != null)
            tempItem.prefab = prefab;
        else
            tempItem.prefab = GetPrefab(spec, type, var);
        
        tempItem.go = localGameobject;
        tempItem.quantity = 0;
        tempItem.specification = spec;
        tempItem.subMenu = subMenu;
        tempItem.type = type;
        tempItem.variation = 0;
        tempItem.SetID(tempItem.go.GetComponent<ID>());

        ProximitySlot tempProximitySlot = new ProximitySlot();

        int freeIdex = FirstFreeProximityIndex();

        if (freeIdex > -1)
        {
            tempProximitySlot = _ProximitySlots[freeIdex];
            tempProximitySlot.item = tempItem;
            tempProximitySlot.occupied = true;
            _ProximitySlots[freeIdex] = tempProximitySlot;
            _ProximitySlots[freeIdex].rectTransform.transform.GetChild(0).GetComponent<Text>().text = tempItem.prefab.name;
            _ProximitySlots[freeIdex].rectTransform.transform.gameObject.SetActive(true);
        }
        else
            if (freeIdex == -1)
            {
                GameObject newObject = Instantiate(_ProximityOptionPrefab) as GameObject;
                newObject.transform.SetParent(_ProximityContent.transform);
                newObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                newObject.GetComponent<RectTransform>().transform.localScale = new Vector3(1f, 1f, 1f);
                newObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (_ProximityContent.transform.GetComponent<RectTransform>().sizeDelta.y / 2f) - (newObject.GetComponent<RectTransform>().sizeDelta.y * (_ProximitySlots.Count)) - newObject.GetComponent<RectTransform>().sizeDelta.y / 2f - 3f, 0f);

                newObject.SetActive(true);

                tempProximitySlot.item = tempItem;
                tempProximitySlot.occupied = true;
                tempProximitySlot.originalColor = _ProximityOptionPrefab.GetComponent<Image>().color;
                tempProximitySlot.rectTransform = newObject.GetComponent<RectTransform>();

                _ProximitySlots.Add(tempProximitySlot);
            }

        RefreshCraftingMenu();
    }

    private bool InitialiseCraftingResources()
    {
        
        if (Prefabs.Length <= 0)
        {
            Debug.Log("Resource folder 'Prefabs' is empty");
            Prefabs = Resources.LoadAll<GameObject>("Prefabs");
           // return false;
        }

        for (int i = 0; i < Prefabs.Length; i++)
        {
            ID PrefabID = Prefabs[i].GetComponent<ID>();

            if (PrefabID == null) continue;
            if (PrefabID.isBase) continue;

            ResourceList tempRL = new ResourceList();
            tempRL.items = new List<Item>();
            tempRL.quantities = new List<int>();
            tempRL.craftedItem = InitialiseItem(PrefabID.type, PrefabID.specification, PrefabID.variation, null, PrefabID.gameObject, null, 0, PrefabID.subMenu);

            for (int j = 0; j < PrefabID.Resources.Count; j++)
            {
                Item tempItem = InitialiseItem(PrefabID.ResourceID[j].type, PrefabID.ResourceID[j].specification, PrefabID.ResourceID[j].variation, null, PrefabID.Resources[j], null, 0, PrefabID.ResourceID[j].subMenu);

                if (tempRL.items.Contains(tempItem))
                {


                    tempRL.quantities[tempRL.items.IndexOf(tempItem)] += 1;
                }
                else
                {
                    tempRL.items.Add(tempItem);
                    tempRL.quantities.Add(1);
                }
            }
            _ResourceList.Add(tempRL);

        }

        return true;
    }


    #region CLEANED - Wrapped Functions
    private Item InitialiseItem(ID.Type type, ID.Specification specification, int variation, ID id, GameObject prefab, GameObject go, int quantity, ID.SubMenu subMenu)
    {
        Item tempItem = new Item();
        tempItem.type = type;
        tempItem.specification = specification;
        tempItem.subMenu = subMenu;
        tempItem.variation = variation;
        tempItem.SetID(id);
        tempItem.prefab = prefab;
        tempItem.go = go;
        tempItem.quantity = quantity;
        return tempItem;
    }
    #endregion

    public void RefreshCraftingMenu()
    {
        //Debug.LogError("REFRESHCRAFTINGMENU");

        // WORK IN PROGRESS, WANT TO GET "REMOVAL" INTO IT
        /*
        for (int i = 1; i < _CraftContSubMenuRef.SubMenusContent.Count; i++ )
        {
            bool good = false;
           
            for (int j = 0; j < _CraftContSubMenuRef.SubMenusContent[i].childCount; j++)
            {

                for (int k = 0; k < _ResourceList[i - 1 + j].items.Count; k++)
                {
                    if (GetTotalQuantity(_ResourceList[i - 1 + j].items[k].specification, _ResourceList[i - 1 + j].items[k].type, _ResourceList[i - 1 + j].items[k].variation) >= _ResourceList[i - 1 + j].quantities[k])
                    {
                        Debug.LogError("GOOD: " + _ResourceList[i - 1 + j].items[k].specification);
                        good = true;
                        break;
                    }
                }

                if (good)
                    break;
              
             
            }

            _CraftContSubMenuRef.SubMenusButtons[i].GetChild(1).GetComponent<Image>().enabled = good;

        }*/

        #region removal
        for (int i = 0; i < _ResourceList.Count; i++)
        {
            // identify craft slot
            int tempCraftSlotIndex = -1;
            for (int j = 0; j < _CraftSlots.Count; j++)
            {
                if (_CraftSlots[j].item.specification == _ResourceList[i].craftedItem.specification
                    && _CraftSlots[j].item.type == _ResourceList[i].craftedItem.type
                    && _CraftSlots[j].item.variation == _ResourceList[i].craftedItem.variation)
                {
                    tempCraftSlotIndex = j;
                }
            }


            bool good = true;
            for (int j = 0; j < _ResourceList[i].items.Count; j++)
            {
                if (GetTotalQuantity(_ResourceList[i].items[j].specification, _ResourceList[i].items[j].type, _ResourceList[i].items[j].variation) < _ResourceList[i].quantities[j])
                {

                    good = false;
                    break;
                }
            }
            if (!good)
            {

                if (_ShowAllToggle.isOn == false)
                {
                    if (tempCraftSlotIndex > -1)
                        _CraftSlots[tempCraftSlotIndex].rectTransform.gameObject.SetActive(false);
                    continue;
                }
            }


            if (tempCraftSlotIndex == -1)
            {
                GameObject newObject = Instantiate(_CraftOptionPrefab) as GameObject;

                // newObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                // newObject.GetComponent<RectTransform>().transform.localScale = new Vector3(1f, 1f, 1f);
                newObject.SetActive(true);

                Item item = new Item();
                item.specification = _ResourceList[i].craftedItem.specification;
                item.subMenu = _ResourceList[i].craftedItem.subMenu;
                item.type = _ResourceList[i].craftedItem.type;
                item.variation = _ResourceList[i].craftedItem.variation;

                if (item.GetID() != null && item.GetID().Prefab != null)
                    item.prefab = item.GetID().Prefab;
                else
                    item.prefab = GetPrefab(item.specification, item.type, item.variation);


                CraftSlot craftSlot = new CraftSlot();
                craftSlot.index = _CraftSlots.Count;
                craftSlot.originalColor = _CraftOptionPrefab.GetComponent<Image>().color;
                craftSlot.rectTransform = newObject.GetComponent<RectTransform>();
                craftSlot.selected = false;
                craftSlot.resources = new List<Item>();
                craftSlot.resourceQuantity = new List<int>();
                craftSlot.item = item;

                if (!good)
                {
                    craftSlot.rectTransform.GetComponent<Image>().color = Color.clear;
                    // craftSlot.originalColor = Color.clear;
                    craftSlot.isPreview = true;
                }

                newObject.transform.GetChild(0).GetComponent<Text>().text = "Resources:";

                for (int j = 0; j < _ResourceList[i].items.Count; j++)
                {
                    Item resourceItem1 = new Item();
                    resourceItem1.specification = _ResourceList[i].items[j].specification;

                    if (_ResourceList[i].items[j].go)
                    resourceItem1.SetID(_ResourceList[i].items[j].go.GetComponent<ID>());
                    resourceItem1.subMenu = _ResourceList[i].items[j].subMenu;
                    resourceItem1.type = _ResourceList[i].items[j].type;
                    resourceItem1.variation = _ResourceList[i].items[j].variation;

                    if (resourceItem1.GetID() != null && resourceItem1.GetID().Prefab != null)
                    {
                        resourceItem1.prefab = resourceItem1.GetID().Prefab;
                    }
                    else
                    resourceItem1.prefab = GetPrefab(resourceItem1.specification, resourceItem1.type, resourceItem1.variation);

                    int quantity1 = _ResourceList[i].quantities[j];

                    craftSlot.resources.Add(resourceItem1);
                    craftSlot.resourceQuantity.Add(quantity1);

                    newObject.transform.GetChild(0).GetComponent<Text>().text += "     " + resourceItem1.prefab.name + "(" + quantity1 + ")";
                }


                newObject.transform.GetChild(1).GetComponent<Text>().text = craftSlot.item.prefab.name;

                newObject.transform.SetParent(_CraftContSubMenuRef.SubMenusContent[(int)item.subMenu]);
                craftSlot.rectTransform.localPosition = Vector3.zero;
                craftSlot.rectTransform.transform.localScale = Vector3.one;
                //newObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                //newObject.GetComponent<RectTransform>().transform.localScale = new Vector3(1f, 1f, 1f);

                _CraftSlots.Add(craftSlot);

                // Set size for content boxes.
                for (int z = 0; z < _CraftContSubMenuRef.SubMenusContent.Count; z++)
                {

                    if (_CraftContSubMenuRef.SubMenusContent[z])
                    {
                        _CraftContSubMenuRef.SubMenusContent[z].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, (_CraftContSubMenuRef.SubMenusContent[z].childCount + 1f) * _CraftOptionPrefab.GetComponent<RectTransform>().sizeDelta.y);

                        float newy = -(_CraftContSubMenuRef.SubMenusContent[z].GetComponent<RectTransform>().sizeDelta.y / 2f);
                        _CraftContSubMenuRef.SubMenusContent[z].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, newy);
                    }
                }

                for (int z = 1; z < _CraftContSubMenuRef.SubMenusContent.Count; z++)
                {
                    for (int j = 0; j < _CraftContSubMenuRef.SubMenusContent[z].childCount; j++)
                    {
                        for (int slotIndex = 0; slotIndex < _CraftSlots.Count; slotIndex++)
                        {
                            if (_CraftSlots[slotIndex].rectTransform == _CraftContSubMenuRef.SubMenusContent[z].GetChild(j))
                            {
                                _CraftSlots[slotIndex].rectTransform.localPosition = new Vector3(0f, (_CraftContSubMenuRef.SubMenusContent[z].GetComponent<RectTransform>().sizeDelta.y / 2f) - 30f - (_CraftSlots[slotIndex].rectTransform.sizeDelta.y * (j)), 0f);
                            }
                        }
                    }
                }

            }
            else
            {

                CraftSlot craftSlot = _CraftSlots[tempCraftSlotIndex];



                if (!good)
                {
                    // Debug.Log("CLEAR: " + craftSlot.item.specification);
                    craftSlot.rectTransform.GetComponent<Image>().color = Color.clear;
                    //craftSlot.originalColor = Color.clear;

                    if (craftSlot.isPreview == false)
                    {
                        craftSlot.isPreview = true;
                        //  _CraftContSubMenuRef.SubMenusButtons[(int)craftSlot.item.subMenu].GetChild(1).GetComponent<Image>().enabled = false;
                    }


                }
                else
                {
                    //Debug.Log("ORIG: " + craftSlot.item.specification);
                    craftSlot.rectTransform.GetComponent<Image>().color = craftSlot.originalColor;


                    if (craftSlot.isPreview == true)
                    {
                        craftSlot.isPreview = false;
                        _CraftContSubMenuRef.SubMenusButtons[(int)craftSlot.item.subMenu].GetChild(1).GetComponent<Image>().enabled = true;
                    }
                }

                _CraftSlots[tempCraftSlotIndex] = craftSlot;
                _CraftSlots[tempCraftSlotIndex].rectTransform.gameObject.SetActive(true);

                tempCraftSlotIndex = -1;
            }
        }
        #endregion

    }

    public void OnUICraftBackButtonClick()
    {
        for (int i = 0; i < _CraftContSubMenuRef.SubMenusContent.Count; i++)
        {
            if (_CraftContSubMenuRef.SubMenusContent[i] != null)
                _CraftContSubMenuRef.SubMenusContent[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _CraftContSubMenuRef.SubMenusButtons.Count; i++)
        {
            if (_CraftContSubMenuRef.SubMenusButtons[i] != null)
                _CraftContSubMenuRef.SubMenusButtons[i].gameObject.SetActive(true);
        }
    }

    public void OnUICraftButtonClick()
    {
        //Debug.Log("true)_");
        GameObject selected = GetSelectedCraftSlotGO();
        if (selected == null) return;
        //Debug.Log("t342423_");
        int index = -1;
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].rectTransform.gameObject == selected)
            {
                index = i;
                continue;
            }
        }

        if (index == -1)
        {
            Debug.LogError("INDEX == -1");
            return;
        }
        if (_CraftSlots[index].isPreview) return;


        List<int> tempResourceQU = new List<int>();


        List<int> removeSlotsAtIndex = new List<int>();

        for (int i = 0; i < _CraftSlots[index].resourceQuantity.Count; i++)
        {

            //Debug.Log(_CraftSlots[index].resources[i].specification + "::resourceQuant: " + _CraftSlots[index].resourceQuantity[i]);
            List<int> tempList = GetIndexesOfGameobject(_CraftSlots[index].resources[i].specification, _CraftSlots[index].resources[i].type, _CraftSlots[index].resources[i].variation, _CraftSlots[index].resourceQuantity[i]);
            tempResourceQU.Add(_CraftSlots[index].resourceQuantity[i]);


            removeSlotsAtIndex.Clear();
            for (int j = 0; j < tempList.Count; j++)
            {

                removeSlotsAtIndex.Add(tempList[j]);
            }

            for (int j = 0; j < removeSlotsAtIndex.Count; j++)
            {

                RemoveItemtAtSlotIndex(removeSlotsAtIndex[j], 1, true);

                tempResourceQU[i]--;

            }

            // remove here?
        }

        //EERRRRROR HERE?
        //WHEN CRAFTING SOMETIME IT SAYS INDEX WRONG!
        List<int> removeProximitySlotsAtIndex = new List<int>();

        // Debug.Log("_CraftSlots[index].resources.Count: " + _CraftSlots[index].resources.Count);
        for (int i = 0; i < _CraftSlots[index].resources.Count; i++)
        {
            // e.g. Stone
            if (tempResourceQU[i] == 0) continue;

            // Get index of a Stone
            List<int> tempList = GetIndexesOfGameobjectProximity(_CraftSlots[index].resources[i].specification, _CraftSlots[index].resources[i].type, _CraftSlots[index].resources[i].variation, _CraftSlots[index].resourceQuantity[i]);

            for (int j = 0; j < tempList.Count; j++)
            {
                Debug.Log(tempList[j]);
                // Add index of STONE to  Remove  Proximity slot 
                removeProximitySlotsAtIndex.Add(tempList[j]);
            }

            List<ProximitySlot> tempProxSlots = new List<ProximitySlot>();
            for (int j = 0; j < _ProximitySlots.Count; j++)
            {
                if (!removeProximitySlotsAtIndex.Contains(j)) continue;

                // Add actual proximitySlot of STONE 
                tempProxSlots.Add(_ProximitySlots[j]);

            }
            /*
           for (int j = 0; j < removeProximitySlotsAtIndex.Count; j++)
           {
               RemoveItemtAtSlotIndexProximity(removeProximitySlotsAtIndex[j], 1, true);
           }*/

            for (int j = 0; j < tempProxSlots.Count; j++)
            {
                // Debug.LogError("PRE: " + (tempProxSlots.Count - j - 1).ToString() + " :: .Count " + tempProxSlots.Count + " :: j " + j);
                RemoveItemtAtSlotIndexProximity(removeProximitySlotsAtIndex[tempProxSlots.Count - j - 1], 1, true);

                //Debug.LogError("tempResCount: " + tempResourceQU.Count);
                tempResourceQU[i]--;


                //  Debug.LogError("GOOD: GOT HERE");

            }

            //new
            removeProximitySlotsAtIndex.Clear();
            tempProxSlots.Clear();
        }
        // HERE
        //  Debug.LogError("GOODdsdds HERE");





        ID.Specification _spec = _CraftSlots[index].item.specification;
        ID.Type _type = _CraftSlots[index].item.type;
        int _vari = _CraftSlots[index].item.variation;
        ID.SubMenu _subMenu = _CraftSlots[index].item.subMenu;


        GameObject _prefab = null;
        
        if(_CraftSlots[index].item.GetID() != null && _CraftSlots[index].item.GetID().Prefab != null)
        {
            _prefab = _CraftSlots[index].item.GetID().Prefab;
        }





        // do the whole if!() instantaite object() thing here.
        if (!AddItem(_spec, _type, _vari, null, _subMenu, _prefab))
        {
            if (InstantiateToWorld(_spec, _type, _vari, _prefab) == null)
                Debug.LogError("Instantiation Failed");
        }

        RefreshCraftingMenu();
    }

    private GameObject InstantiateToWorld(ID.Specification spec, ID.Type type, int variation, GameObject _prefab)
    {
        Debug.Log("callled, change to fix");
        GameObject prefab = _prefab;
        
        if(_prefab == null)
            prefab = GetPrefab(spec, type, variation);
        
        if (prefab == null) return null;

        GameObject newObject = Instantiate(prefab, this.transform.position + transform.forward, this.transform.rotation) as GameObject;

        if (newObject.GetComponent<Rigidbody>())
        {
            newObject.GetComponent<Rigidbody>().isKinematic = false;
            newObject.GetComponent<Rigidbody>().useGravity = true;
            newObject.GetComponent<Rigidbody>().AddForce(this.transform.forward * 4f, ForceMode.Impulse);
        }

        for (int j = 0; j < newObject.GetComponent<ID>().Colliders.Length; j++)
        {
            newObject.GetComponent<ID>().Colliders[j].enabled = true;
        }

        if (newObject.GetComponent<Consumable>())
            newObject.GetComponent<Consumable>().enabled = false;

        newObject.layer = 8;
        newObject.name = prefab.name;

        return newObject;
    }

    public int IndexOfItemObject(GameObject itemObj)
    {
        int tempIndex = -1;

        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.go == itemObj)
            {
                return i;
            }
        }


        return tempIndex;
    }

    public bool SetDurAtSlotIndex(int _idx, float val)
    {
        if (_idx <= -1) return false;
        if (val >= 100f)
        {
            Debug.Log("RESET");
            _Slots[_idx].rectTransform.GetComponent<Parts>().dur.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, -100f);

            _Slots[_idx].rectTransform.GetComponent<Parts>().dur.anchoredPosition = new Vector2(0f, 0f);
            return true;
        }

        float _valOfHundredth = _Slots[_idx].sizeDelta.y / 100f;




        //float _curSize = _Slots[_idx].rectTransform.GetComponent<Parts>().dur.sizeDelta.y;

        Debug.Log("NEWSET: " + (val * _valOfHundredth).ToString());
        _Slots[_idx].rectTransform.GetComponent<Parts>().dur.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, (val * _valOfHundredth));

        float newy = +((val * _valOfHundredth) / 2f);
        // Debug.Log(newy.ToString());
        _Slots[_idx].rectTransform.GetComponent<Parts>().dur.anchoredPosition = new Vector2(0f, newy);



        return true;

    }


    #region UPDATED33333
    GameObject tempSlotGO = null;
    Vector3 tempSlotOccupiedSpace = Vector3.zero;
    Slot tempSlot = new Slot();
    int originalSlotIndex = -1;
    public void OnUISlotButtonClickRight(GameObject slot)
    {

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            int index2 = ItemHolders.IndexOf(slot);

            if (tempSlotGO == null)
            {
                if (_Slots[index2].item.go == null) return;

                tempSlot = _Slots[index2]; // DOES THIS COPY AS REFERENCE AND NOT VALUE? EDIT: LOOKS LIKE IT DOES BUT IM WORKING WITH IT FINE NOW, I GUESS IT NEEDS TO BE CHANGED HOWEVER


                tempSlotGO = Instantiate(tempSlot.rectTransform.gameObject) as GameObject;
                tempSlotGO.transform.SetParent(_SlotContent.transform);
                tempSlotGO.transform.position = Input.mousePosition;
                tempSlotOccupiedSpace = new Vector3((tempSlotGO.GetComponent<RectTransform>().sizeDelta.x / 2f) + 10f, -(tempSlotGO.GetComponent<RectTransform>().sizeDelta.y / 2f), 0f);

                originalSlotIndex = index2;

                _Slots[index2].item.go.SetActive(false);

                // NEW
                for (int i = 0; i < _Slots[index2].otherGO.Count; i++)
                {
                    _Slots[index2].otherGO[i].SetActive(false);

                }

                RemoveItemtAtSlotIndex(index2, _Slots[index2].quantity, false);

                // JUST ADDED
                //  _Slots[index2].otherGO.Clear();


            }
            else
            {

                //Debug.Log("ADD TO!----------   " + _Slots[index2].item.specification + " :::::::::" + tempSlot.item.specification);
                if (_Slots[index2].item.specification == tempSlot.item.specification
                    && _Slots[index2].item.type == tempSlot.item.type
                    && _Slots[index2].item.variation == tempSlot.item.variation)
                {

                    //Debug.Log("ADD TO!000: " + (_Slots[index2].quantity + tempSlot.quantity).ToString() + " ::MAX:: " + _Slots[index2].item.go.GetComponent<ID>().maxStack);
                    if ((_Slots[index2].quantity + tempSlot.quantity) <= _Slots[index2].item.go.GetComponent<ID>().maxStack)
                    {
                        Debug.Log("ADD TO!");

                        _Slots[index2].quantity += tempSlot.quantity;
                        _Slots[index2].rectTransform.GetChild(1).GetComponent<Text>().text = _Slots[index2].quantity.ToString();


                        _Slots[index2].otherGO.Add(tempSlot.item.go);


                        //  _Slots[originalSlotIndex] = new Slot();




                        Destroy(tempSlotGO);
                        tempSlotGO = null;
                        tempSlotOccupiedSpace = Vector3.zero;
                        tempSlot = new Slot();
                        RefreshCraftingMenu();
                        return;
                    }
                }


                // check if slots are free.


                int standardColumn = 0;
                for (int i = 0; i < _Slots.Length; i++)
                {

                    if (standardColumn > (totalColombs - 1)) standardColumn = 0;
                    if (i == index2) break;
                    standardColumn++;
                }
                // Debug.Log("SC: " + standardColumn);
                if (standardColumn + tempSlot.item.prefab.GetComponent<ID>().slotWidth > totalColombs) // no -1 before
                {
                    //Debug.LogError("OVERFLOW");
                    return;
                }

                /*
                if (standardColumn + tempSlot.item.prefab.GetComponent<ID>().slotWidth > totalColombs - 1)
                {
                    Debug.LogError("OCCUPIED000");
                    return;
                }
            
            if (index2 + (totalColombs * tempSlot.item.prefab.GetComponent<ID>().slotHeight) > totalRows)
            {
                Debug.LogError("OCCUPIED111");
                return;
            }*/

                // set back to orginal pos.

                for (int row = 0; row < tempSlot.item.prefab.GetComponent<ID>().slotHeight; row++)
                {
                    for (int i = 0; i < tempSlot.item.prefab.GetComponent<ID>().slotWidth; i++)
                    {
                        //Debug.Log((index2 + (totalColombs * row) + i).ToString());
                        if ((index2 + (totalColombs * row) + i) > _Slots.Length - 1)
                        {
                            return;
                        }

                        if (_Slots[index2 + (totalColombs * row) + i].occupied == true)
                        {
                            //Debug.LogError("OCCUPIED!!!!");
                            Debug.Log("REPLACE!");

                            // STAGE 0 - INITIALISATION
                            Slot slotA = tempSlot;
                            Slot slotB = _Slots[index2];
                            slotA.index = originalSlotIndex;

                            if (slotB.item.GetID() && slotB.item.GetID().Prefab != null)
                                slotB.item.prefab = slotB.item.GetID().Prefab;
                            else
                                slotB.item.prefab = GetPrefab(slotB.item.specification, slotB.item.type, slotB.item.variation);

                            int aIndex = originalSlotIndex;
                            int bIndex = index2;
                            int aWidth = slotA.item.prefab.GetComponent<ID>().slotWidth;
                            int aHeight = slotA.item.prefab.GetComponent<ID>().slotHeight;
                            int bWidth = slotB.item.prefab.GetComponent<ID>().slotWidth;
                            int bHeight = slotB.item.prefab.GetComponent<ID>().slotHeight;

                            Slot[] slotsCopy = _Slots.Clone() as Slot[];

                            // STAGE 1 - REMOVE A & B
                            for (int ah = 0; ah < aHeight; ah++)
                            {
                                for (int aw = 0; aw < aWidth; aw++)
                                {
                                    slotsCopy[aIndex + (ah * totalColombs) + aw] = new Slot();
                                }
                            }
                            for (int bh = 0; bh < bHeight; bh++)
                            {
                                for (int bw = 0; bw < bWidth; bw++)
                                {
                                    slotsCopy[bIndex + (bh * totalColombs) + bw] = new Slot();
                                }
                            }

                            // STAGE 2 - PROJECTION TESTING
                            // Project A
                            for (int ah = 0; ah < aHeight; ah++)
                            {
                                for (int aw = 0; aw < aWidth; aw++)
                                {
                                    if (slotsCopy[bIndex + (ah * totalColombs) + aw].occupied)
                                    {
                                        //Debug.Log("STAGE2a: slot " + (bIndex + (ah * totalColombs) + aw).ToString() + " occupied!");
                                        return;
                                    }
                                    slotsCopy[bIndex + (ah * totalColombs) + aw].occupied = true;
                                }
                            }
                            // Project B
                            for (int bh = 0; bh < bHeight; bh++)
                            {
                                for (int bw = 0; bw < bWidth; bw++)
                                {
                                    if (slotsCopy[aIndex + (bh * totalColombs) + bw].occupied)
                                    {
                                        // Debug.Log("STAGE2b: slot " + (aIndex + (bh * totalColombs) + bw).ToString() + " occupied!");
                                        return;
                                    }
                                    slotsCopy[aIndex + (bh * totalColombs) + bw].occupied = true;
                                }
                            }

                            //STAGE 4 - SUCCESS
                            //Debug.Log("STAGE4: PASS");

                            #region replace
                            // STAGE 5 - PHYSICALLY SWAP
                            _Slots[originalSlotIndex].item = slotB.item;
                            _Slots[originalSlotIndex].quantity = slotB.quantity;
                            _Slots[originalSlotIndex].otherGO = slotB.otherGO;
                            _Slots[originalSlotIndex].rectTransform.GetComponent<RawImage>().color = idleColor;
                            _Slots[originalSlotIndex].rectTransform.GetChild(0).GetComponent<Text>().text = slotB.item.specification.ToString();
                            _Slots[originalSlotIndex].rectTransform.GetChild(1).GetComponent<Text>().text = slotB.quantity.ToString();

                            _Slots[originalSlotIndex].rectTransform.sizeDelta = slotB.rectTransform.sizeDelta;

                            if (_Slots[originalSlotIndex].item.go.GetComponent<Durabillity>())
                                SetDurAtSlotIndex(index2, _Slots[originalSlotIndex].item.go.GetComponent<Durabillity>().totalDmgTaken);

                            _Slots[originalSlotIndex].rectTransform.SetAsLastSibling();

                            // IF SLOTB IS SAME SIZE AS A DOBNT MOVE? ////

                            if (slotB.item.prefab.GetComponent<ID>().slotWidth > 1)
                            {

                                Vector3 newPos = _Slots[originalSlotIndex].rectTransform.localPosition;
                                Debug.Log("ORIGINAL original: " + newPos);
                                newPos.x += ((_SlotPrefab.GetComponent<RectTransform>().rect.width / 2f) * (slotB.item.prefab.GetComponent<ID>().slotWidth - 1));
                                _Slots[originalSlotIndex].rectTransform.localPosition = newPos;
                                Debug.Log("MOVE ORIGINAL X: " + (slotB.item.prefab.GetComponent<ID>().slotWidth - 1));
                            }
                            if (slotB.item.prefab.GetComponent<ID>().slotHeight > 1)
                            {
                                Vector3 newPos = _Slots[originalSlotIndex].rectTransform.localPosition;
                                newPos.y -= ((_SlotPrefab.GetComponent<RectTransform>().rect.height / 2f) * (slotB.item.prefab.GetComponent<ID>().slotHeight - 1));
                                _Slots[originalSlotIndex].rectTransform.localPosition = newPos;
                            }

                            for (int k = 0; k < slotB.item.prefab.GetComponent<ID>().slotHeight; k++)
                            {
                                for (int l = 0; l < slotB.item.prefab.GetComponent<ID>().slotWidth; l++)
                                {
                                    _Slots[originalSlotIndex + (k * totalColombs) + l].occupied = true;

                                    if ((originalSlotIndex + (k * totalColombs) + l) > originalSlotIndex)
                                        ItemHolders[originalSlotIndex + (k * totalColombs) + l].SetActive(false);
                                }
                            }
                            RemoveItemtAtSlotIndex(bIndex, _Slots[bIndex].quantity, false);


                            _Slots[index2].occupied = true;
                            _Slots[index2].otherGO = slotA.otherGO;
                            _Slots[index2].item = slotA.item;
                            _Slots[index2].quantity = slotA.quantity;
                            _Slots[index2].rectTransform.GetComponent<RawImage>().color = idleColor;
                            _Slots[index2].rectTransform.GetChild(0).GetComponent<Text>().text = slotA.item.specification.ToString();
                            _Slots[index2].rectTransform.GetChild(1).GetComponent<Text>().text = slotA.quantity.ToString();

                            Rect _tempRect2 = _Slots[index2].rectTransform.rect;
                            _tempRect2.width = _SlotPrefab.GetComponent<RectTransform>().rect.width + _SlotPrefab.GetComponent<RectTransform>().rect.width * (_Slots[index2].item.prefab.GetComponent<ID>().slotWidth - 1);
                            _tempRect2.height = _SlotPrefab.GetComponent<RectTransform>().rect.height + _SlotPrefab.GetComponent<RectTransform>().rect.height * (_Slots[index2].item.prefab.GetComponent<ID>().slotHeight - 1);

                            _Slots[index2].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _tempRect2.width);
                            _Slots[index2].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _tempRect2.height);
                            _Slots[index2].sizeDelta = _Slots[index2].rectTransform.sizeDelta;

                            if (_Slots[index2].item.go.GetComponent<Durabillity>())
                                SetDurAtSlotIndex(index2, _Slots[index2].item.go.GetComponent<Durabillity>().totalDmgTaken);

                            _Slots[index2].rectTransform.SetAsLastSibling();


                            if (slotA.item.prefab.GetComponent<ID>().slotWidth > 1)
                            {

                                Vector3 newPos = _Slots[index2].rectTransform.localPosition;
                                Debug.Log("hitslot original: " + newPos);
                                newPos.x += ((_SlotPrefab.GetComponent<RectTransform>().rect.width / 2f) * (slotA.item.prefab.GetComponent<ID>().slotWidth - 1));
                                _Slots[index2].rectTransform.localPosition = newPos;
                                Debug.Log("MOVE HITSLOT X: " + (slotA.item.prefab.GetComponent<ID>().slotWidth - 1));
                            }
                            if (slotA.item.prefab.GetComponent<ID>().slotHeight > 1)
                            {
                                Vector3 newPos = _Slots[index2].rectTransform.localPosition;
                                newPos.y -= ((_SlotPrefab.GetComponent<RectTransform>().rect.height / 2f) * (slotA.item.prefab.GetComponent<ID>().slotHeight - 1));
                                _Slots[index2].rectTransform.localPosition = newPos;
                            }

                            for (int k = 0; k < slotA.item.prefab.GetComponent<ID>().slotHeight; k++)
                            {
                                for (int l = 0; l < slotA.item.prefab.GetComponent<ID>().slotWidth; l++)
                                {
                                    _Slots[index2 + (k * totalColombs) + l].occupied = true;

                                    if ((index2 + (k * totalColombs) + l) > index2)
                                        ItemHolders[index2 + (k * totalColombs) + l].SetActive(false);
                                }
                            }


                            Destroy(tempSlotGO);
                            tempSlotGO = null;
                            tempSlotOccupiedSpace = Vector3.zero;
                            tempSlot = new Slot();

                            RefreshCraftingMenu();
                            return;
                            #endregion

                        }

                    }
                }

                Debug.Log("MOVE!!");

                for (int i = 0; i < tempSlot.item.prefab.GetComponent<ID>().slotHeight; i++)
                {
                    for (int j = 0; j < tempSlot.item.prefab.GetComponent<ID>().slotWidth; j++)
                    {
                        //Debug.Log("::::::::::::::::::::::::::::::::: " + (index + (i * totalColombs) + j));
                        _Slots[index2 + (i * totalColombs) + j].occupied = true; // total row?

                        if ((index2 + (i * totalColombs) + j) > index2)
                            ItemHolders[index2 + (i * totalColombs) + j].SetActive(false);
                    }
                }

                _Slots[index2].occupied = true;
                _Slots[index2].otherGO = tempSlot.otherGO;
                // Debug.LogError("IODSJFDJF::: " + _Slots[index2].otherGO.Count + " dddd " + tempSlot.otherGO.Count);
                _Slots[index2].item = tempSlot.item;

                _Slots[index2].quantity = tempSlot.quantity;
                _Slots[index2].rectTransform.GetComponent<RawImage>().color = idleColor;
                _Slots[index2].rectTransform.GetChild(0).GetComponent<Text>().text = tempSlot.item.specification.ToString();
                _Slots[index2].rectTransform.GetChild(1).GetComponent<Text>().text = _Slots[index2].quantity.ToString();

                Rect _tempRect = _Slots[index2].rectTransform.rect;
                _tempRect.width = _SlotPrefab.GetComponent<RectTransform>().rect.width + _SlotPrefab.GetComponent<RectTransform>().rect.width * (_Slots[index2].item.prefab.GetComponent<ID>().slotWidth - 1);
                _tempRect.height = _SlotPrefab.GetComponent<RectTransform>().rect.height + _SlotPrefab.GetComponent<RectTransform>().rect.height * (_Slots[index2].item.prefab.GetComponent<ID>().slotHeight - 1);

                _Slots[index2].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _tempRect.width);
                _Slots[index2].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _tempRect.height);
                _Slots[index2].sizeDelta = _Slots[index2].rectTransform.sizeDelta;

                if (_Slots[index2].item.go.GetComponent<Durabillity>())
                    SetDurAtSlotIndex(index2, _Slots[index2].item.go.GetComponent<Durabillity>().totalDmgTaken);
                // _Slots[index2].rectTransform.GetComponent<Parts>().dur.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
                // _Slots[index2].rectTransform.GetComponent<Parts>().dur.anchoredPosition = tempSlot.rectTransform.

                _Slots[index2].rectTransform.SetAsLastSibling();

                if (_Slots[index2].item.prefab.GetComponent<ID>().slotWidth > 1)
                {
                    Vector3 newPos = _Slots[index2].rectTransform.localPosition;
                    newPos.x += ((_SlotPrefab.GetComponent<RectTransform>().rect.width / 2f) * (_Slots[index2].item.prefab.GetComponent<ID>().slotWidth - 1));
                    _Slots[index2].rectTransform.localPosition = newPos;
                }
                if (_Slots[index2].item.prefab.GetComponent<ID>().slotHeight > 1)
                {
                    Vector3 newPos = _Slots[index2].rectTransform.localPosition;
                    newPos.y -= ((_SlotPrefab.GetComponent<RectTransform>().rect.height / 2f) * (_Slots[index2].item.prefab.GetComponent<ID>().slotHeight - 1));
                    _Slots[index2].rectTransform.localPosition = newPos;
                }

                // RESET ORIGIONAL ?

                //NEWWWWWWW
                // NVM DID NOTH
                /*
                _Slots[originalSlotIndex].otherGO.Clear();
                _Slots[originalSlotIndex].quantity = 0;
                _Slots[originalSlotIndex].item = new Item();
                _Slots[originalSlotIndex].occupied = false;
                _Slots[originalSlotIndex].primary = false;
                */
                Destroy(tempSlotGO);
                tempSlotGO = null;
                tempSlotOccupiedSpace = Vector3.zero;
                tempSlot = new Slot();

                // _Slots[originalSlotIndex] = new Slot();

                RefreshCraftingMenu();
            }
        }

        if (!Input.GetKey(KeyCode.Mouse1)) return;

        int index = ItemHolders.IndexOf(slot);
        if (_Slots[index].item.go == null) return;
        if (_Slots[index].item.GetID().isBase) return;

        List<GameObject> lclResources = _Slots[index].item.GetID().Resources;

        // Disemble

        //remove
        RemoveItemtAtSlotIndex(index, _Slots[index].quantity, true);

        for (int i = 0; i < lclResources.Count; i++)
        {
            if (!AddItem(lclResources[i].GetComponent<ID>().specification, lclResources[i].GetComponent<ID>().type, lclResources[i].GetComponent<ID>().variation, null, lclResources[i].GetComponent<ID>().subMenu, lclResources[i].GetComponent<ID>().Prefab))
            {
                if (InstantiateToWorld(lclResources[i].GetComponent<ID>().specification, lclResources[i].GetComponent<ID>().type, lclResources[i].GetComponent<ID>().variation, lclResources[i].GetComponent<ID>().Prefab) == null)
                    Debug.LogError("Instantiation Failed");


            }
        }
        //new does fixe>
        RefreshCraftingMenu();

    }

    public void OnUISubMenuClick(GameObject subMenuSlot, ID.SubMenu subMenu)
    {
        bool activeOn = !_CraftContSubMenuRef.SubMenusContent[(int)subMenu].gameObject.activeSelf;

        _CraftContSubMenuRef.SubMenusContent[(int)subMenu].gameObject.SetActive(activeOn);

        for (int i = 0; i < _CraftContSubMenuRef.SubMenusButtons.Count; i++)
        {
            if (_CraftContSubMenuRef.SubMenusButtons[i] != null)
                _CraftContSubMenuRef.SubMenusButtons[i].gameObject.SetActive(!activeOn);
        }
    }

    public void OnUICraftClick(GameObject slot)
    {
        //Debug.Log("PRESSED!");
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].rectTransform.gameObject == slot)
            {
                if (_CraftSlots[i].isPreview) return;
            }

        }
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].selected == true)
            {
                //NEW
                // fixed

                if (_CraftSlots[i].rectTransform.GetComponent<Image>().color == highlightedColor)
                {

                    _CraftSlots[i].rectTransform.GetComponent<Image>().color = _CraftSlots[i].originalColor;

                }
                else if (_CraftSlots[i].rectTransform.GetComponent<Image>().color != _CraftSlots[i].originalColor)
                {

                    _CraftSlots[i].rectTransform.GetComponent<Image>().color = Color.clear;
                }
                CraftSlot tempSlot = _CraftSlots[i];
                tempSlot.selected = false;
                _CraftSlots[i] = tempSlot;


                if (_CraftSlots[i].rectTransform.gameObject == slot)
                {
                    return;
                }
            }
        }

        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].rectTransform.gameObject == slot)
            {

                //
                //set the previouse selected to false and origional colour!!!
                //

                /*
                for (int j = 0; j < _CraftSlots.Count; j++ )
                {
                    if(_CraftSlots[j].isPreview == true)
                    {
                        _CraftSlots[j].rectTransform.GetComponent<Image>().color = Color.clear;
                    }
                    
                }
                */
                _CraftSlots[i].rectTransform.GetComponent<Image>().color = highlightedColor;

                CraftSlot tempSlot = _CraftSlots[i];
                tempSlot.selected = true;
                _CraftSlots[i] = tempSlot;
                continue;
            }
        }
        // Select (change UI and change state to selected).

    }
    #endregion

    public bool AddDeleteItem(ID.Specification spec, ID.Type type, int variation, Transform hit, ID.SubMenu subMenu, GameObject prefab)
    {
        if (AddItem(spec, type, variation, hit.gameObject, subMenu, prefab))
        {
            //Destroy(hit.gameObject);
            return true;
        }
        return false;

    }

    public bool AddDeleteItemLITERAL(ID.Specification spec, ID.Type type, int variation, Transform hit, ID.SubMenu subMenu, bool LITERAL, GameObject prefab)
    {
        if (!LITERAL)
        {
            if (AddItem(spec, type, variation, null, subMenu, prefab))
            {
                return true;
            }
        }
        else if (AddItem(spec, type, variation, hit.gameObject, subMenu, prefab))
        {
            if (LITERAL)
                Destroy(hit.gameObject);
            return true;
        }
        return false;

    }

    public bool AddItem(ID.Specification spec, ID.Type type, int variation, GameObject pickupGO, ID.SubMenu subMenu, GameObject prefab)
    {
        //if (SlotsContainItem(spec, type, variation)) { }// if quanity < maxQuanity then add it on.

        // Configure slot. (inc quanitity)
        int index = FirstEmptyIndex();
        if (index == -1)
        {
            Debug.Log("index == -1   :::: return");
            return false;
        }

        // Set up the new object to the new item details.
        Item _Item = new Item();
        _Item.specification = spec;
        _Item.type = type;
        _Item.variation = variation;
        _Item.subMenu = subMenu;

        if (prefab != null)
            _Item.prefab = prefab;
        else
         _Item.prefab = GetPrefab(spec, type, variation);

        if (pickupGO != null)
        {
            if (spec == pickupGO.GetComponent<ID>().specification)
                _Item.go = pickupGO;
        }
        // IF CANT ADD TO QUANITITY;

        int existingSlotIndex = -1;
        // Check if a coherant slot already exists.
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.specification == _Item.specification
                && _Slots[i].item.type == _Item.type
                && _Slots[i].item.variation == _Item.variation)
            {
                if (_Slots[i].quantity < _Slots[i].item.prefab.GetComponent<ID>().maxStack)
                {
                    existingSlotIndex = i;
                    break;
                }
            }
        }

        if (existingSlotIndex > -1)
        {
            // Debug.Log("CAN STACK");
            // can stack!
            _Slots[existingSlotIndex].quantity++;
            _Slots[existingSlotIndex].rectTransform.GetChild(1).GetComponent<Text>().text = _Slots[existingSlotIndex].quantity.ToString();


            //new
            index = existingSlotIndex;
            // return?
        }
        else
        {


            if (GetTopLeftFreeIndex(_Item.prefab.GetComponent<ID>().slotWidth, _Item.prefab.GetComponent<ID>().slotHeight) == -1)
            {
                // Debug.Log("Failed");
                return false;
            }

            index = GetTopLeftFreeIndex(_Item.prefab.GetComponent<ID>().slotWidth, _Item.prefab.GetComponent<ID>().slotHeight);

            //Debug.Log("INDEX:::: " + index);

            for (int i = 0; i < _Item.prefab.GetComponent<ID>().slotHeight; i++)
            {
                for (int j = 0; j < _Item.prefab.GetComponent<ID>().slotWidth; j++)
                {
                    //Debug.Log("::::::::::::::::::::::::::::::::: " + (index + (i * totalColombs) + j));
                    _Slots[index + (i * totalColombs) + j].occupied = true; // total row?

                    if ((index + (i * totalColombs) + j) > index)
                        ItemHolders[index + (i * totalColombs) + j].SetActive(false);
                }
            }

            _Slots[index].occupied = true;
            _Slots[index].otherGO = new List<GameObject>();
            _Slots[index].item = _Item;
            _Slots[index].quantity = 1;
            _Slots[index].rectTransform.GetComponent<RawImage>().color = idleColor;
            _Slots[index].rectTransform.GetChild(0).GetComponent<Text>().text = _Item.specification.ToString();
            _Slots[index].rectTransform.GetChild(1).GetComponent<Text>().text = _Slots[index].quantity.ToString();

            // _Slots[index].otherGO = new GameObject[_Slots[index].item.go.GetComponent<ID>().maxStack - 1];

            _Slots[index].item = _Item;

            Rect _tempRect = _Slots[index].rectTransform.rect;
            _tempRect.width = _SlotPrefab.GetComponent<RectTransform>().rect.width + _SlotPrefab.GetComponent<RectTransform>().rect.width * (_Slots[index].item.prefab.GetComponent<ID>().slotWidth - 1);
            _tempRect.height = _SlotPrefab.GetComponent<RectTransform>().rect.height + _SlotPrefab.GetComponent<RectTransform>().rect.height * (_Slots[index].item.prefab.GetComponent<ID>().slotHeight - 1);

            _Slots[index].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _tempRect.width);
            _Slots[index].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _tempRect.height);
            _Slots[index].sizeDelta = _Slots[index].rectTransform.sizeDelta;

            //_Slots[index].rectTransform.GetComponent<Parts>().dur.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            //_Slots[index].rectTransform.GetComponent<Parts>().dur.anchoredPosition = Vector2.zero;

            // USE SAME METHOD FOR SWAPPING! JUST ACCESS THE TEMPITEM!!!!!!!!!!!!!!!!!!!!
            if (_Slots[index].item.go != null)
            {
                if (_Slots[index].item.go.GetComponent<Durabillity>())
                    SetDurAtSlotIndex(index, _Slots[index].item.go.GetComponent<Durabillity>().totalDmgTaken);
            }//!!!!!!!!!!!!!!!

            _Slots[index].rectTransform.SetAsLastSibling();

            if (_Slots[index].item.prefab.GetComponent<ID>().slotWidth > 1)
            {
                Vector3 newPos = _Slots[index].rectTransform.localPosition;
                newPos.x += ((_SlotPrefab.GetComponent<RectTransform>().rect.width / 2f) * (_Slots[index].item.prefab.GetComponent<ID>().slotWidth - 1));
                _Slots[index].rectTransform.localPosition = newPos;
            }
            if (_Slots[index].item.prefab.GetComponent<ID>().slotHeight > 1)
            {
                Vector3 newPos = _Slots[index].rectTransform.localPosition;
                newPos.y -= ((_SlotPrefab.GetComponent<RectTransform>().rect.height / 2f) * (_Slots[index].item.prefab.GetComponent<ID>().slotHeight - 1));
                _Slots[index].rectTransform.localPosition = newPos;
            }
        }

        // Instaniate the prefab if no primary item.
        if (GetPrimaryIndex() == -1)
        {
            // Debug.Log("GETPRIMARYINDEX() == -1");
            _Slots[index].rectTransform.GetComponent<RawImage>().color = highlightedColor;
            _Slots[index].primary = true;



            GameObject newObject = _Item.go;// _Slots[index].item.go

            if (newObject == null)
            {

                newObject = Instantiate(_Slots[index].item.prefab, this.transform.position + transform.forward + transform.right * 0.45f, _Slots[index].item.prefab.transform.rotation) as GameObject;
                _Slots[index].item.go = newObject;
                Debug.Log("INSTAN: ADDED");
                newObject.SetActive(true);
            }
            newObject.transform.position = this.transform.position + transform.forward + transform.right * 0.45f;
            // newObject.transform.rotation = _Slots[index].item.prefab.transform.rotation;

            if (newObject.GetComponent<NavMeshAgent>())
                newObject.GetComponent<NavMeshAgent>().enabled = false;

            if (newObject.GetComponent<AINavigation>())
                newObject.GetComponent<AINavigation>().enabled = false;

            for (int i = 0; i < newObject.GetComponent<ID>().Colliders.Length; i++)
            {
                newObject.GetComponent<ID>().Colliders[i].enabled = false;
            }

            for (int i = 0; i < newObject.GetComponent<ID>().Rigidbodies.Length; i++)
            {
                newObject.GetComponent<ID>().Rigidbodies[i].useGravity = false;
                newObject.GetComponent<ID>().Rigidbodies[i].isKinematic = true;
            }

            if (newObject.GetComponent<Consumable>())
                newObject.GetComponent<Consumable>().enabled = true;




            newObject.layer = 0;

            if (newObject.transform.parent)
            {
                Transform parent = newObject.transform.parent;
                // Debug.Log(newObject.transform.parent.name);
            }

            newObject.transform.parent = _MainCamera.transform;

            // Debug.Log(newObject.transform.localRotation);
            newObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            for (int i = 0; i < _Slots[index].item.prefab.transform.childCount; i++)
            {
                for (int j = 0; j < newObject.transform.childCount; j++)
                {
                    if (newObject.transform.GetChild(j) == _Slots[index].item.prefab.transform.GetChild(i))
                        newObject.transform.GetChild(j).transform.localRotation = _Slots[index].item.prefab.transform.GetChild(i).transform.localRotation;
                }

            }

            //Debug.Log("AFT:" + newObject.transform.localRotation);
            //primaryItem = newObject;
            // _Slots[index].item.go = newObject;


            if (_Slots[index].item.go != null && _Slots[index].quantity > 1)
            {
                newObject.SetActive(false);


                _Slots[index].otherGO.Add(newObject);

                _Slots[index].item.go.SetActive(true);

            }
            else
                _Slots[index].item.SetID(newObject.GetComponent<ID>());



        }
        else
        {

            GameObject newObject = null;

            if (_Slots[index].quantity == 1)
                newObject = _Slots[index].item.go;
            else
                newObject = pickupGO;

            if (newObject == null)
            {
                Debug.Log("INSTAN");
                newObject = Instantiate(_Slots[index].item.prefab, this.transform.position + transform.forward + transform.right * 0.45f, _Slots[index].item.prefab.transform.rotation) as GameObject;
                _Slots[index].item.go = newObject;
            }

            // Debug.Log("FALSE");
            newObject.SetActive(false);


            newObject.transform.position = this.transform.position + transform.forward + transform.right * 0.45f;
            newObject.transform.rotation = _Slots[index].item.prefab.transform.rotation;

            if (newObject.GetComponent<NavMeshAgent>())
                newObject.GetComponent<NavMeshAgent>().enabled = false;

            if (newObject.GetComponent<AINavigation>())
                newObject.GetComponent<AINavigation>().enabled = false;

            for (int i = 0; i < newObject.GetComponent<ID>().Colliders.Length; i++)
            {
                newObject.GetComponent<ID>().Colliders[i].enabled = false;
            }

            for (int i = 0; i < newObject.GetComponent<ID>().Rigidbodies.Length; i++)
            {
                newObject.GetComponent<ID>().Rigidbodies[i].useGravity = false;
                newObject.GetComponent<ID>().Rigidbodies[i].isKinematic = true;
            }

            if (newObject.GetComponent<Consumable>())
                newObject.GetComponent<Consumable>().enabled = true;




            newObject.layer = 0;
            newObject.transform.parent = _MainCamera.transform;
            newObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            /*
            for (int i = 0; i < newObject.transform.childCount; i++)
            {
                newObject.transform.GetChild(i).transform.localRotation = _Slots[index].item.prefab.transform.GetChild(i).transform.localRotation;
            }*/

            for (int i = 0; i < _Slots[index].item.prefab.transform.childCount; i++)
            {
                for (int j = 0; j < newObject.transform.childCount; j++)
                {
                    if (newObject.transform.GetChild(j) == _Slots[index].item.prefab.transform.GetChild(i))
                        newObject.transform.GetChild(j).transform.localRotation = _Slots[index].item.prefab.transform.GetChild(i).transform.localRotation;
                }

            }

            _Slots[index].item.SetID(newObject.GetComponent<ID>());

            if (_Slots[index].quantity > 1)
                _Slots[index].otherGO.Add(newObject);
        }

        RefreshCraftingMenu();
        return true;
    }

    private int GetTopLeftFreeIndex(int width, int height)
    {
        // bool[] checkedSlots = new bool[totalRows * totalColombs];

        int firstIndex = -1;

        // Check Width
        bool widthEmpty = true;
        int column = 0;
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (i > 0)
            {
                column++;
                if (column > (totalColombs - 1)) column = 0;
            }
            if (_Slots[i].occupied == true)
            {
                //checkedSlots[i] = true;
                widthEmpty = false;
                continue;
            }

            // FOUND FIRST EMPTY
            firstIndex = i;

            bool cont = false;
            //check height and continue if poor.
            for (int j = 0; j < height; j++)
            {
                if ((i + (j * totalColombs)) > (_Slots.Length - 1) || _Slots[i + (j * totalColombs)].occupied == true) // added -1
                {
                    cont = true;
                    continue;
                }
            }
            if (cont == true) continue;

            // CHECK TO SEE IF WIDTH FREE.
            for (int j = 0; j < width; j++)
            {
                // IF NOT CONTINUE CHECKING TO FIND ANOTHER EMPTY
                //Debug.Log("COL3: " + (column + j) = " :::::::::::: " + (totalColomn - 1));
                if ((column + j) > (totalColombs - 1) || _Slots[i + j].occupied == true) // if extends row..
                {
                    //if (_Slots[i + j].occupied == true)
                    //checkedSlots[i + j] = true;

                    widthEmpty = false;
                    break;
                }
                else
                {
                    if (j == width - 1)
                    {
                        widthEmpty = true;
                    }
                }
            }

            // IF SO RETURN FROM LOOP ALTOGETHER
            if (widthEmpty == true)
            {
                // Debug.Log("WIDTH FAIL!");
                break;
            }
            continue;
            //return -1;
        }
        if (widthEmpty == false) return -1;

        //Debug.Log("SUCCESS:WIDTH  " + firstIndex);

        if (height <= 1) return firstIndex;


        //Debug.Log("HEIGHT > 1!");

        // Check height

        // THIS SI THE FREAKING PROBLEMO
        for (int row = 1; row < height; row++)
        {
            for (int j = 0; j < width; j++)
            {
                //Debug.Log("BREAKPOINT:  " + (firstIndex + (totalColombs * (row)) + j).ToString());
                if ((firstIndex + (totalColombs * (row)) + j) > _Slots.Length - 1)
                {
                    //Debug.Log("here!");
                    return -1;
                }
                if (_Slots[firstIndex + (totalColombs * (row)) + j].occupied == true)
                {
                    //checkedSlots[firstIndex + (totalColombs * (row)) + j] = true;
                    //Debug.Log("there!");
                    return -1;
                }
            }
        }

        // if failed then checkedSlots[firstIndex] = true;
        // then do whole process until every checked slots is true?

        //Debug.Log("SUCCESS:HEIGHT  ");

        return firstIndex;
    }

    #region UPDATED!!
    /* public void EquipItem(int slot){}*/

    public void OnUISlotClick(GameObject slot)
    {
        int primaryIndexlcl = GetPrimaryIndex();

        // NEED TO DO DELETION ONCE.. NEED TO ADD BOOL INSTEAD OF PRIMAYITEM != NULL AS PRIMARY ITEM WOULD HAVE TO BE DELETED..

        if (primaryIndexlcl > -1)
        {

            // If the primary weapon has been clicked. //NOTE THIS IS WORKING OUT PRIMARY INDEX
            //if (primaryItem.GetComponent<ID>().Compare(_Slots[ItemHolders.IndexOf(slot)].item.specification, _Slots[ItemHolders.IndexOf(slot)].item.type, _Slots[ItemHolders.IndexOf(slot)].item.variation))
            if (ItemHolders.IndexOf(slot) == primaryIndexlcl)
            {
                // "Unequipt"
                //Debug.Log("UNEQUIPT");

                // NEW
                //GameObject.Destroy(_Slots[primaryIndexlcl].item.go);
                _Slots[primaryIndexlcl].item.go.SetActive(false);

                //primaryItem = null;
                _Slots[primaryIndexlcl].primary = false;
                _Slots[primaryIndexlcl].rectTransform.GetComponent<RawImage>().color = idleColor;
                return;
            }




            // Destroy current item
            //Debug.Log("SWITCH");

            // new
            //GameObject.Destroy(_Slots[primaryIndexlcl].item.go);
            _Slots[primaryIndexlcl].item.go.SetActive(false);

            //primaryItem = null;

            _Slots[primaryIndexlcl].primary = false;
            _Slots[primaryIndexlcl].rectTransform.GetComponent<RawImage>().color = idleColor;

            if (_Slots[primaryIndexlcl].occupied == false)
                _Slots[primaryIndexlcl].rectTransform.GetComponent<RawImage>().color = _Slots[primaryIndexlcl].originalColor;
        }

        if (_Slots.Length <= 1) return;

        if (!_Slots[ItemHolders.IndexOf(slot)].occupied)
            return;

        //           _Slots[primaryIndexlcl].rectTransform.GetComponent<RawImage>().color = _Slots[primaryIndexlcl].originalColor;

        _Slots[ItemHolders.IndexOf(slot)].rectTransform.GetComponent<RawImage>().color = highlightedColor;
        _Slots[ItemHolders.IndexOf(slot)].primary = true;
        //NEW
        Debug.Log("just removed");
        if (_Slots[ItemHolders.IndexOf(slot)].item.go != null)
            _Slots[ItemHolders.IndexOf(slot)].item.go.SetActive(true);
        /*
        GameObject tempItem = _Slots[ItemHolders.IndexOf(slot)].item.prefab;
        //prefab sometimes doesnt exist..

        GameObject newObject = Instantiate(tempItem, this.transform.position + transform.forward + transform.right, _Slots[ItemHolders.IndexOf(slot)].item.prefab.transform.rotation) as GameObject;
        newObject.layer = 0;
        newObject.transform.parent = this.transform;
        newObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        
        //
        _Slots[ItemHolders.IndexOf(slot)].item.SetID(newObject.GetComponent<ID>());
        _Slots[ItemHolders.IndexOf(slot)].item.go = newObject;
        //
     

        for (int i = 0; i < newObject.GetComponent<ID>().Colliders.Length; i++)
        {
            Destroy(newObject.GetComponent<ID>().Colliders[i]);
        }

        for (int i = 0; i < newObject.GetComponent<ID>().Rigidbodies.Length; i++)
        {
            newObject.GetComponent<ID>().Rigidbodies[i].useGravity = false;
            newObject.GetComponent<ID>().Rigidbodies[i].isKinematic = true;
        }

        if (newObject.GetComponent<Consumable>())
            newObject.GetComponent<Consumable>().enabled = true;

        newObject.layer = 0;

        //primaryItem = newObject;

        _Slots[ItemHolders.IndexOf(slot)].item.go = newObject;
            */



    }

    public void DropItem(GameObject item, bool RigidbodyOn)
    {
        if (item == null) return;

        // Identify slot holding item.
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.go != item) continue;

            //GameObject newObject = Instantiate(_Slots[i].item.prefab, this.transform.position + transform.forward, this.transform.rotation) as GameObject;
            GameObject newObject = item;
            newObject.SetActive(true);

            newObject.transform.SetParent(null);

            //Vector3 tempTransformEul = newObject.transform.rotation.eulerAngles;
            //tempTransformEul.y += 90; 
            //newObject.transform.localRotation = Quaternion.Euler(tempTransformEul);

            if (RigidbodyOn)
            {
                if (newObject.GetComponent<Rigidbody>())
                {
                    newObject.GetComponent<Rigidbody>().isKinematic = false;
                    newObject.GetComponent<Rigidbody>().useGravity = true;
                    newObject.GetComponent<Rigidbody>().AddForce(this.transform.forward * 4f, ForceMode.Impulse);
                }
            }

            for (int j = 0; j < newObject.GetComponent<ID>().Colliders.Length; j++)
            {
                newObject.GetComponent<ID>().Colliders[j].enabled = true;
            }

            if (newObject.GetComponent<Consumable>())
                newObject.GetComponent<Consumable>().enabled = false;

            newObject.layer = 8;
            newObject.name = _Slots[i].item.prefab.name;

            //Debug.Log("ADDEDD!");

            //HEREEEEE

            //    tempSlot.item.go = tempSlot.otherGO[0];
            //   tempSlot.otherGO.RemoveAt(0);

            // if selected

            if (tempSlot.primary)
                tempSlot.item.go.SetActive(true);

            ///

            newObject = _Slots[i].item.go;

            RemoveItemtAtSlotIndex(i, 1, false);
            return;
        }
    }
    #endregion

    #region UPDATED2222
    public void RemoveItem(GameObject item, int quantity)
    {
        // Identify slot holding item.
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.go != item) continue;

            //Debug.Log("REMOVE: " + item.name);
            RemoveItemtAtSlotIndex(i, quantity, true);
            return;
        }
    }
    #endregion

    // woudl love to make this a function of the Slot. e.g. Slot[i].Reset(); how?
    private void RemoveItemtAtSlotIndex(int index, int quantity, bool deleteGO)
    {

        Slot tempSlot = _Slots[index];

        tempSlot.quantity -= quantity;

        /*if (_Slots[index].quantity > 1)
            _Slots[index].otherGO[_Slots[index].quantity - 1] = pickupGO;
        */


        if (tempSlot.quantity <= 0)
        {

            if (deleteGO)
            {
                GameObject.Destroy(tempSlot.item.go);

                for (int i = 0; i < tempSlot.otherGO.Count; i++)
                {
                    GameObject.Destroy(tempSlot.otherGO[i]);
                }
                tempSlot.otherGO.Clear();
            }

            for (int i = 0; i < tempSlot.item.GetID().slotHeight; i++)
            {
                for (int j = 0; j < tempSlot.item.GetID().slotWidth; j++)
                {
                    _Slots[index + (i * totalColombs) + j].occupied = false;


                    if ((index + (i * totalColombs) + j) > index)
                        ItemHolders[index + (i * totalColombs) + j].SetActive(true);
                }
            }

            tempSlot.rectTransform.GetComponent<RawImage>().color = tempSlot.originalColor;
            tempSlot.rectTransform.GetChild(0).GetComponent<Text>().text = "";
            tempSlot.rectTransform.GetChild(1).GetComponent<Text>().text = "";
            tempSlot.occupied = false;
            tempSlot.primary = false;

            tempSlot.quantity = 0;


            Rect _tempRect = _Slots[index].rectTransform.rect;
            _tempRect.width = _SlotPrefab.GetComponent<RectTransform>().rect.width;
            _tempRect.height = _SlotPrefab.GetComponent<RectTransform>().rect.height;

            _Slots[index].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _tempRect.width);
            _Slots[index].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _tempRect.height);
            _Slots[index].sizeDelta = _Slots[index].rectTransform.sizeDelta;

            _Slots[index].rectTransform.GetComponent<Parts>().dur.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            _Slots[index].rectTransform.GetComponent<Parts>().dur.anchoredPosition = Vector2.zero;

            _Slots[index].rectTransform.SetAsFirstSibling();

            if (_Slots[index].item.GetID().slotWidth > 1)
            {
                Vector3 newPos = _Slots[index].rectTransform.localPosition;
                newPos.x -= ((_SlotPrefab.GetComponent<RectTransform>().rect.width / 2f) * (_Slots[index].item.prefab.GetComponent<ID>().slotWidth - 1));
                _Slots[index].rectTransform.localPosition = newPos;
            }
            if (_Slots[index].item.GetID().slotHeight > 1)
            {
                Vector3 newPos = _Slots[index].rectTransform.localPosition;
                newPos.y += ((_SlotPrefab.GetComponent<RectTransform>().rect.height / 2f) * (_Slots[index].item.prefab.GetComponent<ID>().slotHeight - 1));
                _Slots[index].rectTransform.localPosition = newPos;
            }

            tempSlot.item = new Item(); //undo?

            //primaryItem = null;
        }
        else
        {


            if (deleteGO)
            {

                GameObject.Destroy(tempSlot.item.go);


                //try this
                //tempSlot.item.go.SetActive(true);
            }

            // if selected

            // DO I NEED TO SWITCH TO ITEM.GO NO MATTER WHAT?
            // IF I DROP IT DOESNT "DELETEGO"

            tempSlot.item.go = tempSlot.otherGO[0];


            tempSlot.otherGO.RemoveAt(0);

            if (tempSlot.primary)
                tempSlot.item.go.SetActive(true);



            tempSlot.rectTransform.GetChild(1).GetComponent<Text>().text = tempSlot.quantity.ToString();

        }
        _Slots[index] = tempSlot;
        RefreshCraftingMenu();
        return;
    }

    private void RemoveItemtAtSlotIndexProximity(int index, int quantity, bool deleteGO)
    {
        if (deleteGO)
            GameObject.Destroy(_ProximitySlots[index].item.go);

        _ProximitySlots[index].HideAndDefault();

        RefreshCraftingMenu();
        return;
    }

    #region Wrapped Functions (private)

    #region ~~CHECKED~~
    private RectTransform GetFirstSlotGameObject(ID.Specification spec, ID.Type type, int variation)
    {
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.specification == spec && _Slots[i].item.type == type && _Slots[i].item.variation == variation)
            {
                return _Slots[i].rectTransform;
            }
        }

        Debug.Log("NOT FOUND!");
        return null;
    }
    #endregion

    #region UPDATED!!
    private List<int> GetIndexesOfGameobject(ID.Specification spec, ID.Type type, int variation, int quantity)
    {
        List<int> commonSlots = new List<int>();
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.specification == spec && _Slots[i].item.type == type && _Slots[i].item.variation == variation)
            {

                for (int j = 0; j < _Slots[i].quantity; j++)
                {
                    commonSlots.Add(i);

                    if (commonSlots.Count == quantity)
                    {

                        return commonSlots;
                    }
                }


            }
        }

        // Debug.Log("NOT FOUND");
        return commonSlots;
    }

    private List<int> GetIndexesOfGameobjectProximity(ID.Specification spec, ID.Type type, int variation, int quantity)
    {
        List<int> commonSlots = new List<int>();
        for (int i = 0; i < _ProximitySlots.Count; i++)
        {
            if (_ProximitySlots[i].item.specification == spec && _ProximitySlots[i].item.type == type && _ProximitySlots[i].item.variation == variation)
            {
                commonSlots.Add(i);
                if (commonSlots.Count == quantity) return commonSlots;
            }
        }

        //Debug.Log("NOT FOUND2");
        return commonSlots;
    }
    #endregion

    #region ~~CHECKED~~
    private int GetIndexOfSlot(ID.Specification spec, ID.Type type, int variation)
    {
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].item.specification == spec && _CraftSlots[i].item.type == type && _CraftSlots[i].item.variation == variation)
            {
                return i;
            }
        }

        Debug.Log("NOT FOUND");
        return 0;
    }

    private GameObject GetSlot(ID.Specification spec, ID.Type type, int variation)
    {
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].item.specification == spec && _CraftSlots[i].item.type == type && _CraftSlots[i].item.variation == variation)
            {
                return _CraftSlots[i].rectTransform.gameObject;
            }
        }

        Debug.Log("NOT FOUND");
        return null;
    }

    private bool CraftSlotsContains(ID.Specification spec, ID.Type type, int variation)
    {
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].item.specification == spec && _CraftSlots[i].item.type == type && _CraftSlots[i].item.variation == variation)
            {
                return true;
            }
        }

        return false;
    }

    private GameObject GetSelectedCraftSlotGO()
    {
        for (int i = 0; i < _CraftSlots.Count; i++)
        {
            if (_CraftSlots[i].selected == true)
            {
                return _CraftSlots[i].rectTransform.gameObject;
            }
        }
        return null;
    }
    #endregion

    #region UPDATED!!
    private List<RectTransform> GetQuantityOfGameObjects(ID.Specification spec, ID.Type type, int variation, int quantity)
    {
        List<RectTransform> commonSlots = new List<RectTransform>();
        for (int i = 0; i < _Slots.Length; i++)
        {

            if (_Slots[i].item.specification == spec && _Slots[i].item.type == type && _Slots[i].item.variation == variation)
            {
                //Debug.Log("ADDED: " + _Slots[i].rectTransform.name);
                commonSlots.Add(_Slots[i].rectTransform);
                if (commonSlots.Count == quantity) return commonSlots;
            }

        }
        return commonSlots;
    }

    private int GetTotalQuantity(ID.Specification spec, ID.Type type, int variation)
    {
        int quantity = 0;
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.specification == spec && _Slots[i].item.type == type && _Slots[i].item.variation == variation)
            {
                quantity += _Slots[i].quantity;
            }
        }

        // NEW
        for (int i = 0; i < _ProximitySlots.Count; i++)
        {
            // Debug.Log("Proximi: " + _ProximitySlots[i].item.prefab.name);
            if (_ProximitySlots[i].item.specification == spec && _ProximitySlots[i].item.type == type && _ProximitySlots[i].item.variation == variation)
            {
                //Debug.Log("HEY!!! YEAH");
                quantity += 1;
            }
        }
        return quantity;
    }

    private bool SlotsContainItem(ID.Specification spec, ID.Type type, int variation)
    {
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].item.GetID() != null)
            {
                if (_Slots[i].item.GetID().Compare(spec, type, variation))
                    return true;
            }
            else
            {
                if (_Slots[i].item.specification == spec && _Slots[i].item.type == type && _Slots[i].item.variation == variation)
                    return true;
            }
        }

        return false;
    }

  
    private GameObject GetPrefab(ID.Specification spec, ID.Type type, int variation)
    {
        if (Prefabs.Length == 0)
        {
            Debug.LogError("PREFABLENGTH 0");
            Prefabs = Resources.LoadAll<GameObject>("Prefabs");
        }
        for (int i = 0; i < Prefabs.Length; i++)
        {
            if (!Prefabs[i].GetComponent<ID>()) continue;
            if (Prefabs[i].GetComponent<ID>().Compare(spec, type, variation))
            {
                GameObject prefab = Prefabs[i];
                return prefab;
            }
        }
        Debug.LogError("Prefab not found! + " + spec + " + " + type + " + " + variation.ToString());
        return null;
    }

    public int FirstEmptyIndex()
    {
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].occupied == false)
                return i;
        }
        // Debug.Log("NONE EMPTY");
        return -1;
    }

    public int FirstFreeProximityIndex()
    {
        for (int i = 0; i < _ProximitySlots.Count; i++)
        {
            if (_ProximitySlots[i].occupied == false)
                return i;
        }
        return -1;
    }

    public GameObject GetPrimaryObject()
    {
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].primary)
                return _Slots[i].item.go;
        }

        return null;
    }

    public int GetPrimaryIndex()
    {
        for (int i = 0; i < _Slots.Length; i++)
        {
            if (_Slots[i].primary)
                return i;
        }

        //Debug.Log("Not found");
        return -1;
    }

    private ProximitySlot newProximitySlot(GameObject proxSlot)
    {
        ProximitySlot tempProxSlot = new ProximitySlot();

        tempProxSlot.item = new Item();
        tempProxSlot.originalColor = _ProximityOptionPrefab.GetComponent<Image>().color;
        tempProxSlot.rectTransform = proxSlot.GetComponent<RectTransform>();
        tempProxSlot.occupied = false;
        tempProxSlot.textName = proxSlot.transform.GetChild(0).GetComponent<Text>();

        tempProxSlot.textName.text = "";

        return tempProxSlot;
    }



    private bool InitialiseProximitySlots()
    {
        _ProximitySlots.Clear();

        for (int i = 0; i < 10; i++)
        {
            GameObject proxSlotObject = Instantiate(_ProximityOptionPrefab) as GameObject;
            proxSlotObject.transform.SetParent(_ProximityContent.transform);
            proxSlotObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (_ProximityContent.transform.GetComponent<RectTransform>().sizeDelta.y / 2f) - (proxSlotObject.GetComponent<RectTransform>().sizeDelta.y * (_ProximitySlots.Count)) - proxSlotObject.GetComponent<RectTransform>().sizeDelta.y / 2f - 3f, 0f);
            proxSlotObject.GetComponent<RectTransform>().transform.localScale = new Vector3(1f, 1f, 1f);
            proxSlotObject.SetActive(false);

            _ProximitySlots.Add(newProximitySlot(proxSlotObject));
        }

        return true;
    }

    private bool InitialiseSlots()  // I WANT TO HAVE POINTER AND PASS BY REFERENCE
    {
        ItemHolders.Clear();

        RectTransform slotContentRecTra = _SlotContent.transform.GetComponent<RectTransform>();
        RectTransform slotPrefabRectTrans = _SlotPrefab.GetComponent<RectTransform>();

        float sclSlotWidth = (slotContentRecTra.rect.width - 10F) / totalColombs; // -10 came from calculation on paper. to do with spacing.
        float sclSlotHeight = (slotContentRecTra.rect.height - 15f) / totalRows;

        if (scaleSlots)
        {
            //update the existing prefab so that it undertakes stretching.
            slotPrefabRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sclSlotWidth);
            slotPrefabRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sclSlotHeight);
        }

        for (int row = 0; row < totalRows; row++)
        {
            for (int colomb = 0; colomb < totalColombs; colomb++)
            {
                GameObject slot = Instantiate(_SlotPrefab);
                RectTransform slotRectTrans = slot.GetComponent<RectTransform>();
                slot.transform.SetParent(_SlotContent.transform);

                Vector3 newPos = Vector3.zero;
                newPos.x = -(slotContentRecTra.rect.width / 2f) + 5f + (slotRectTrans.rect.width / 2f) + (colomb * slotRectTrans.rect.width);
                newPos.y = (slotContentRecTra.rect.height / 2f) - 10f - (slotRectTrans.rect.height / 2f) - (row * slotRectTrans.rect.height);

                slotRectTrans.localPosition = newPos;
                slotRectTrans.localScale = new Vector3(1f, 1f, 1f);

                slot.SetActive(true);



                ItemHolders.Add(slot);
            }
        }

        _Slots = new Slot[totalRows * totalColombs];

        for (int i = 0; i < _Slots.Length; i++)
        {
            _Slots[i].index = i;
            _Slots[i].item = new Item();
            _Slots[i].otherGO = new List<GameObject>(); //new
            _Slots[i].occupied = false;
            _Slots[i].quantity = 0;
            _Slots[i].originalColor = ItemHolders[i].GetComponent<RawImage>().color;
            _Slots[i].rectTransform = ItemHolders[i].GetComponent<RectTransform>();
            _Slots[i].sizeDelta = _Slots[i].rectTransform.sizeDelta;
            _Slots[i].primary = false;
        }


        return true;
    }
    #endregion

    #endregion
    // I MAY NEED TO CREATE DATABASE OF THE ITEMS... With the rotation etc (prefab details);
}
