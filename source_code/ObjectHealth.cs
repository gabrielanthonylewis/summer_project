using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectHealth : MonoBehaviour
{

    public float TotalHealth = 100;
    public float currentHealth;

    public GameObject Log = null;
    public List<GameObject> FireWoodSegments = new List<GameObject>();

    [SerializeField]
    private List<Transform> _Meat = new List<Transform>();
    [SerializeField]
    private List<Transform> _Fur = new List<Transform>();

    public int fleeingChance = 10;

    public bool isAlive = true;

    private ID _ID = null;
    private AINavigation _AINavigation = null;

    void Awake()
    {
        currentHealth = TotalHealth;
        if (currentHealth <= 0f)
            isAlive = false;


    }

    void Start()
    {
        _ID = this.GetComponent<ID>();
        _AINavigation = this.GetComponent<AINavigation>();
    }


    public void ReduceHealth(float ammount, Transform attacker)
    {
        if (isAlive == false) return;

        if (this.tag == "Player")
        {
            UIManager.Instance.AdjustBar(-ammount, UIManager.Bars.Health);
            return;
        }

        currentHealth -= ammount;

        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth > TotalHealth) currentHealth = TotalHealth;

        if (currentHealth == 0) Death(attacker);

        if (_ID)
        {
            if (_ID._LifeForm != ID.Lifeform.NULL)
            {
                // either run or attack depending on predators

                if (currentHealth <= 20f)
                {
                    int chance = Random.Range(0, 100);
                    if (chance <= fleeingChance)
                    {
                        _AINavigation.PrepareToFlee();
                        return;
                    }
                }

                // TEMP
                if (_ID._LifeForm == ID.Lifeform.Bear || _ID._LifeForm == ID.Lifeform.Wolf)
                {
                    _AINavigation.PrepareToAttack(attacker);
                }
                else if (_ID._LifeForm == ID.Lifeform.Rabbit)
                {
                    _AINavigation.PrepareToFlee();
                }
            }
        }

    }
    public void StabToGet()
    {
        if (_Meat.Count > 0)
        {
            for (int i = 0; i < _Meat.Count; i++)
            {
                if (_Meat[i] != null)
                {
                    if (_Meat[i].gameObject.activeSelf == false)
                    {
                        _Meat[i].gameObject.SetActive(true);
                        _Meat[i] = null;
                        return;
                    }
                }

            }
        }

        if (_Fur.Count > 0)
        {
            for (int i = 0; i < _Fur.Count; i++)
            {
                if (_Fur[i] != null)
                {
                    if (_Fur[i].gameObject.activeSelf == false)
                    {
                        _Fur[i].gameObject.SetActive(true);
                        _Fur[i] = null;
                        return;
                    }
                }
            }
        }

    }

    private void Death(Transform attacker)
    {
        isAlive = false;

        if (this.transform.tag == "Player")
        {
            //   GameManager.Instance.Die();
            return;
        }



        if (!_ID) return;

        if (_ID._LifeForm != ID.Lifeform.NULL)
        {
            if (_AINavigation)
                _AINavigation.enabled = false;

            if (this.GetComponent<NavMeshAgent>())
                this.GetComponent<NavMeshAgent>().enabled = false;

            if (this.GetComponent<FishAI>())
                this.GetComponent<FishAI>().enabled = false;



            if (attacker.transform.tag == "Player")
            {
                if (this.GetComponent<Consumable>())
                {
                    Camera.main.GetComponent<Inventory>().AddDeleteItem(ID.Specification.Rabbit, ID.Type.Consumable, 0, this.transform, ID.SubMenu.NULL, null);
                }

            }
            else
            {
                // IF THIS IS PREY / objects predator is this.. Eat!

                Debug.Log("Eaten!");
                Destroy(this.gameObject);

                // IF not eaten do this!

                if (this.GetComponent<Rigidbody>())
                {
                    this.GetComponent<Rigidbody>().isKinematic = false;
                    this.GetComponent<Rigidbody>().useGravity = true;
                }

                for (int j = 0; j < _ID.Colliders.Length; j++)
                {
                    _ID.Colliders[j].enabled = true;
                }

                if (this.GetComponent<Consumable>())
                    this.GetComponent<Consumable>().enabled = false;

                this.gameObject.layer = 8;
            }


            //this.name = _Slots[i].item.prefab.name;

            Debug.Log("dead");
            return;
        }


        if (_ID._StaticTag == ID.StaticTag.Tree)
        {
            GameObject Log1 = Instantiate(Log, this.transform.position, this.transform.rotation) as GameObject;
            Log1.name = Log.name;
            GameObject Log2 = Instantiate(Log, this.transform.position + transform.up * 0.85f - transform.right * 0.4f, this.transform.rotation) as GameObject;
            Log2.name = Log.name;
            GameObject Log3 = Instantiate(Log, this.transform.position - transform.up * 0.85f + transform.right * 0.4f, this.transform.rotation) as GameObject;
            Log3.name = Log.name;
            Destroy(this.gameObject);
        }
        else
            if (_ID.specification == ID.Specification.Log)
            {
                for (int i = 0; i < FireWoodSegments.Count; i++)
                {
                    FireWoodSegments[i].transform.parent = null;
                    FireWoodSegments[i].AddComponent<Rigidbody>();

                    FireWoodSegments[i].GetComponent<ID>().Rigidbodies = new Rigidbody[FireWoodSegments[i].GetComponent<ID>().Rigidbodies.Length + 1];
                    FireWoodSegments[i].GetComponent<ID>().Rigidbodies[0] = FireWoodSegments[i].GetComponent<Rigidbody>();

                    FireWoodSegments[i].GetComponent<Rigidbody>().useGravity = true;
                    FireWoodSegments[i].GetComponent<Rigidbody>().isKinematic = false;
                    FireWoodSegments[i].layer = 8;
                }
                Destroy(this.gameObject);
            }
    }
}
