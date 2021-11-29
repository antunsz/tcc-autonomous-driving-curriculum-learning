using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject car;

    public void SpawnCarSpot1()
    {
        car.transform.position = spawnPoints[0].transform.position;
        car.transform.rotation = spawnPoints[0].transform.rotation;
    }
    public void SpawnCarSpot2()
    {
        car.transform.position = spawnPoints[1].transform.position;
        car.transform.rotation = spawnPoints[1].transform.rotation;
    }
    public void SpawnCarSpot3()
    {
        car.transform.position = spawnPoints[2].transform.position;
        car.transform.rotation = spawnPoints[2].transform.rotation;
    }

}
