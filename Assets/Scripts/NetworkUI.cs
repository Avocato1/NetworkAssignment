using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [FormerlySerializedAs("clientButton")] [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField  codeInput;
    [SerializeField] private TextMeshProUGUI codeText;
    [FormerlySerializedAs("serverButton")] [SerializeField] private Button disconnectButton;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField]   TestRelay _testRelay;
    private readonly NetworkVariable<int> _playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            _testRelay.CreateRelay();
        });
        joinButton.onClick.AddListener(() =>
        {
            _testRelay.JoinRelay(codeInput.text);
        });
        disconnectButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
        });
        
        
    }  
    

    void Update()
    {
        playerCountText.text = "Players: " +  _playersNum.Value.ToString();
        
        if(!IsServer) return;
        _playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
        codeText.text = "Join code: " + _testRelay.codeText;
        
    }
    

}
