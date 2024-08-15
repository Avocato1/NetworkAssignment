using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TextMeshProUGUI playerName;

    public NetworkVariable<FixedString128Bytes> networkPlayerName =
        new NetworkVariable<FixedString128Bytes>("Player : 0", NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public List<Color> Colors = new List<Color>();

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        //gets the players network id and changes the material for every new player
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        playerName.text = networkPlayerName.Value.ToString();
        _meshRenderer.material.color = Colors[(int)OwnerClientId];
    }
    

}
