using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInfo : MonoBehaviour
{
    public static Dictionary<string, bool> rodaRua;
    public static Dictionary<string, bool> rodaTerreno;

    private void Awake()
    {
        rodaRua = new Dictionary<string, bool>();
        rodaTerreno = new Dictionary<string, bool>();
    }

    public void ConsultDictionaryRua(string input)
    {
        foreach (var key in rodaRua.Keys)
        {
            if (input.Contains(key))
                Debug.Log("string contains key: " + key + " " + rodaRua[key].ToString());
        }
    }    
    public void ConsultDictionaryTerreno(string input)
    {
        foreach (var key in rodaTerreno.Keys)
        {
            if (input.Contains(key))
                Debug.Log("string contains key: " + key + " " + rodaTerreno[key].ToString());
        }
    }
}
