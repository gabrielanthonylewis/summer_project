using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Raycast : MonoBehaviour
{

    int layerMask = 1 << 8;

    public float distance = 3f;

    List<Transform> highlightedObjects = new List<Transform>(); // may change to "rasycastbehaviour"

    private bool showNoSpaceroutine = false;
    private bool showOnce = true;
    bool stopShowno = false;

    public GameObject CarriedObject = null;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {

        if (CarriedObject)
        {
            CarriedObject.transform.position = this.transform.position + transform.forward * 1.5F;

            if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                ID tempID = CarriedObject.GetComponent<ID>();
                for (int i = 0; i < tempID.Rigidbodies.Length; i++)
                {
                    tempID.Rigidbodies[i].useGravity = true;
                    tempID.Rigidbodies[i].isKinematic = false;
                }
              //  tempID.Rigidbodies[0].isKinematic = false;

                /*
                for (int coli = 0; coli < tempID.Colliders.Length; coli++)
                {
                    tempID.Colliders[coli].enabled = true;
                }*/
                CarriedObject = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Camera.main.GetComponent<Inventory>().GetPrimaryIndex() != -1)
            {
                if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue > 0f)
                {
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Animation>())
                    {




                        //Debug.Log(UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue);
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Animation>().isPlaying) return;
                        Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Animation>().Play();


                    }
                    UIManager.Instance.AdjustBar(-35f, UIManager.Bars.Sprint);

                }
            }


        }

        /*
        RaycastHit hit3;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit3, distance))
        {

            if (hit3.transform != null)
            {

            // Debug.Log("NAME: " + hit.transform.name);

                if (Camera.main.GetComponent<Inventory>().GetPrimaryIndex() != -1)
                {

                    if (hit3.transform.gameObject.layer == 9)
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                        {
                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().canPlaceInGround)
                            {
                                MeshRenderer[] mats = Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponentsInChildren<MeshRenderer>();
                                Color[] originalColors = new Color[mats.Length];
                                for(int i = 0; i < mats.Length; i++)
                                {
                                    originalColors[i] = mats[i].material.color;
                                    Color transColor = mats[i].material.color;
                                    transColor.a = 0.4f;


                                    mats[i].material.color = transColor;
                                }

                            }

                        }
                    }
                }
            }

        }*/


        // NEED TO DO FUNCTION WHEN ANIMATION IS DONE.
        // THIS SYSTEM AUTOMATIACLLY DOES FUNCTION. (WHICH IS GOOD FOR SOME BUT NOT FOR PLACING TORCH SAY.

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;

            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, distance))
            {

                if (hit.transform == null) return;

                // Debug.Log("NAME: " + hit.transform.name);

                if (Camera.main.GetComponent<Inventory>().GetPrimaryIndex() == -1) return;

                if (hit.transform.gameObject.layer == 9)
                {





                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().canPlaceInGround)
                        {
                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Animation>())
                            {
                                if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Animation>().isPlaying)
                                    Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Animation>().Stop();
                            }

                            GameObject torch = Camera.main.GetComponent<Inventory>().GetPrimaryObject();
                            Camera.main.GetComponent<Inventory>().DropItem(torch, false);




                            torch.transform.position = hit.point + new Vector3(0f, torch.GetComponent<ID>().PrimaryObject.GetComponent<Collider>().bounds.extents.y - 0.01f, 0f);//+ new Vector3(0f, 0.2F, 0f);
                            torch.transform.rotation = this.transform.parent.transform.rotation;

                            if (torch.GetComponentInChildren<FirePlace>())
                            {
                                //   torch.GetComponentInChildren<FirePlace>().EnableWarmth(true);
                            }
                            return;
                        }
                    }

                }


                if (hit.transform.GetComponent<Construction>())
                {
                    Debug.Log("NAME: " + hit.transform.name);
                    GameObject primary = Camera.main.GetComponent<Inventory>().GetPrimaryObject();


                    if (primary != null)
                    {

                        ID tempID = primary.GetComponent<ID>();
                        if (hit.transform.GetComponent<Construction>().AddToConstruction(tempID.specification, tempID.type, tempID.variation))
                        {
                            Camera.main.GetComponent<Inventory>().DropItem(primary, false);
                            Destroy(primary);
                        }
                    }
                }

                if (!hit.transform.GetComponent<ID>()) return;



                // Debug.Log("NAME: " + hit.transform.name);



                if (hit.transform.GetComponent<ObjectHealth>())
                {
                    if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue <= 0) return;
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>())
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>().currentDurabillity <= 0f) return;

                    if (hit.transform.GetComponent<ID>()._LifeForm != ID.Lifeform.NULL)
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject())
                        {
                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().transform.GetComponent<ID>().specification == ID.Specification.StoneKnife)
                            {
                                hit.transform.GetComponent<ObjectHealth>().StabToGet();

                                if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>())
                                    Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>().Decrease(20);

                                return;
                            }

                        }
                        /*
                        if (hit.transform.GetComponent<ID>().Rigidbodies[0] != null)
                        {
                         
                             Debug.Log("HIT: " + hit.transform.name);
                        hit.transform.GetComponent<AINavigation>().enabled = false;
                        hit.transform.GetComponent<NavMeshAgent>().enabled = false;
                            hit.transform.GetComponent<ID>().Rigidbodies[0].isKinematic = false;
                            hit.transform.GetComponent<ID>().Rigidbodies[0].useGravity = true;
                            hit.transform.GetComponent<ID>().Rigidbodies[0].AddForce(transform.up * 10f + -transform.forward * 10f, ForceMode.Impulse);
                            hit.transform.GetComponent<ID>().Rigidbodies[0].isKinematic = true;
                            hit.transform.GetComponent<ID>().Rigidbodies[0].useGravity = false;
                          hit.transform.GetComponent<NavMeshAgent>().enabled = true;
                        hit.transform.GetComponent<AINavigation>().enabled = true;
                            
                        }*/

                        float dmg = 0f;
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Spear)
                            dmg = 22f;
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.StoneKnife)
                            dmg = 18f;
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.WoodenAxe)
                            dmg = 18f;
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Stone)
                            dmg = 12f;
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.LongStick)
                            dmg = 5f;
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Stick)
                            dmg = 2f;

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>())
                            Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>().Decrease(10);

                        if (this.transform.tag == "MainCamera")
                        {
                            hit.transform.GetComponent<ObjectHealth>().ReduceHealth(dmg, this.transform.parent);

                        }
                        else
                            hit.transform.GetComponent<ObjectHealth>().ReduceHealth(dmg, this.transform);
                    }
                    else if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.WoodenAxe)
                    {
                        //when animation finished
                        Debug.Log("HEY");
                        Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Durabillity>().Decrease(6);

                        if (this.transform.tag == "MainCamera")
                        {

                            hit.transform.GetComponent<ObjectHealth>().ReduceHealth(25f, this.transform.parent);

                        }
                        else
                            hit.transform.GetComponent<ObjectHealth>().ReduceHealth(25f, this.transform);

                    }

                    return;
                }



                if (hit.transform.GetComponent<ID>().specification == ID.Specification.WoodenTorch)
                {
                    Debug.Log("aaa");
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.BurningStick)
                    {


                        hit.transform.GetComponentInChildren<FirePlace>().LightFire();
                        Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponentInChildren<FirePlace>().AdjustCurrentLifetime(-10);
                    }
                }



                if (hit.transform.GetComponent<ID>()._StaticTag == ID.StaticTag.Water)
                {
                    //Debug.Log("REFILLED HIT");
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>())
                    {
                        //Debug.Log("REFILLED0000");
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().canRefill)
                        {
                            if (!Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().requiresHeatingToHarden)
                            {
                                //Debug.Log("REFILLED");
                                Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().remaining = Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().hydration;
                                Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().isPure = false;
                            }
                        }
                    }

                }


                /*if (hit.transform.GetComponent<ID>().isFlamable)
                {
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == Inventory.Specification.BurningStick)
                    {
                        hit.transform.GetChild(1).GetComponent<FireSource>().LightFire();
                        this.GetComponent<Inventory>().RemoveItem(this.GetComponent<Inventory>().GetPrimaryObject(), 1);
                    }
                }*/


                if (hit.transform.GetComponent<FishAI>())
                {
                    Debug.Log("HEY");
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Spear)
                        {
                            Debug.Log("HEY2");
                            ID _ID = hit.transform.GetComponent<ID>();
                            hit.transform.GetComponent<FishAI>().enabled = false;
                            this.GetComponent<Inventory>().AddDeleteItem(_ID.specification, _ID.type, _ID.variation, hit.transform, _ID.subMenu, _ID.Prefab);
                        }
                    }
                }


                if (hit.transform.GetComponent<ID>().specification == ID.Specification.Stone)
                {
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Stone)
                        {
                            Camera.main.GetComponent<Inventory>().RemoveItem(Camera.main.GetComponent<Inventory>().GetPrimaryObject(), 1);
                            Camera.main.GetComponent<Inventory>().AddDeleteItemLITERAL(ID.Specification.RefinedStone, ID.Type.Weapon, 0, hit.transform, ID.SubMenu.Misc, true, null);
                            return;
                        }

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.RefinedStone)
                        {
                            Camera.main.GetComponent<Inventory>().RemoveItem(Camera.main.GetComponent<Inventory>().GetPrimaryObject(), 1);
                            Camera.main.GetComponent<Inventory>().AddDeleteItemLITERAL(ID.Specification.StoneKnife, ID.Type.Weapon, 0, hit.transform, ID.SubMenu.ToolsWeapons, false, null);
                            return;
                        }

                    }

                }

                if (hit.transform.GetComponent<ID>().specification == ID.Specification.FirePlace || hit.transform.GetComponent<ID>().specification == ID.Specification.CampfireBase)
                {
                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.WoodenTorch)
                        {
                            Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponentInChildren<FirePlace>().LightFire();

                        }

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.FireWood)
                        {
                            if (hit.transform.GetComponentInChildren<FirePlace>().AddLog())
                            {
                                this.GetComponent<Inventory>().RemoveItem(this.GetComponent<Inventory>().GetPrimaryObject(), 1);
                                return;
                            }

                        }

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Stick)
                        {
                            if (hit.transform.GetComponentInChildren<FirePlace>().AddStick())
                            {
                                Debug.LogError("SUCCESS!");
                                this.GetComponent<Inventory>().RemoveItem(this.GetComponent<Inventory>().GetPrimaryObject(), 1);
                                return;
                            }
                            else
                            {
                                Debug.LogError("FAIL1");
                            }

                        }


                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.LongStick)
                        {
                            if (hit.transform.GetComponentInChildren<FirePlace>().AddLongStick())
                            {
                                this.GetComponent<Inventory>().RemoveItem(this.GetComponent<Inventory>().GetPrimaryObject(), 1);
                                return;
                            }

                        }

                    }

                    if (hit.transform.GetChild(0).GetComponent<FirePlace>().isLit)
                    {
                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                        {


                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.Stick)
                            {
                                hit.transform.GetComponentInChildren<FirePlace>().AdjustCurrentLifetime(+30f);
                                this.GetComponent<Inventory>().RemoveItem(this.GetComponent<Inventory>().GetPrimaryObject(), 1);
                                return;

                            }


                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>())
                            {
                                if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().canDrink)
                                {
                                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().remaining > 0f)
                                    {

                                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().isPure == false)
                                        {
                                            // do same code as cookitem but purifies instead,.
                                            Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().isPure = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                        {
                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>())
                            {
                                if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().requiresHeatingToHarden)
                                {
                                    // do same code as cookitem but hardens instead. 
                                    Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().requiresHeatingToHarden = false;
                                }
                            }
                        }

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                        {
                            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>())
                            {
                                if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().needsCooking == true)
                                {
                                    if (hit.transform.GetComponent<Cooking>().FirstFreeCookingSlot() == -1) return;

                                    //Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<Consumable>().needsCooking = false;
                                    GameObject itemtobeCooked = Camera.main.GetComponent<Inventory>().GetPrimaryObject();
                                    Camera.main.GetComponent<Inventory>().DropItem(itemtobeCooked, true);
                                    hit.transform.GetComponent<Cooking>().CookItem(itemtobeCooked, true);
                                    return;
                                }
                            }
                        }


                    }


                    if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>())
                    {

                        if (Camera.main.GetComponent<Inventory>().GetPrimaryObject().GetComponent<ID>().specification == ID.Specification.BurningStick)
                        {
                            if (hit.transform.GetChild(0).GetComponent<FirePlace>().LightFire())
                            {
                                this.GetComponent<Inventory>().RemoveItem(this.GetComponent<Inventory>().GetPrimaryObject(), 1);
                                return;
                            }
                        }
                    }

                }
            }

        }

        RaycastHit hit2;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit2, distance, layerMask))
        {

            //Debug.Log(hit.transform.name);

            if (!highlightedObjects.Contains(hit2.transform))
            {
                if (showNoSpaceroutine) stopShowno = true;
                // Debug.Log("DOESNT CONTAIN MATE");
                List<Transform> tempList = new List<Transform>();
                tempList = highlightedObjects;

                for (int i = 0; i < highlightedObjects.Count; i++)
                {
                    // highlightedObjects[i].GetComponent<RaycastBehaviour>().Hit(false);
                    tempList.Remove(tempList[i]);
                }

                highlightedObjects = tempList;

                //hit.transform.GetComponent<RaycastBehaviour>().Hit(true);
                highlightedObjects.Add(hit2.transform);


                showOnce = true;

            }

            if (showOnce)
            {
                showOnce = false;

                if (hit2.transform.GetComponent<ID>().isRooted)
                {
                    UIManager.Instance.ShowBar(UIManager.Bars.E, true);
                    UIManager.Instance.SetBar(10f, UIManager.Bars.E);
                }

                if (!showNoSpaceroutine)
                    UIManager.Instance.ShowPickup(hit2.transform.name, true);
            }


            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!CarriedObject)
                {
                    ID _hit2ID = hit2.transform.GetComponent<ID>();
                    if (_hit2ID)
                    {
                        if (!_hit2ID.isRooted)
                        {
                            CarriedObject = hit2.transform.gameObject;

                            for (int i = 0; i < hit2.transform.GetComponent<ID>().Rigidbodies.Length; i++)
                            {
                                _hit2ID.Rigidbodies[i].useGravity = false;
                            }
                            //_hit2ID.Rigidbodies[0].isKinematic = true;

                            /*
                            for (int coli = 0; coli < _hit2ID.Colliders.Length; coli++)
                            {
                                _hit2ID.Colliders[coli].enabled = false;
                            }
                              */  

                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hit2.transform.GetComponent<ID>().isRooted && !CarriedObject)
                {
                    UIManager.Instance.AdjustBar(+22f, UIManager.Bars.E);
                    UIManager.Instance.AdjustBar(-3f, UIManager.Bars.Sprint);
                    // UIManager.Instance.AdjustBar(-0.4f, UIManager.Bars.Hydration); //energy, energy regens if stomach..
                    // UIManager.Instance.AdjustBar(-0.3f, UIManager.Bars.Food); //energy, energy regens if stomach..
                }
                if (!hit2.transform.GetComponent<ID>().isRooted || UIManager.Instance.GetBar(UIManager.Bars.E).currentValue >= 100F && !CarriedObject)
                {

                    ID _ID = hit2.transform.GetComponent<ID>();



                    if (this.GetComponent<Inventory>().FirstEmptyIndex() == -1)
                    {
                        //Debug.Log("RETURN>>>> FIRSTEMPTYINDEX() == -1");
                        if (!showNoSpaceroutine)
                        {
                            StartCoroutine("ShowNoSpaceRoutine");
                        }



                        UIManager.Instance.ShowPickup("null", false);

                        UIManager.Instance.ShowBar(UIManager.Bars.E, false);

                        showOnce = false;
                        if (hit2.transform.GetComponent<ID>().isRooted)
                        {
                            for (int i = 0; i < hit2.transform.GetComponent<ID>().Rigidbodies.Length; i++)
                            {
                                hit2.transform.GetComponent<ID>().Rigidbodies[i].useGravity = true;
                                hit2.transform.GetComponent<ID>().Rigidbodies[i].isKinematic = false;
                            }
                            hit2.transform.GetComponent<ID>().isRooted = false;
                        }
                        return;
                    }

                    hit2.transform.GetComponent<ID>().isRooted = false;
                    UIManager.Instance.ShowPickup("null", false);

                    UIManager.Instance.ShowBar(UIManager.Bars.E, false);
                    CarriedObject = null;
                    showOnce = false;
                    if (!this.GetComponent<Inventory>().AddDeleteItem(_ID.specification, _ID.type, _ID.variation, hit2.transform, _ID.subMenu, _ID.Prefab))
                    {
                  

                        if (!showNoSpaceroutine) StartCoroutine("ShowNoSpaceRoutine");
                        for (int i = 0; i < hit2.transform.GetComponent<ID>().Rigidbodies.Length; i++)
                        {
                            hit2.transform.GetComponent<ID>().Rigidbodies[i].useGravity = true;
                            hit2.transform.GetComponent<ID>().Rigidbodies[i].isKinematic = false;
                        }


                       
                        //Debug.Log("RETURN>>>> AddDeleteItem FAILED");
                        return;
                    }
                    else
                    {
                        highlightedObjects.Remove(hit2.transform);
                        if (hit2.transform.GetComponentInChildren<FirePlace>())
                        {
                            //   hit2.transform.GetComponentInChildren<FirePlace>().EnableWarmth(false);
                        }
                    }
                    return;

                }
            }
        }
        else
        {


            if (highlightedObjects.Count == 0)
            {

                // if (showNoSpaceroutine) stopShowno = true;
                showOnce = true;
                return;
            }


            List<Transform> tempList = new List<Transform>();
            tempList = highlightedObjects;

            for (int i = 0; i < highlightedObjects.Count; i++)
            {
                //highlightedObjects[i].GetComponent<RaycastBehaviour>().Hit(false);


                tempList.Remove(tempList[i]);
            }

            UIManager.Instance.ShowPickup("null", false);
            // UIManager.Instance.ShowNoSpace(false);
            // Debug.Log("highlightedObjects.Count == 0");



            // showOnce = true;
            UIManager.Instance.ShowBar(UIManager.Bars.E, false);

            highlightedObjects = tempList;
        }

    }


    IEnumerator ShowNoSpaceRoutine()
    {
        showNoSpaceroutine = true;
        UIManager.Instance.ShowNoSpace(true);

        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.1f);
            UIManager.Instance.AdjustNoSpaceTransparency(-25f);

            if (stopShowno)
            {
                // Debug.Log("breakkkkk");
                break;
            }

        }

        // do transition (visibility) and reset visibillity when back on


        if (!showOnce)
        {
            //Debug.Log("OFFFFFFF");



            showOnce = true;
        }

        stopShowno = false;
        UIManager.Instance.ShowNoSpace(false);
        showNoSpaceroutine = false;
    }
}
