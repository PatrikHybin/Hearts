using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for joining lobby from menu.
/// </summary>
public class JoinLobbyMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject background = null;
    [SerializeField] private TMP_InputField inputIpAddress = null;
    [SerializeField] private Button joinButton = null;
    

    private void OnEnable()
    {
        NetworkManagerHearts.OnClientConnected += HandleClientConnected;
        NetworkManagerHearts.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerHearts.OnClientConnected -= HandleClientConnected;
        NetworkManagerHearts.OnClientDisconnected -= HandleClientDisconnected;
    }
    /// <summary>
    /// Method used for connecting to created lobby.
    /// </summary>
    public void JoinLobby() {
        
        //Set ip address from input field
        string ipAddress = inputIpAddress.text;
        if (ipAddress == "") { return; }
        MainMenu.networkManager.networkAddress = ipAddress;
        MainMenu.networkManager.StartClient();
        
        joinButton.interactable = false;
    }

    private void HandleClientConnected() {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        background.SetActive(false);
     
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
       
    }

}
