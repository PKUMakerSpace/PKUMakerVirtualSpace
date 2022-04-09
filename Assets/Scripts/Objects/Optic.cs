using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Optic : ObjectData
{
    public Optic(GameObject p, Sprite i, string name = "", int num = 1) : base(num, name)
    {
        objectPrefab = p;
        objectImage = i;
    }

    public Optic(Optic old)
    {
        name = old.name;
        num = old.num;
        objectPrefab = old.objectPrefab;
        objectImage = old.objectImage;
        objectColliderPrefab = old.objectColliderPrefab;
    }
    public GameObject CreateOptic(Transform parent)
    {
        GameObject temp = GameObject.Instantiate(objectPrefab, parent);
        temp.transform.position = parent.transform.position;
        return temp;
    }
}
