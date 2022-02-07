using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for working with players connecting to lobby.
/// </summary>
public class NetworkRoomPlayerHearts : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobby = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Image[] playerReadyImages = new Image[4];
    [SerializeField] private Button startGameButton = null;
    private Sprite ready;
    private Sprite notReady;

    /// <summary>
    /// Property for displaying player name.
    /// </summary>
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = " ";
    /// <summary>
    /// Property for indicating if player is ready.
    /// </summary>
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isHost;
    /// <summary>
    /// Property shows which player is host.
    /// </summary>
    public bool IsHost {
        set {
            isHost = value;
            startGameButton.gameObject.SetActive(value);
        }
        get { return isHost; }
    }
   
    private NetworkManagerHearts room;
    /// <summary>
    /// Property of NetworkManagerHearts
    /// </summary>
    private NetworkManagerHearts Room {
        get {
            if (room != null) {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerHearts;
        }
    }

    private void Awake()
    {
        ready = Resources.Load<Sprite>("Sprites/ready");
        notReady = Resources.Load<Sprite>("Sprites/readyNot");
    }

    internal void HandleReadyToStart(bool readyToStart)
    {
        if (!isHost) {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority()
    {
        CmdSetDisplayName(InputPlayerName.DisplayName);
        lobby.SetActive(true);
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        UpdateDisplay();
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
        UpdateDisplay();
    }
    /// <summary>
    /// Hook method that is called when player status is changed. Used for updating player display.
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    /// <summary>
    /// Hook method that is called when player name is changed. Used for updating player display.
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority) {

            foreach (var player in Room.RoomPlayers) {
                if (player.hasAuthority) {

                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = " ";
            playerReadyTexts[i].text = " ";
            Color normalColor;
            ColorUtility.TryParseHtmlString("#81ACCA", out normalColor);
            playerReadyImages[i].color = normalColor;
            playerReadyImages[i].sprite = null;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            Room.RoomPlayers[i].gameObject.name = Room.RoomPlayers[i].DisplayName;
            playerReadyImages[i].sprite = Room.RoomPlayers[i].IsReady ?Room.RoomPlayers[i].ready : Room.RoomPlayers[i].notReady;
            playerReadyImages[i].color = Color.white;
        }

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    private void CmdSetDisplayName(string displayName) {
        RpcSetDisplayName(displayName);
    }

    [ClientRpc]
    private void RpcSetDisplayName(string displayName)
    {
        DisplayName = displayName;
        gameObject.name = displayName;
        UpdateDisplay();
    }
    /// <summary>
    /// Method used for changing ready button and changing player status on server.
    /// </summary>
    public void Ready() {
        ColorBlock color = ColorBlock.defaultColorBlock;
        Color normalColor;
        Color highlightedColor;
        Color pressedColor;
        if (!IsReady)
        {
            ColorUtility.TryParseHtmlString("#2FB444", out normalColor);
            ColorUtility.TryParseHtmlString("#DB3943", out highlightedColor);
            ColorUtility.TryParseHtmlString("#B7242C", out pressedColor);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#FFFFFF", out normalColor);
            ColorUtility.TryParseHtmlString("#2FB444", out highlightedColor);
            ColorUtility.TryParseHtmlString("#258235", out pressedColor);
        }
        color.normalColor = normalColor;
        color.highlightedColor = highlightedColor;
        color.pressedColor = pressedColor;
        foreach (Button button in gameObject.GetComponentsInChildren<Button>())
        {
            if (button.name == "Button_Ready")
            {
                button.colors = color;
            }
        }
        
        CmdReady();
    }
    /// <summary>
    /// Method for readying. Called from client and ran on server.
    /// </summary>
    [Command]
    public void CmdReady() {

        IsReady = !IsReady;
        Room.NotifyPlayersOfReadyState();
    }
    /// <summary>
    /// Method used to leave lobby.
    /// </summary>
    public void Leave()
    {
        
        if (isHost)
        {
            Room.StopHost();
        }
        else
        {
            Room.StopClient();
        }
        Room.NotifyPlayersOfReadyState();
    }
    /// <summary>
    /// Method used by host to start game.
    /// </summary>
    [Command]
    public void CmdStartGame() {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { 
            return;
        }
        Room.StartGame();
    }
}
