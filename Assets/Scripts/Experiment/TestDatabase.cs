using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PKU.Services;
using PKU.DataModels;
using System;
using UnityEngine.UI;

public class TestDatabase : NetworkBehaviour 
{
    [SerializeField]
    private InputField registerEmail;
    [SerializeField]
    private InputField searchEmail;
    [SerializeField]
    private InputField searchID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void TestRegisterServerRpc(string email)
    {
        IPlayerService playerService = new PlayerService();
        try
        {
            var succeeded = playerService.RegisterPlayer(new PlayerData { NickName = "x", StudentID = "x", Email = email });
            if (!succeeded)
            {
                Debug.Log("register failed");
            }
        }
        catch(Exception e)
        {
            Debug.Log("got exception");
        }
    }

    [ServerRpc]
    public void TestSearchByIDServerRpc(int ID)
    {
        IPlayerService playerService = new PlayerService();
        try
        {
            var result = playerService.GetPlayerDataById(ID);
            if (result == null)
            {
                Debug.Log("not found");
            }
            else
            {
                Debug.Log($"PlayerData {result.Email}, {result.NickName}, {result.StudentID}");
            }
        }
        catch (Exception e)
        {
            Debug.Log("got exception");
        }

    }

    [ServerRpc]
    public void TestSearchByEmailServerRpc(string email)
    {
        IPlayerService playerService = new PlayerService();
        try
        {
            var result = playerService.GetPlayerDataByEmail(email);
            if (result == null)
            {
                Debug.Log("not found");
            }
            else
            {
                Debug.Log($"PlayerData {result.Email}, {result.NickName}, {result.StudentID}");
            }
        }
        catch (Exception e)
        {
            Debug.Log("got exception");
        }
    }

    public void OnRegisterClicked()
    {
        string email = registerEmail.text;
        TestRegisterServerRpc(email);
    }

    public void OnSearchByIDClicked()
    {
        int ID = int.Parse(searchID.text);
        TestSearchByIDServerRpc(ID);
    }

    public void OnSearchByEmailClicked()
    {
        string email = searchEmail.text;
        TestSearchByEmailServerRpc(email);
    }
}
