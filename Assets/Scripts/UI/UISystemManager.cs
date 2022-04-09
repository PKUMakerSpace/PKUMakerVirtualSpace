using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISystemManager : MonoBehaviour,IPointerDownHandler {

    // Use this for initialization
    public GameObject present;
    public PlayerController playercontroller;
    public Image imagecontrol;
    public Sprite noneImage;

    public Objects presentObject;

    public Image MainWeapon1Image;
    public Image MainWeapon2Image;
    public Image DeputyWeaponImage;
    public Image MainWeapon1SilencerImage;
    public Image MainWeapon2SilencerImage;
    public Image MainWeapon1OpticImage;
    public Image MainWeapon2OpticImage;


    public ObjectView[] sceneObjectViews;
    public ObjectView[] playerObjectViews;

    public int from = 0;
    const int ID_NONE = 0;
    const int ID_VIEWBOARD = 1;
    const int ID_MAINWEAPON1 = 2;
    const int ID_SILENCER1 = 3;
    const int ID_MAGPUL1 = 4;
    const int ID_OPTIC1 = 5;
    const int ID_MAINWEAPON2 = 6;
    const int ID_SILENCER2 = 7;
    const int ID_MAGPUL2 = 8;
    const int ID_OPTIC2 = 9;

    public void OnPointerDown(PointerEventData eventData)
    {
        from = ID_NONE;
    }

    void Start () {
		
	}
	public void OnMainWeapon1ImageClick()
    {
       
    }
    public void OnSilencer1Click()
    {
      
    }
    public void OnSilencer2Click()
    {
       
    }
    public void OnOptic1Click()
    {
        
    }
    public void OnOptic2Click()
    {
        
    }
    public void OnBackPackBoardClick()
    {
        
    }
    public void OnOutSideBoardClick()
    {
        
    }
    public void OnDeputyWeaponImageClick()
    {
        if (from == ID_VIEWBOARD && !(presentObject.Data is MainWeapon))
        {
            DeputyWeaponImage.sprite = imagecontrol.sprite;
            from = ID_NONE;
        }
    }
	// Update is called once per frame
	void Update () {
        if (from != ID_NONE)
        {
            present.SetActive(true);
            present.transform.position = Input.mousePosition;
        }
        else
            present.SetActive(false);
        for(int i = 0;i<Mathf.Min(playercontroller.objectsAround.Count,sceneObjectViews.Length);i++)
            sceneObjectViews[i].SetObject(playercontroller.objectsAround[i]);
        for(int i = playercontroller.objectsAround.Count;i<sceneObjectViews.Length;i++)
            sceneObjectViews[i].clear();
        for (int i = 0; i < playercontroller.objectsOwned.Count; i++)
            playerObjectViews[i].SetObject(playercontroller.objectsOwned[i]);
        for (int i = playercontroller.objectsOwned.Count; i < playerObjectViews.Length; i++)
            playerObjectViews[i].clear();
        if (playercontroller.hasMainWeapon1)
        {
            MainWeapon1Image.sprite = playercontroller.MainWeapon1.GetComponent<RifleControl>().Data.objectImage;
            if (playercontroller.MainWeapon1.GetComponent<RifleControl>().Data.hasSilencer)
                MainWeapon1SilencerImage.sprite = playercontroller.MainWeapon1.GetComponent<RifleControl>().Silencer.GetComponent<SilenserControl>().Data.objectImage;
            else
                MainWeapon1SilencerImage.sprite = noneImage;
            if (playercontroller.MainWeapon1.GetComponent<RifleControl>().Data.hasOptic)
                MainWeapon1OpticImage.sprite = playercontroller.MainWeapon1.GetComponent<RifleControl>().Optic.GetComponent<OpticControl>().Data.objectImage;
            else
                MainWeapon1OpticImage.sprite = noneImage;
        }
        else
        {
            MainWeapon1Image.sprite = noneImage;
            MainWeapon1SilencerImage.sprite = noneImage;
            MainWeapon1OpticImage.sprite = noneImage;
        }
        if (playercontroller.hasMainWeapon2)
        {
            MainWeapon2Image.sprite = playercontroller.MainWeapon2.GetComponent<RifleControl>().Data.objectImage;
            if (playercontroller.MainWeapon2.GetComponent<RifleControl>().Data.hasSilencer)
                MainWeapon2SilencerImage.sprite = playercontroller.MainWeapon2.GetComponent<RifleControl>().Silencer.GetComponent<SilenserControl>().Data.objectImage;
            else
                MainWeapon2SilencerImage.sprite = noneImage;
            if (playercontroller.MainWeapon2.GetComponent<RifleControl>().Data.hasOptic)
                MainWeapon2OpticImage.sprite = playercontroller.MainWeapon2.GetComponent<RifleControl>().Optic.GetComponent<OpticControl>().Data.objectImage;
            else
                MainWeapon2OpticImage.sprite = noneImage;
        }
        else
        {
            MainWeapon2Image.sprite = noneImage;
            MainWeapon2SilencerImage.sprite = noneImage;
            MainWeapon2OpticImage.sprite = noneImage;
        }
    }
}
