using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetect : MonoBehaviour
{
    private bool rua;
    private bool terreno;

    public string whichWheel;

    private void Start()
    {
        whichWheel = gameObject.name;
        CarInfo.rodaRua.Add(whichWheel, false);
        CarInfo.rodaTerreno.Add(whichWheel, false);
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Rua")
        {
            rua = true;
            if(CarInfo.rodaRua.ContainsKey(whichWheel))
                CarInfo.rodaRua.Remove(whichWheel);
            CarInfo.rodaRua.Add(whichWheel, rua);
            Debug.Log(whichWheel + " wheel added to Rua");
        }

        if (col.tag == "Terreno")
        {
            terreno = true;
            if (CarInfo.rodaTerreno.ContainsKey(whichWheel))
                CarInfo.rodaTerreno.Remove(whichWheel);
            CarInfo.rodaTerreno.Add(whichWheel, terreno);
            Debug.Log(whichWheel + " wheel added to Terreno");
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Rua")
        {
            rua = false;
            if (CarInfo.rodaRua.ContainsKey(whichWheel))
            {
                CarInfo.rodaRua.Remove(whichWheel);
                Debug.Log(whichWheel + " wheel removed from Rua");
            }
            CarInfo.rodaRua.Add(whichWheel, rua);
        }

        if (col.tag == "Terreno")
        {
            terreno = false;
            if (CarInfo.rodaTerreno.ContainsKey(whichWheel))
            {
                CarInfo.rodaTerreno.Remove(whichWheel);
                Debug.Log(whichWheel + " wheel removed from Terreno");
            }
            CarInfo.rodaTerreno.Add(whichWheel, terreno);
        }
    }

    public bool getRua(){
        return this.rua;
    }

}
