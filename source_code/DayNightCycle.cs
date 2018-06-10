using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{

    //public float hours = 24;
    public float realtimeMinutesInDay = 3;
    float _Interval = 0f;

    Color32 originalFogColor32;
    float eulerRotX = 0f;

    public enum DayState { NULL, PrepareDay, Day, PrepareNight, Night, PrepareDusk, Dusk, PrepareDawn, Dawn };
    public DayState _DayState = DayState.NULL;

    void Awake()
    {
        originalFogColor32 = RenderSettings.fogColor;

        _Interval = 360f / (realtimeMinutesInDay * 60f);

    }

    void Start()
    {

        if (this.transform.eulerAngles.x < 90f )
        {
            _DayState = DayState.PrepareDay;
        }

        if (this.transform.eulerAngles.x > 200f && this.transform.eulerAngles.x < 350f)
        {
            _DayState = DayState.PrepareNight;
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (this.transform.eulerAngles.x < 90f)
            eulerRotX = (this.transform.eulerAngles.x + 10f);
        else
            eulerRotX = 0f + 10f;

        this.transform.Rotate(new Vector3(-1, 0, 0), _Interval * Time.deltaTime);

        if (_DayState != DayState.Day && _DayState != DayState.PrepareDay)
        {
            if (this.transform.eulerAngles.x < 90f)
                _DayState = DayState.PrepareDay;
        }


        if (_DayState != DayState.Dusk && _DayState != DayState.PrepareDusk)
        {
            if (this.transform.eulerAngles.x <= 360 && this.transform.eulerAngles.x > 350)
                _DayState = DayState.PrepareDusk;
        }

        if (_DayState != DayState.Night && _DayState != DayState.PrepareNight)
        {
            // NOTE: 335 IS WHEN NO LONGER LIGHT. So This is when should cut out.
            if (this.transform.eulerAngles.x > 200f && this.transform.eulerAngles.x < 335f)
                _DayState = DayState.PrepareNight;
        }


        switch (_DayState)
        {
            case DayState.PrepareDawn:

                _DayState = DayState.Dawn;
                break;

            case DayState.PrepareDay:

                _DayState = DayState.Day;
                break;

            case DayState.PrepareDusk:



                _DayState = DayState.Dusk;
                break;

            case DayState.PrepareNight:

                this.GetComponent<Light>().intensity = 0;
                   RenderSettings.fogColor = Color.black;
                    RenderSettings.ambientIntensity = 0.1f;
                RenderSettings.reflectionIntensity = 0f;
                _DayState = DayState.Night;
                break;

            case DayState.Dawn:

                break;

            case DayState.Day:

                if (eulerRotX < (90f + 10f))
                {
                    float scaler = (1f / ((90f + 10f) / eulerRotX)) * 3f;
                    scaler = Mathf.Clamp(scaler, 0f, 1f);
                    RenderSettings.ambientIntensity = scaler;
                    RenderSettings.reflectionIntensity = scaler;
                    this.GetComponent<Light>().intensity = scaler;

                   

                    // MABE SHOULD KICK IN IF less than 10 say.
                    Color newColor = RenderSettings.fogColor;
                    newColor.r = 1.5f * (1f / ((90f) / eulerRotX));
                    newColor.g = 1.5f * (1f / ((90f) / eulerRotX));
                    newColor.b = 1.5f * (1f / ((90f) / eulerRotX));
                    RenderSettings.fogColor = newColor;

                }

                break;

            case DayState.Dusk:

                RenderSettings.ambientIntensity = (1f / ((90f + 10f) / eulerRotX)) * 2f;
                RenderSettings.reflectionIntensity = (1f / ((90f + 10f) / eulerRotX)) * 2f;
                this.GetComponent<Light>().intensity = (1f / ((90f + 10f) / eulerRotX)) * 2f;

                // MABE SHOULD KICK IN IF less than 10 say.
                Color newColor2 = RenderSettings.fogColor;
                newColor2.r = 1.25f * (1f / ((90f + 10f) / eulerRotX));
                newColor2.g = 1.25f * (1f / ((90f + 10f) / eulerRotX));
                newColor2.b = 1.25f * (1f / ((90f + 10f) / eulerRotX));
                RenderSettings.fogColor = newColor2;

                break;

            case DayState.Night:

 
                break;
        }



    }


}
