using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayTime : MonoBehaviour
{

    int hours = 0;
    int minutes = 0;
    int seconds = 0;

    string disHours = "00";
    string disMinutes = "00";
    string disSeconds = "00";

    public Text _Text;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("Iterate");
    }

    IEnumerator Iterate()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);

            seconds++;
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
               

                if (minutes >= 60)
                {
                    minutes = 0;
                    hours++;
                    if(hours < 10)
                        disHours = "0" + hours;
                    else
                        disHours = hours.ToString();
                }

                if (minutes < 10)
                    disMinutes = "0" + minutes;
                else
                    disMinutes = minutes.ToString();
            }

            if (seconds < 10)
                disSeconds = "0" + seconds;
            else
                disSeconds = seconds.ToString();


            if(_Text)
            {
                _Text.text = disHours + ":" + disMinutes + ":" + disSeconds;
            }

        }
    }
}
