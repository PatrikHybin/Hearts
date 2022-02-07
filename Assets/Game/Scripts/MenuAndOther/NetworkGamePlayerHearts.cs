using Mirror;

/// <summary>
/// Script for working with players connecting to game.
/// </summary>
public class NetworkGamePlayerHearts : NetworkBehaviour
{
    /// <summary>
    /// Field for display name in game.
    /// </summary>
    [SyncVar]
    public string displayName = " ";

    [SyncVar]
    private bool isHost;
    /// <summary>
    /// Property shows which player is host.
    /// </summary>
    public bool IsHost
    {
        set { isHost = value; }
        get { return isHost; }
    }
    
    private NetworkManagerHearts room;
    
    private NetworkManagerHearts Room {
        get {
            if (room != null) {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerHearts;
        }
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this); 
    }
    /// <summary>
    /// Method used for setting player name on each client. 
    /// </summary>
    /// <param name="displayName"></param>
    [Server]
    public void SetDisplayName(string displayName) {
        this.displayName = displayName;
        this.gameObject.name = displayName;
    }

}
