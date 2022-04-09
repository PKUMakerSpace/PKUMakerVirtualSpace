using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenserControl : MonoBehaviour {
    public Silenser Data;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public Objects Drop(PlayerController controller)
    {
        GameObject collider = GameObject.Instantiate(Data.objectColliderPrefab, controller.transform.position, Quaternion.identity);
        GameObject temp = Data.CreateSilenser(collider.transform);
        collider.GetComponentInChildren<Objects>().Data = temp.GetComponent<SilenserControl>().Data;
        Destroy(gameObject);
        controller.mainSystem.objects.Add(collider.GetComponentInChildren<Objects>());
        return collider.GetComponentInChildren<Objects>();
    }
    public bool ToBackPack(PlayerController controller)
    {
        Objects temp = Drop(controller);
        controller.objectsAround.Add(temp);
        controller.AddObject(temp);
        return true;
    }
}
