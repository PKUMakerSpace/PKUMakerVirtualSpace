using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectView : MonoBehaviour {

    // Use this for initialization
    public GameObject Button;
    public Image ImageShow;
    public Text TextShow;
    public GameObject Self;
    public Sprite image;
    private Objects objects;
    private ObjectData data;
    public UISystemManager manager;
    private bool empty = true;

	void Start ()
    {
        ImageShow.sprite = image;
	}
	
	// Update is called once per frame
	void Update () {
        if (empty)
            ImageShow.sprite = image;
        else
        {
            ImageShow.sprite = data.objectImage;
            if (data.num == 1)
                TextShow.text = data.name;
            else
                TextShow.text = data.name + " x" + data.num;
        }
            
    }
    public void SetImage(Sprite i)
    {
        ImageShow.sprite = i;
    }
    public void SetObject(Objects o)
    {
        objects = o;
        data = o.Data;
        empty = false;
        Self.SetActive(true);
    }
    public void clear()
    {
        empty = true;
        Self.SetActive(false);
    }
    public void OnClick()
    {
        Debug.Log("Thread");
        manager.from = 1;
        manager.imagecontrol.sprite = ImageShow.sprite;
        manager.presentObject = objects;
    }
}
