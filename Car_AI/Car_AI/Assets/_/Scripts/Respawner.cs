using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject carRoot;
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            carRoot.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            carRoot.transform.position = spawnPoint.transform.position;
            carRoot.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }
}
