using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Silenser : ObjectData
{
    public Silenser(GameObject p, Sprite i, string name = "", int num = 1) : base(num, name)
    {
        objectPrefab = p;
        objectImage = i;
    }

    public Silenser(Silenser old)
    {
        name = old.name;
        num = old.num;
        objectPrefab = old.objectPrefab;
        objectImage = old.objectImage;
        objectColliderPrefab = old.objectColliderPrefab;
    }
    public GameObject CreateSilenser(Transform parent)
    {
        GameObject temp = GameObject.Instantiate(objectPrefab, parent);
        temp.transform.position = parent.transform.position;
       
        return temp;
    }
}
