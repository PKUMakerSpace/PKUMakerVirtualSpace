using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainWeapon:ObjectData{

    public bool hasMagpul = false, hasOptic = false, hasSilencer = false;
    public float[] fireRate;
    public int[] bulletForOneShot;
    public float[] Range;
    public int bulletNum = 0;
    public int maxBulletNum;
    public float reLoadTime;
    public int bulletVelocity;
    public int maxDistance;
    public string bulletName;
    public GameObject BulletPrefab;

    public MainWeapon(GameObject p, Sprite i,string name = "",int num = 1):base(num,name)
    {
        objectPrefab = p;
        objectImage = i;
    }

    public MainWeapon(MainWeapon old)
    {
        name = old.name;
        num = old.num;
        objectPrefab = old.objectPrefab;
        objectImage = old.objectImage;
        objectColliderPrefab = old.objectColliderPrefab;
        bulletNum = old.bulletNum;
        hasMagpul = old.hasMagpul;
        hasOptic = old.hasOptic;
        hasSilencer = old.hasSilencer;
    }
    public GameObject CreateWeapon(Transform parent)
    {
        GameObject temp = GameObject.Instantiate(objectPrefab,parent);
        temp.transform.position = parent.transform.position;
        RifleControl rf = temp.GetComponent<RifleControl>();
        rf.Data.bulletNum = bulletNum;

        return temp;
    }


}
