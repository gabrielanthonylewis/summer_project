using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirePlace : MonoBehaviour
{

    public bool isLit = false;

    public ParticleSystem Fire;
    public ParticleSystem Glow;
    public ParticleSystem Smoke;
    public ParticleSystem SmokeLit;
    public Light _Light;
    public InitialiseAudioClip _InitAudioClip;

    private ID _ID = null;

    [SerializeField]
    private float currFlameLifeTime = 100f;
    public float totalFlameLifeTime = 100f;

    bool extinguishRoutine = false;
    bool warmthRoutine = false;
    bool waitLogRoutine = false;

    public bool noWarmth = false;
    public bool destroyOnExtinguish = false;
    private bool warmthRoutinenew = false;
    public int totalLogLifeTime = 30;
    public float originalIntensity = 3.5f;
    public float originalRange = 0f;

    [SerializeField]
    private List<Transform> _EntPlayers = new List<Transform>();


    [SerializeField]
    private List<Transform> _Sticks = new List<Transform>();
    [SerializeField]
    private List<Transform> _LongSticks = new List<Transform>();
  
    [SerializeField]
    private List<Transform> _Logs = new List<Transform>();
    [SerializeField]
    public int[] _LogDuration;
    [SerializeField]
    public bool[] _LogDecreasing;
    [SerializeField]
    private bool passiveBurnOut = true;

    bool allSticks = false;
    bool allLongSticks = false;

    int noOfFireWood = 0;

    void Awake()
    {
        if (this.GetComponent<ID>()) _ID = this.GetComponent<ID>();
        else if (this.GetComponentInParent<ID>()) _ID = this.GetComponentInParent<ID>();
        else if (this.GetComponentInChildren<ID>()) _ID = this.GetComponentInChildren<ID>();
        else Debug.LogError("_ID not found");
       
        
        currFlameLifeTime = totalFlameLifeTime;

        if (_Light)
        {
            originalIntensity = _Light.intensity;
            originalRange = _Light.range;
        }
        _LogDuration = new int[_Logs.Count];
        _LogDecreasing = new bool[_Logs.Count];

        for (int i = 0; i < _LogDuration.Length; i++)
        {
            _LogDuration[i] = totalLogLifeTime;
            _LogDecreasing[i] = false;
        }

        if (isLit)
            LightFire();
    }

    void OnEnable()
    {
        extinguishRoutine = false;
    }

    public void EnableWarmth(bool val)
    {
        if(val == true)
        {
            noWarmth = false;
          //  transform.GetComponent<Collider>().enabled = true;

            
        }else
        if(val == false)
        {
         //   transform.GetComponent<Collider>().enabled = false;

            noWarmth = true;

            for (int i = 0; i < _EntPlayers.Count; i++)
            {
                _EntPlayers[i].transform.GetComponent<Stats>().playerWithinWarmth = false;
            }
           // _EntPlayers.Clear();
               
        }
    }

    void Update()
    {

        if (!passiveBurnOut) return;

        if (!extinguishRoutine && isLit)
        {
            StartCoroutine("WaitThenExtinguish");
        }
    }

    public void AdjustCurrentLifetime(float value)
    {
        currFlameLifeTime += value;
        if (currFlameLifeTime < 0)
        {
            currFlameLifeTime = 0;
            ExtinguishFire();
        }

        if (currFlameLifeTime > totalFlameLifeTime)
            currFlameLifeTime = totalFlameLifeTime;
    }

    public bool LightFire()
    {

        if (_Logs.Count <= 0)
        {
            if (Fire)
                Fire.enableEmission = true;
            isLit = true;

            if (this.GetComponent<Collider>())
                this.GetComponent<Collider>().enabled = true;

            if (Fire)
                Fire.Play();
            if (Glow)
                Glow.Play();
            if (Smoke)
                Smoke.Play();
            if (SmokeLit)
                SmokeLit.Play();
            if (_InitAudioClip)
                _InitAudioClip.Play();

            if (_Light)
                _Light.enabled = true;

            if (waitLogRoutine == false)
                StartCoroutine("WaitThenDestroyLog");

            return true;
        }
        else
        {

            for (int i = _Logs.Count - 1; i > -1; i--)
            {
                
                if (_Logs[i].gameObject.activeSelf == true)
                {
                    Debug.Log("iiii: " + i);

                    if (Fire)
                        Fire.enableEmission = true;
                    isLit = true;

                    if (this.GetComponent<Collider>())
                        this.GetComponent<Collider>().enabled = true;

                    if (Fire)
                        Fire.Play();
                    if (Glow)
                        Glow.Play();
                    if (Smoke)
                        Smoke.Play();
                    if (SmokeLit)
                        SmokeLit.Play();
                    if (_InitAudioClip)
                        _InitAudioClip.Play();

                    if (_Light)
                        _Light.enabled = true;

                    if (waitLogRoutine == false)
                        StartCoroutine("WaitThenDestroyLog");

                    return true;
                }
            }
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (noWarmth) return;
        if (!isLit) return; //
        if (other.tag != "Player") return;

        if (!_EntPlayers.Contains(other.transform))
            _EntPlayers.Add(other.transform);

        other.transform.GetComponent<Stats>().playerWithinWarmth = true;

        if (warmthRoutinenew == false)
            StartCoroutine("Warmth");

    }
    

    public bool AddStick()
    {
        for (int i = 0; i < _Sticks.Count; i++)
        {
            if (_Sticks[i].gameObject.activeSelf == false)
            {
                _Sticks[i].gameObject.SetActive(true);

                bool tempgood = true;
                for (int j = 0; j < _Sticks.Count; j++)
                {
                    if (_Sticks[j].gameObject.activeSelf == false)
                    {
                        tempgood = false;
                    }
                }

                allSticks = tempgood;

                if (allSticks && allLongSticks)
                {
                    // set cooking active
                    this.transform.parent.GetComponent<Cooking>().SetSlotsActive(true);
                }

                return true;
            }
        }

        

        return false;
    }

    public bool AddLongStick()
    {
        for (int i = 0; i < _LongSticks.Count; i++)
        {
            if (_LongSticks[i].gameObject.activeSelf == false)
            {
                _LongSticks[i].gameObject.SetActive(true);

                bool tempgood = true;
                for (int j = 0; j < _LongSticks.Count; j++)
                {
                    if (_LongSticks[j].gameObject.activeSelf == false)
                    {
                        tempgood = false;
                    }
                }

                allLongSticks = tempgood;

                if (allSticks && allLongSticks)
                {
                    // set cooking active
                    this.transform.parent.GetComponent<Cooking>().SetSlotsActive(true);
                }

                return true;
            }
        }

        

        return false;
    }


    public bool AddLog()
    {
        for (int i = 0; i < _Logs.Count; i++)
        {
            if (_Logs[i].gameObject.activeSelf == false)
            {
                _LogDuration[i] = totalLogLifeTime;
                _LogDecreasing[i] = false;
                _Logs[i].gameObject.SetActive(true);
                noOfFireWood++;


                _Light.intensity = ((originalIntensity / _Logs.Count) * noOfFireWood);
                _Light.range = ((originalRange / _Logs.Count) * noOfFireWood);

                if (i == _Logs.Count - 1)
                {
                    if (isLit)
                    {
                        bool isDec = false;
                        for (int j = 0; j < _LogDecreasing.Length; j++)
                        {
                            if (_LogDecreasing[i] == true)
                            {
                                isDec = true;
                            }


                        }
                        if (!isDec)
                        {
                            if (waitLogRoutine == false)
                                StartCoroutine("WaitThenDestroyLog");
                        }


                    }
                }
                return true;
            }
        }


        return false;
    }

    IEnumerator WaitThenDestroyLog()
    {
        waitLogRoutine = true;

        int index = -1;

        bool oneDec = false;
        for (int i = 0; i < _LogDecreasing.Length; i++)
        {
            if (_LogDecreasing[i] == true)
            {
                Debug.Log("LOGDEC");
                oneDec = true;

            }
        }

        if (!oneDec)
        {

            for (int i = _Logs.Count - 1; i > -1; i--)
            {
                if (_Logs[i].gameObject.activeSelf == true)
                {
                    index = i;
                    _LogDecreasing[i] = true;
                    break;
                }
            }
        }

        if (index == -1)
        {
           // Debug.Log("YIELD RETURN NULL");
            yield return null;
        }
        else
        {

            do
            {
                yield return new WaitForSeconds(1f);
                _LogDuration[index]--;
            }
            while (_LogDuration[index] > 0 && isLit);

            if (_LogDuration[index] <= 0)
            {
                _LogDecreasing[index] = false;
                _Logs[index].gameObject.SetActive(false);
                noOfFireWood--;

                _Light.intensity = ((originalIntensity/_Logs.Count) * noOfFireWood);
                _Light.range = ((originalRange / _Logs.Count) * noOfFireWood);
            


                bool isOne = false;
                for (int i = 0; i < _Logs.Count; i++)
                {
                    if (_Logs[i].gameObject.activeSelf == true)
                    {
                        isOne = true;

                    }
                }

                if (isOne == false)
                    ExtinguishFire();
                else
                {

                    waitLogRoutine = false;
                    StartCoroutine("WaitThenDestroyLog");
                    yield return null;

                }
            }

        }


        waitLogRoutine = false;

    }

    IEnumerator Warmth()
    {
        warmthRoutinenew = true;

        do
        {
            yield return new WaitForSeconds(1F);

            // Debug.Log("INCREASE!");
            UIManager.Instance.AdjustWarmthValue(+(30f/_Logs.Count) * noOfFireWood, true, _ID.specification);
           
        }
        while (_EntPlayers.Count > 0 && !noWarmth);

        warmthRoutinenew = false;

        yield return null;
    }

    void OnTriggerExit(Collider other)
    {
        if (noWarmth)

            return;


        if (other.tag != "Player") return;
        other.transform.GetComponent<Stats>().playerWithinWarmth = false;

        _EntPlayers.Remove(other.transform);

        if (_EntPlayers.Count <= 0)
        {
            warmthRoutinenew = false;
            StopCoroutine("Warmth");
        }

    }

    private void ExtinguishFire()
    {
        if (destroyOnExtinguish)
        {
            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject() == this.gameObject)
            {
                Debug.LogError("DestroyOnExtinguish >>> .GetPrimaryObject() == this.gameObject");
                Camera.main.GetComponent<Inventory>().RemoveItem(this.gameObject, 1);
                  Destroy(this.gameObject);
            }
            else
            {
                    Debug.LogError("DestroyOnExtinguish <<< .GetPrimaryObject() != this.gameObject");
               
                Destroy(this.gameObject);
            }
            return;
        }

        Debug.Log("EXTIN");
        if (Fire)
            Fire.enableEmission = false;
        isLit = false;

        if(this.GetComponent<Collider>())
        this.GetComponent<Collider>().enabled = false;

        if (Fire)
            Fire.Stop();

        if (Glow)
            Glow.Stop();
        if (Smoke)
            Smoke.Stop();
        if (SmokeLit)
            SmokeLit.Stop();
        if (_InitAudioClip)
            _InitAudioClip.Stop();
        if (_Light)
            _Light.enabled = false;

        //ADDED
        currFlameLifeTime = totalFlameLifeTime;
    }

    IEnumerator WaitThenExtinguish()
    {
        extinguishRoutine = true;
        yield return new WaitForSeconds(1F);
        currFlameLifeTime -= 1f;

        //Debug.Log("Intensity decrease and height and range also?");
       // _Light.intensity = 1f / (originalIntensity  currFlameLifeTime);



        // Debug.Log("-1");
        if (currFlameLifeTime <= 0f)
        {
            ExtinguishFire();

        }

        extinguishRoutine = false;
    }

}
