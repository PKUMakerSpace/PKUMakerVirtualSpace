using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ObjectData{

    public string name;
    public int num;
    public GameObject objectColliderPrefab;
    public GameObject objectPrefab;
    public Sprite objectImage;

    public ObjectData(int N = 1,string Name = "")
    {
        name = Name;
        num = N;
    }
    public ObjectData(ObjectData old)
    {
        name = old.name;
        num = old.num;
        objectPrefab = old.objectPrefab;
        objectImage = old.objectImage;
        objectColliderPrefab = old.objectColliderPrefab;
    }
    public void SetPrefab(GameObject p)
    {
        objectPrefab = p;
    }
    public void SetImage(Sprite i)
    {
        objectImage = i;
    }

}
