using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEditor;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    public string codeText;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        IngameDebugConsole.DebugLogConsole.AddCommand("CreateRelay", "Creates a relay", CreateRelay);
        IngameDebugConsole.DebugLogConsole.AddCommand<string>("JoinRelay", "Joins a relay", JoinRelay);
    }

    private void Update()
    {
       
    }
    public async void CreateRelay()
    {
        //creates a session and generates a joincode
        try
        {
            Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            Debug.Log("Join code: " + joinCode);
            
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartHost();
            codeText = joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        
    }
    
    
    public async void JoinRelay(string joinCode)
    {
        //tries to join with the code
        try
        {
            Debug.Log("Joining relay with code: " + joinCode);
           JoinAllocation joinAllocation =  await RelayService.Instance.JoinAllocationAsync(joinCode);
            
           RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            
           NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    
}

