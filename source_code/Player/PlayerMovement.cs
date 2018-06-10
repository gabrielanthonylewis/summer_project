using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private float _VerticalSpeed = 6f;
    [SerializeField]
    private float _HorizontalSpeed = 6f;
    [SerializeField]
    private float _RunningMulti = 1.5f;

    [SerializeField]
    private bool _isJumping = false;

    private float energy = 100f;

    //[SerializeField]
    //private float currentEnergy = 100f;

    private float reductionValue = 0.1f;


    public bool allowRegen = false;
    private bool regenRoutine = false;
    private bool isRegenFromZero = false;
    private bool waitBool = false;

    void Update()
    {



        float hMulti = 1f;
        float vMulti = 1f;


       
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // new size
            this.transform.localScale = new Vector3(1f, 0.75f, 1f);
        }
        else
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                // new size
                this.transform.localScale = new Vector3(1f, 0.25f, 1f);
            }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            hMulti = 0.5f;
            vMulti = 0.5f;
          
        }
        else
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                hMulti = 0.1f;
                vMulti = 0.1f;
            }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // return to original size
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                // return to original size
                this.transform.localScale = new Vector3(1f, 1f, 1f);
            }

        float horizontal = Input.GetAxis("Horizontal") * _HorizontalSpeed * hMulti;
        float vertical = Input.GetAxis("Vertical") * _VerticalSpeed * vMulti;

        // Moving decrease and idle decrease

        if ((horizontal != 0f || vertical != 0f) && !Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("!");

            UIManager.Instance.AdjustBar(-reductionValue * 2f * Time.deltaTime, UIManager.Bars.Food);
            UIManager.Instance.AdjustBar(-reductionValue * 2f * Time.deltaTime, UIManager.Bars.Hydration);
        }

        else if (horizontal == 0f && vertical == 0f)
        {
            //Debug.Log("-");

            UIManager.Instance.AdjustBar(-reductionValue * Time.deltaTime / 1.2f, UIManager.Bars.Food);
            UIManager.Instance.AdjustBar(-reductionValue * Time.deltaTime / 1.2f, UIManager.Bars.Hydration);
        }

        float runningMulti = 1f;

        /*
        if (Input.GetKey(KeyCode.LeftShift))
        {
            UIManager.Instance.ShowBar(UIManager.Bars.Sprint, true);
        }*/

        if (Input.GetKeyUp(KeyCode.LeftShift) && isRegenFromZero)
        {
            isRegenFromZero = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && !_isJumping && UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue > 0f && !isRegenFromZero)
        {
            allowRegen = false;
            //Debug.Log("grgt");
            runningMulti = _RunningMulti;

            UIManager.Instance.AdjustBar(-reductionValue * 400f * Time.deltaTime, UIManager.Bars.Sprint);
            UIManager.Instance.AdjustBar(-reductionValue * 10f * Time.deltaTime, UIManager.Bars.Food);
            UIManager.Instance.AdjustBar(-reductionValue * 10f * Time.deltaTime, UIManager.Bars.Hydration);
        }


        transform.Translate(new Vector3(horizontal, 0f, 0f) * runningMulti * Time.deltaTime);
        transform.Translate(new Vector3(0, 0f, vertical) * runningMulti * Time.deltaTime);



        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E))
        {
            if (Camera.main.GetComponent<Inventory>().GetPrimaryObject() != null && UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue > 0)
            {
                if (!waitBool)
                    StartCoroutine(Wait(0.475f));
            }
        }

        if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue < UIManager.Instance.GetBar(UIManager.Bars.Sprint).totalValue)
        {
            if (UIManager.Instance.GetBar(UIManager.Bars.Food).currentValue > 0f)
            {
                if (waitBool)
                {
                   // Debug.Log("WAIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
                else
                    if (isRegenFromZero)
                    {

                        allowRegen = true;
                        StartCoroutine(WaitThenRegen(1F));

                    }
                    else
                        if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue <= 0F)
                        {
                            // if (allowRegen == false)
                            //{
                            Debug.Log("GO");

                            isRegenFromZero = true;
                            allowRegen = true;
                            StartCoroutine(WaitThenRegen(1F));

                            // }

                        }
                        else
                            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.E))     // NEED TO CHECK IF PRIMARY SWUNG..NOT JUST MOUSE0
                            {


                                // if (allowRegen == false)
                                // {

                                //Debug.Log("CALLED!!!REGEN1");
                                allowRegen = true;      // THIS HAPPENDS MULTIPLE TIMES UNLESS ALLOWREGEN = TRUE BUT THEN CANT INTERUPT. SO NEW BOOL MAYBE?
                                StartCoroutine(WaitThenRegen(0.5f));
                                // }



                            }

            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
        {
            this.GetComponent<Rigidbody>().WakeUp();


            if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue <= 0f)
            {
                this.GetComponent<Rigidbody>().AddForce(this.transform.up * 100f, ForceMode.Acceleration); // *_RunningMulti when that is here
            }
            else
                this.GetComponent<Rigidbody>().AddForce(this.transform.up * 200f, ForceMode.Acceleration); // *_RunningMulti when that is here //* Time.fixedDeltaTime

            UIManager.Instance.AdjustBar(-20f, UIManager.Bars.Sprint);

            _isJumping = true;
        }
    }




    void OnTriggerEnter(Collider other)
    {
        if (other.transform.position.y < this.transform.position.y)
        {
            _isJumping = false;
        }
    }


    IEnumerator Wait(float period)
    {
        waitBool = true;
        allowRegen = false;
        yield return new WaitForSeconds(period);
        allowRegen = true;
        waitBool = false;
    }

    IEnumerator WaitThenRegen(float period)
    {

        yield return new WaitForSeconds(period);//0.3f

        // NEED TO CHECK IF PRIMARY SWUNG..

        if (!regenRoutine && !waitBool)
            StartCoroutine("Regen");



    }

    IEnumerator Regen()
    {

        // Debug.Log("REGEN");
        regenRoutine = true;
        // INSTEAD COULD DO *rate. RATE DEPENDS ON IF 0 or maybe on ammount.
        /*if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue == 0f)
        {
            yield return new WaitForSeconds(1f);
        }else*/
        yield return new WaitForSeconds(0.1F);

        regenRoutine = false;
        if (waitBool)
        {
            StopCoroutine("Regen");

        }
        else
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E)) && (!allowRegen || waitBool))
            {
                //Debug.Log("LSHIFT");
                StopCoroutine("Regen");


            }
            else
                if (_isJumping)
                {
                    // Debug.Log("JUMPING");
                    StartCoroutine("Regen");
                }
                else
                    if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue >= energy)
                    {
                        //Debug.Log(">=energy");
                        allowRegen = false;
                        isRegenFromZero = false;
                        StopCoroutine("Regen");
                    }
                    else
                        if (UIManager.Instance.GetBar(UIManager.Bars.Sprint).currentValue + 10f > energy)
                        {
                            // currentEnergy = energy;
                            // Debug.Log(">energry");
                            UIManager.Instance.AdjustBar(100f, UIManager.Bars.Sprint);
                            allowRegen = false;
                            isRegenFromZero = false;
                            StopCoroutine("Regen");
                        }
                        else
                        {
                            //Debug.Log("REGEN+++++10");
                            //Debug.Log("hey");
                            UIManager.Instance.AdjustBar(10F, UIManager.Bars.Sprint);
                            //  StartCoroutine("Regen");
                        }
    }

}
