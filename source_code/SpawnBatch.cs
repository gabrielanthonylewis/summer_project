using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBatch : MonoBehaviour
{

    private List<GameObject> _TreePrefabs = new List<GameObject>();
    private List<GameObject> _StonePrefab = new List<GameObject>();
    private List<GameObject> _PondPrefab = new List<GameObject>();
    private GameObject _CabinPrefab = null;
    private List<GameObject> _BoulderPrefab = new List<GameObject>();
    private List<GameObject> _BushPrefab = new List<GameObject>();
    private List<GameObject> _BerryBushPrefab = new List<GameObject>();
    private List<GameObject> _LongStickPrefab = new List<GameObject>();
    private List<GameObject> _MushroomPatchPrefab = new List<GameObject>();
    private List<GameObject> _RabbitHabitatPrefab = new List<GameObject>();
    private List<GameObject> _BearHabitatPrefab = new List<GameObject>();
    private List<GameObject> _BerryBushPoisonPrefab = new List<GameObject>();
    private List<GameObject> _WolfHabitatPrefab = new List<GameObject>();
    private List<GameObject> _WolfCavePrefab = new List<GameObject>();
    private List<GameObject> _BearCavePrefab = new List<GameObject>();

    public int StoneAmmount = 3;
    public int TreeAmmount = 3;
    public int WaterAmmount = 3;
    public int CabinAmmount = 1;
    public int BoulderAmmount = 0;
    public int BerryBushAmmount = 0;
    public int BushAmmount = 0;
    public int LongStickAmount = 0;
    public int MushroomPatchAmount = 0;
    public int RabbitHabitatAmount = 0;
    public int BearHabitatAmount = 0;
    public int BerryBushPoisonAmmount = 0;
    public int WolfHabitatAmount = 0;
    public int WolfCaveAmount = 0;
    public int BearCaveAmount = 0;

    int layerMask = 1 << 9;

    // Editor script that auto adds structs of each item with a bool == true. Contains ammount and prefab. You see ammount?


    public Initialise _Initialise = null;
    Color Color1 = Color.white;
    Color Color2 = Color.white;
    void Awake()
    {


        GameObject[] StaticPrefabs = Resources.LoadAll<GameObject>("StaticPrefabs");
        for (int i = 0; i < StaticPrefabs.Length; i++)
        {
            switch (StaticPrefabs[i].GetComponent<ID>()._StaticTag)
            {
                case ID.StaticTag.Tree:
                    _TreePrefabs.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.Water:
                    _PondPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.Cabin:
                    _CabinPrefab = StaticPrefabs[i];
                    break;

                case ID.StaticTag.Boulder:
                    _BoulderPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.BerryBush:
                    _BerryBushPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.Bush:
                    _BushPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.MushroomPatch:
                    _MushroomPatchPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.RabbitHabitat:
                    _RabbitHabitatPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.BerryBushPoison:
                    _BerryBushPoisonPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.BearHabitat:
                    _BearHabitatPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.WolfHabitat:
                    _WolfHabitatPrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.CaveWolf:
                    _WolfCavePrefab.Add(StaticPrefabs[i]);
                    break;

                case ID.StaticTag.CaveBear:
                    _BearCavePrefab.Add(StaticPrefabs[i]);
                    break;
            }

        }

        GameObject[] Prefabs = Resources.LoadAll<GameObject>("Prefabs");
        for (int i = 0; i < Prefabs.Length; i++)
        {

            if (Prefabs[i].GetComponent<ID>().Compare(ID.Specification.Stone, ID.Type.Weapon, 0))
            {
                _StonePrefab.Add(Prefabs[i]);
            }
            if (Prefabs[i].GetComponent<ID>().Compare(ID.Specification.LongStick, ID.Type.Weapon, 0))
            {
                _LongStickPrefab.Add(Prefabs[i]);
            }
        }

    }

    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < CabinAmmount; i++)
        {
            if (_CabinPrefab == null) return;
            SpawnObject(_CabinPrefab, false, false, true, false);
        }

        for (int i = 0; i < BoulderAmmount; i++)
        {
            if (_BoulderPrefab.Count == 0) return;
            int index = Random.Range(0, _BoulderPrefab.Count);
            SpawnObject(_BoulderPrefab[index], true, true, true, false);
        }

        for (int i = 0; i < WolfCaveAmount; i++)
        {
            if (_WolfCavePrefab.Count == 0) return;
            int index = Random.Range(0, _WolfCavePrefab.Count);
            SpawnObject(_WolfCavePrefab[index], true, false, true, false);
        }

        for (int i = 0; i < BearCaveAmount; i++)
        {
            if (_BearCavePrefab.Count == 0) return;
            int index = Random.Range(0, _BearCavePrefab.Count);
            SpawnObject(_BearCavePrefab[index], true, false, true, false);
        }

        Color1 = _Initialise.Color1;
        Color2 = _Initialise.Color2;



        // RANDOM.RANGE A COLOUR OR RANDOMLY PICK A INDEX OF A SET OF COLOURS,
        // DO THIS TWICE
        // AND SET ONE TO BERRY BUSH
        // SET OTHER TO BERRY BUSH POISON.

        for (int i = 0; i < BerryBushAmmount; i++)
        {
            if (_BerryBushPrefab.Count == 0) return;
            int index = Random.Range(0, _BerryBushPrefab.Count);
            SpawnObject(_BerryBushPrefab[index], true, false, true, false);
        }

        for (int i = 0; i < BerryBushPoisonAmmount; i++)
        {
            if (_BerryBushPoisonPrefab.Count == 0) return;
            int index = Random.Range(0, _BerryBushPoisonPrefab.Count);
            SpawnObject(_BerryBushPoisonPrefab[index], true, false, true, false);
        }

        for (int i = 0; i < BushAmmount; i++)
        {
            if (_BushPrefab.Count == 0) return;
            int index = Random.Range(0, _BushPrefab.Count);
            SpawnObject(_BushPrefab[index], true, false, true, false);
        }

        for (int i = 0; i < LongStickAmount; i++)
        {
            if (_LongStickPrefab.Count == 0) return;
            int index = Random.Range(0, _LongStickPrefab.Count);
            SpawnObject(_LongStickPrefab[index], false, true, true, true);
        }

        for (int i = 0; i < StoneAmmount; i++)
        {
            if (_StonePrefab.Count == 0) return;
            int index = Random.Range(0, _StonePrefab.Count);
            SpawnObject(_StonePrefab[index], true, true, true, false);
        }


        for (int i = 0; i < MushroomPatchAmount; i++)
        {
            if (_MushroomPatchPrefab.Count == 0) return;
            int index = Random.Range(0, _MushroomPatchPrefab.Count);
            SpawnObject(_MushroomPatchPrefab[index], false, true, true, false);
        }

        for (int i = 0; i < WaterAmmount; i++)
        {
            if (_PondPrefab.Count == 0) return;
            int index = Random.Range(0, _PondPrefab.Count);
            SpawnObject(_PondPrefab[index], true, true, true, false);
        }
        for (int i = 0; i < RabbitHabitatAmount; i++)
        {
            if (_RabbitHabitatPrefab.Count == 0) return;
            int index = Random.Range(0, _RabbitHabitatPrefab.Count);
            SpawnObject(_RabbitHabitatPrefab[index], true, true, false, false);
        }
        for (int i = 0; i < BearHabitatAmount; i++)
        {
            if (_BearHabitatPrefab.Count == 0) return;
            int index = Random.Range(0, _BearHabitatPrefab.Count);
            SpawnObject(_BearHabitatPrefab[index], true, true, false, false);
        }
        for (int i = 0; i < WolfHabitatAmount; i++)
        {
            if (_WolfHabitatPrefab.Count == 0) return;
            int index = Random.Range(0, _WolfHabitatPrefab.Count);
            SpawnObject(_WolfHabitatPrefab[index], true, true, false, false);
        }

        for (int i = 0; i < TreeAmmount; i++)
        {
            if (_TreePrefabs.Count == 0) return;
            int index = Random.Range(0, _TreePrefabs.Count);
            SpawnObject(_TreePrefabs[index], false, true, true, false);
        }

        this.GetComponent<Collider>().enabled = false;
    }

    void SpawnObject(GameObject prefab, bool getChild, bool ignoreCommonCollision, bool dontCollide, bool getChildofChild)
    {
        GameObject newObject = Instantiate(prefab, transform.position, prefab.transform.rotation) as GameObject;

        RaycastHit hit, hit2;

        Vector3 newPos = newObject.transform.position;
        //newPos.y += -0.2f;
        newPos.x += Random.Range((-transform.localScale.x / 2f) + newObject.transform.localScale.x, (transform.localScale.x / 2f) - newObject.transform.localScale.x);
        newPos.z += Random.Range((-transform.localScale.z / 2f) + newObject.transform.localScale.z, (transform.localScale.z / 2f) - newObject.transform.localScale.z);


        // if ignoreSelf = true allow spawn
        // otherwise choose another position until placed == true?
        // USE RAYCAST WITHOUT LAYERMASK

        bool placeFound = false;
        if (dontCollide)
        {
            for (int i = 0; i < 5; i++)
            {
                newPos = newObject.transform.position;
                //newPos.y += -0.2f;
                newPos.x += Random.Range((-transform.localScale.x / 2f) + newObject.transform.localScale.x, (transform.localScale.x / 2f) - newObject.transform.localScale.x);
                newPos.z += Random.Range((-transform.localScale.z / 2f) + newObject.transform.localScale.z, (transform.localScale.z / 2f) - newObject.transform.localScale.z);

                // layermask that covers everything EXCEPT IGNORE (which will be assigned to all spawners etc).
                // This will solve it once and for all.
                if (Physics.Raycast(new Vector3(newPos.x, newPos.y + 100f, newPos.z), Vector3.down, out hit2, Mathf.Infinity))
                {
                    if (hit2.transform.gameObject.layer == 9)
                    {
                        placeFound = true;
                        break;
                    }

                    if (ignoreCommonCollision)
                    {
                        // if match
                        //Debug.Log(hit2.transform.name + "::::::::::::");
                        if (hit2.transform.GetComponent<ID>())
                        {
                            if (hit2.transform.GetComponent<ID>()._StaticTag == newObject.GetComponent<ID>()._StaticTag)
                            {
                                if (newObject.GetComponent<ID>()._StaticTag == ID.StaticTag.NULL) continue;
                                // Place on terrain
                                //Debug.Log(":) MATCH :)");
                                placeFound = true;
                                break;
                            }

                        }
                        else
                            if(hit2.transform.parent.GetComponent<ID>())
                            {
                                if (hit2.transform.parent.GetComponent<ID>()._StaticTag == newObject.GetComponent<ID>()._StaticTag)
                                {
                                    if (newObject.GetComponent<ID>()._StaticTag == ID.StaticTag.NULL) continue;
                                    // Place on terrain
                                    //Debug.Log(":) MATCH PARENT :)");
                                    placeFound = true;
                                    break;
                                }
                            }
                    }

                }


            }


        }
        else
        {
            placeFound = true;
        }

        if (placeFound == false)
        {
            Debug.Log("Destroy0!");
            Destroy(newObject.gameObject);
            return;
        }

        if (Physics.Raycast(new Vector3(newPos.x, newPos.y + 100f, newPos.z), Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            if (getChild)
                newPos.y = hit.point.y + (newObject.transform.GetChild(0).GetComponent<Collider>().bounds.size.y / 2f);
            else if(getChildofChild)
                 newPos.y = hit.point.y + (newObject.transform.GetChild(0).GetChild(0).GetComponent<Collider>().bounds.size.y / 2f);
            else
            {
                if (prefab.GetComponent<ID>()._StaticTag == ID.StaticTag.Cabin)
                {
                    newPos.y = hit.point.y + (newObject.GetComponent<Collider>().bounds.size.y / 4f);
                }
                else
                {
                    if(newObject.GetComponent<ID>()._StaticTag == ID.StaticTag.Water)
                    {
                        Debug.Log("HitY: " + hit.point.y + " :::: ColY " + (newObject.GetComponent<Collider>().bounds.size.y / 2f) + " :::: = " + (hit.point.y + (newObject.GetComponent<Collider>().bounds.size.y / 2f)).ToString() );
                    }
                    newPos.y = hit.point.y + (newObject.GetComponent<Collider>().bounds.size.y / 2f);
                }

            }
        }
        else
        {
            Debug.Log("Destroy1!");
            Destroy(newObject.gameObject);
            return;
        }

        //newPos.y -= 0.02f;
        newObject.transform.position = newPos;

        if (newObject.GetComponent<ID>().specification == ID.Specification.Stone
            || newObject.GetComponent<ID>()._StaticTag == ID.StaticTag.Bush
            || newObject.GetComponent<ID>().specification == ID.Specification.LongStick
            )
        {
            Vector3 newRot = newObject.transform.GetChild(0).localRotation.eulerAngles;
            newRot.y = Random.Range(0F, 355F);
            newObject.transform.GetChild(0).localRotation = Quaternion.Euler(newRot);
        }

        newObject.transform.SetParent(this.transform);
        newObject.name = prefab.name;

        if (newObject.GetComponent<ID>()._StaticTag == ID.StaticTag.BerryBush)
        {
            for (int i = 0; i < newObject.transform.childCount - 1; i++)
            {
                for (int j = 0; j < newObject.transform.GetChild(i + 1).childCount; j++)
                {
                    newObject.transform.GetChild(i + 1).GetChild(j).GetComponent<Renderer>().material.color = Color1;
                }

            }
        }
        if (newObject.GetComponent<ID>()._StaticTag == ID.StaticTag.BerryBushPoison)
        {
            for (int i = 0; i < newObject.transform.childCount - 1; i++)
            {
                for (int j = 0; j < newObject.transform.GetChild(i + 1).childCount; j++)
                {
                    newObject.transform.GetChild(i + 1).GetChild(j).GetComponent<Renderer>().material.color = Color2;
                }

            }

        }

    }
}
