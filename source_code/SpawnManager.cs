using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

        GameObject[] SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
          int[] randomIndexes = new int[Players.Length];
        List<int> SpawnPointIndexes = new List<int>();
        int max;
        int min = 0;

        for(int i = 0; i < SpawnPoints.Length; i++)
        {
            SpawnPointIndexes.Add(i);
           
        }
         max = SpawnPointIndexes.Count;

        for (int i = 0; i < randomIndexes.Length; i++)
        {
            int rand = Random.Range(min, max);

            randomIndexes[i] = rand;

            if (SpawnPointIndexes.Count <= 0) break;

            if(rand == max)
            {
                max = SpawnPointIndexes[rand - 1];
            }
            if(rand == min)
            {
                min = SpawnPointIndexes[rand + 1];
            }
        }


        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].transform.position = SpawnPoints[randomIndexes[i]].transform.position + new Vector3(0f, Players[i].transform.GetComponent<Collider>().bounds.size.y, 0f);
        }
    }

}
