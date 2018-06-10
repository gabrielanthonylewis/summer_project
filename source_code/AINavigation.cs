using UnityEngine;
using System.Collections;

public class AINavigation : MonoBehaviour
{
    //Serialised Fields
    [SerializeField]
    private NavigationState _NavState = NavigationState.Idle;
    [SerializeField]
    private Transform NavBoundary = null;
    [SerializeField]
    private AttackBox AttackBox = null;
    [SerializeField]
    private int minRoamTime = 5;
    [SerializeField]
    private int maxRoamTime = 15;

    //References
    private NavMeshAgent _NavAgent;
    private Collider _Collider;
    private AudioSource _AudioSource;

    //Public
    public AudioClip HowlClip = null;

    //Private
    private bool countdown = false;
    private bool stopFleeingIE = false;
    private bool attackIE = false;
    private bool waitthenhowlIE = false;
    private Vector3 _halfNavBoundryScale = Vector3.zero;

    private enum NavigationState { Idle, PreparetoRoam, Roaming, PreparetoFlee, Fleeing, PrepareAttack, Attacking };

    private float _OriginalSpeed = 0f;
    private float _OriginalAnglularSpeed = 0f;

    private int layerMask = 1 << 9;

    private Transform _Target = null;

    void Awake()
    {
        _NavAgent = this.GetComponent<NavMeshAgent>();
        _Collider = this.GetComponent<Collider>();
        _AudioSource = this.GetComponent<AudioSource>();

        _OriginalSpeed = _NavAgent.speed;
        _OriginalAnglularSpeed = _NavAgent.angularSpeed;

        _halfNavBoundryScale = NavBoundary.localScale / 2f;

        _NavState = NavigationState.PreparetoRoam;
    }

    void Update()
    {
        if (HowlClip != null && _AudioSource != null)
        {
            if (waitthenhowlIE == false && _NavState != NavigationState.Attacking && _NavState != NavigationState.PrepareAttack)
            {
                // Debug.Log("INIT CALL!");
                StartCoroutine("WaitThenHowl");
            }
        }

        switch (_NavState)
        {
            case NavigationState.Idle:
                break;

            case NavigationState.PreparetoRoam:

                ClearStopAllRoutines();

                _NavAgent.speed = _OriginalSpeed;
                _NavAgent.angularSpeed = _OriginalAnglularSpeed;

                _NavAgent.SetDestination(GetRandomDestination());

                _NavState = NavigationState.Roaming;
                break;

            case NavigationState.Roaming:

                // check every 0.5 second?
                if (_NavAgent.remainingDistance == 0f)
                {
                    _NavState = NavigationState.PreparetoRoam;
                    break;
                }

                if (!countdown)
                    StartCoroutine("CountDown");

                break;

            case NavigationState.PreparetoFlee:

                ClearStopAllRoutines();

                _NavAgent.speed = _OriginalSpeed * 6F;
                _NavAgent.angularSpeed = _OriginalAnglularSpeed * 2F;

                _NavAgent.SetDestination(GetRandomDestination());

                _NavState = NavigationState.Fleeing;
                break;

            case NavigationState.Fleeing:

                // check every 0.5 second?
                if (_NavAgent.remainingDistance == 0f)
                {
                    _NavState = NavigationState.PreparetoFlee;
                    break;
                }

                if (!countdown)
                    StartCoroutine("CountDown");

                if (!stopFleeingIE)
                    StartCoroutine("StopFleeing");

                break;

            case NavigationState.PrepareAttack:

                ClearStopAllRoutines();

                if (_Target == null)
                {
                    _NavState = NavigationState.PreparetoRoam;
                    break;
                }

                _NavAgent.speed = _OriginalSpeed * 5F;
                _NavAgent.angularSpeed = _OriginalAnglularSpeed * 10f;
                _NavAgent.SetDestination(_Target.position);

                // check every 0.5 second?
                if (Vector3.Distance(this.transform.position, _Target.transform.position) > 15f)
                {
                    StopAttack();
                    break;
                }

                if (Vector3.Distance(this.transform.position, _Target.transform.position) <= 3f)
                    _NavState = NavigationState.Attacking;

                break;

            case NavigationState.Attacking:

                if (_NavAgent.remainingDistance > 15f)
                {
                    StopAttack();
                    break;
                }

                if (!attackIE)
                    StartCoroutine("Attack");

                break;
        }
    }

