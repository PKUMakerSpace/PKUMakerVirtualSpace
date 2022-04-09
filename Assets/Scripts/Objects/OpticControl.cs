using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpticControl : MonoBehaviour
{
    public Optic Data;
    public Transform inOpticPos;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public Objects Drop(PlayerController controller)
    {
        GameObject collider = GameObject.Instantiate(Data.objectColliderPrefab, controller.transform.position, Quaternion.identity);
        GameObject temp = Data.CreateOptic(collider.transform);
        collider.GetComponentInChildren<Objects>().Data = temp.GetComponent<OpticControl>().Data;
        Destroy(gameObject);
        return collider.GetComponentInChildren<Objects>(); ;
    }
    public bool ToBackPack(PlayerController controller)
    {
        Objects temp = Drop(controller);
        controller.objectsAround.Add(temp);
        controller.AddObject(temp);
        return true;
    }
}
