using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour {
    public ObjectData Data;
    public GameObject Self;
	// Use this for initialization
	void Start () {
		if(Data.objectPrefab.GetComponent<RifleControl>()!=null)
            Data = Data.objectPrefab.GetComponent<RifleControl>().Data;
        if (Data.objectPrefab.GetComponent<SilenserControl>() != null)
            Data = Data.objectPrefab.GetComponent<SilenserControl>().Data;
        if (Data.objectPrefab.GetComponent<OpticControl>() != null)
            Data = Data.objectPrefab.GetComponent<OpticControl>().Data;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