    IEnumerator WaitThenHowl()
    {
        waitthenhowlIE = true;


        float rand = Random.Range(20, 60);


        yield return new WaitForSeconds(rand);

        PlayHowl();

        waitthenhowlIE = false;
    }

    private void PlayHowl()
    {


        if (_AudioSource.clip == HowlClip && _AudioSource.isPlaying)
        {
            waitthenhowlIE = false;
            StopCoroutine("WaitThenHowl");
            StartCoroutine("WaitThenHowl");

            Debug.Log("return!");
            return;

        }

        _AudioSource.clip = HowlClip;
        _AudioSource.Play();

        // Debug.Log("play...");

    }

    private void ClearStopAllRoutines()
    {
        StopCoroutine("Attack");
        StopCoroutine("CountDown");
        StopCoroutine("StopFleeing");
        stopFleeingIE = false;
        countdown = false;
        attackIE = false;
    }

    public void PrepareToFlee()
    {
        _NavState = NavigationState.PreparetoFlee;
        return;
    }

    public void PrepareToAttack(Transform target)
    {
        if ((_Target == target && _NavState == NavigationState.PrepareAttack || _NavState == NavigationState.Attacking)
            || _NavState == NavigationState.Fleeing || _NavState == NavigationState.PreparetoFlee) return;

        _Target = target;
        _NavState = NavigationState.PrepareAttack;
        return;
    }

    public void StopAttack()
    {
        if (_NavState != NavigationState.PrepareAttack && _NavState != NavigationState.Attacking) return;

        _Target = null;
        _NavState = NavigationState.PreparetoRoam;
    }

    IEnumerator Attack()
    {
        attackIE = true;

     

        _NavAgent.ResetPath();

        // play animation;
        yield return new WaitForSeconds(1.2f);

        if (_NavState != NavigationState.Attacking)
        {
            Debug.LogError("THIS SHOULD RETURN, I DIDNT WANT THIS TO HAPPEN though..");
            attackIE = false;
            yield return null;

        }

        // if still infront then damamge.
        if (_Target != null)
        {
            if (AttackBox.TargetWithinAttackBox(_Target))
            {
                if (!_Target.GetComponent<ObjectHealth>())
                    Debug.LogError("COULDNT FIND OBJECT HEALTH..");

                //Debug.Log("REDUCE");

                _Target.GetComponent<ObjectHealth>().ReduceHealth(35f, this.transform);
            }
        }

        // END OF ANIMATION
        yield return new WaitForSeconds(0.5f);

        if (_NavState != NavigationState.Attacking)
        {
            attackIE = false;
            yield return null;

        }
        else
            _NavState = NavigationState.PrepareAttack;

        attackIE = false;
    }

    IEnumerator CountDown()
    {
        countdown = true;

        int randomSeconds = Random.Range(minRoamTime, maxRoamTime);

        yield return new WaitForSeconds(randomSeconds);

        if (countdown == false)
            Debug.Log("WOWOWO");

        _NavState = NavigationState.PreparetoRoam;

        countdown = false;

    }

    IEnumerator StopFleeing()
    {
        stopFleeingIE = true;

        yield return new WaitForSeconds(15f);

        _NavState = NavigationState.PreparetoRoam;

        stopFleeingIE = false;
    }

    private Vector3 GetRandomDestination()
    {
        Vector3 lclDestination = NavBoundary.position;
        lclDestination.x += Random.Range(-_halfNavBoundryScale.x, _halfNavBoundryScale.x);
        lclDestination.y = (_Collider.bounds.size.y); // 2f
        lclDestination.z += Random.Range(-_halfNavBoundryScale.z, _halfNavBoundryScale.z);

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(lclDestination.x, lclDestination.y + 100f, lclDestination.z), Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            lclDestination.y += hit.point.y;
        }

        return lclDestination;
    }
}
