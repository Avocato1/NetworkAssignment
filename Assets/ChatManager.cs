using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatManager : NetworkBehaviour
{

    public static ChatManager Singleton;


    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private TMP_Text textArea;
    [SerializeField] private ScrollRect scrollView;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        //sets the text to nothing at start
        
        textArea.SetText("");
      
    }


    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrWhiteSpace(messageInput.text))
            {
                PlayerSettings playerSettings = FindLocalPlayerSettings();
                if (playerSettings != null)
                {
                    SendMessageServerRPC(messageInput.text,playerSettings.networkPlayerName.Value.ToString());
                }
            }
        }
    }
    private PlayerSettings FindLocalPlayerSettings()
    {
        //Get reference to all the players
        foreach (var player in FindObjectsOfType<PlayerSettings>())
        {
            if (player.IsOwner)
            {
                return player;
            }
        }
        return null;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRPC(string text, string playerName)
    {
        AddTextClientRPC(text, playerName);
    }

    [ClientRpc]
    void AddTextClientRPC(string text, string playerName)
    {
        AddText(playerName + ": " + text);
        messageInput.text = "";
    }

    void AddText(string chat)
    {
        string latestText = textArea.text;
        textArea.SetText(latestText + '\n' + chat);
        messageInput.ActivateInputField();
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        //scroll down to the bottom with a delay because it was too fast without it
        yield return new WaitForEndOfFrame();
        scrollView.verticalNormalizedPosition = 0f;
    }


    public override void OnNetworkDespawn()
    {
        textArea.SetText("");
        base.OnNetworkDespawn();
    }

    public void FocusOnInputField()
    {
        if (messageInput)
        {
            messageInput.ActivateInputField();
        }
    }
    
}
