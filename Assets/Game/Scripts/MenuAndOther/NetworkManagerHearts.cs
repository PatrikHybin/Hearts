using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Script for network manager for game.
/// </summary>
public class NetworkManagerHearts : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;
    [Scene] [SerializeField] private string gameScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerHearts roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerHearts gamePlayerPrefab = null;
    [SerializeField] private GameObject spawnSystemPrefab = null;
    [SerializeField] private GameObject gameManagerPrefab = null;

    /// <summary>
    /// Event for when client is connected.
    /// </summary>
    public static event Action OnClientConnected;
    /// <summary>
    /// Event for when client is disconnected.
    /// </summary>
    public static event Action OnClientDisconnected;
    /// <summary>
    /// Event for when client is ready.
    /// </summary>
    public static event Action<NetworkConnection> OnServerReadied;

    /// <summary>
    /// Property representing players in lobby.
    /// </summary>
    public List<NetworkRoomPlayerHearts> RoomPlayers { get; } = new List<NetworkRoomPlayerHearts>();

    /// <summary>
    /// Property representing players in game.
    /// </summary>
    public List<NetworkGamePlayerHearts> GamePlayers { get; } = new List<NetworkGamePlayerHearts>();


    /// <summary>
    /// This is invoked when a server is started - including when a host is started.
    /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartServer() {
        RoomPlayers.Clear();
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnPrefabs").ToList();
    } //=> spawnPrefabs = Resources.LoadAll<GameObject>("SpawnPrefabs").ToList();


    /// <summary>
    /// This is invoked when the client is started.
    /// </summary>
    public override void OnStartClient()
    {
        var prefabs = Resources.LoadAll<GameObject>("SpawnPrefabs");

        foreach (var prefab in prefabs) {
            ClientScene.RegisterPrefab(prefab);
        } 
    }

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        OnClientConnected?.Invoke();
    }

    /// <summary>
    /// Called on clients when disconnected from a server.
    /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        
        OnClientDisconnected?.Invoke();
    }

    /// <summary>
    /// This is called when a client is stopped.
    /// </summary>
    public override void OnStopClient()
    {
        Debug.Log(" On stop client v manageri");
        ServerChangeScene("Scene_Menu");
        base.OnStopClient();
    }

    /// <summary>
    /// This is called when a host is stopped.
    /// </summary>
    public override void OnStopHost()
    {
        Debug.Log(" On stop host v manageri");
        ServerChangeScene("Scene_Menu");
        base.OnStopClient();
    }

    /// <summary>
    /// Called on the server when a new client connects.
    /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerConnect(NetworkConnection conn)
    {

        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            ServerChangeScene("Scene_Menu");
            return;
        }

        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
        
    }

    /// <summary>
    /// Called on the server when a client disconnects.
    /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<NetworkRoomPlayerHearts>();
            var playerGame = conn.identity.GetComponent<NetworkGamePlayerHearts>();

            
            GamePlayers.Remove(playerGame);

            RoomPlayers.Remove(player);


            NotifyPlayersOfReadyState();

        }
        
        base.OnServerDisconnect(conn);
    }
    /// <summary>
    /// Method used for changing ready state for each player.
    /// </summary>
    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers) {
            //host can interact with start button
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) {
                return false;
            }
        }

        if (RoomPlayers.Count < Settings.NumberOfPlayersToPlay) {
            return false;
            
        }
        return true;
    }

    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene) {
            
            bool isHost = RoomPlayers.Count == 0;
            NetworkRoomPlayerHearts roomPlayer = Instantiate(roomPlayerPrefab);
            
            roomPlayer.IsHost = isHost;
            Debug.Log(RoomPlayers.Count);
            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        }
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
        GamePlayers.Clear();
    }
    /// <summary>
    /// Method used for starting game.
    /// </summary>
    public void StartGame() {
        Debug.Log("Start Game");
        if (SceneManager.GetActiveScene().path == menuScene) {

            if (!IsReadyToStart()) {
                return;    
            }
            ServerChangeScene("Scene_Game");
        }
        
    }

    /// <summary>
    /// This causes the server to switch scenes and sets the networkSceneName.
    /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready during the change and ready again to participate in the new scene.</para>
    /// </summary>
    /// <param name="newSceneName"></param>
    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Scene_Game"))
        {
            Debug.Log(GamePlayers.Count + " kolko hracov tam je");
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);

                gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                
                gamePlayerInstance.name = RoomPlayers[i].DisplayName;
                gamePlayerInstance.IsHost = RoomPlayers[i].IsHost;
                //destroy roomplayer
                NetworkServer.Destroy(conn.identity.gameObject);
                //give them authority of objects
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
                

            }
        }
        
        if (SceneManager.GetActiveScene().path == gameScene && newSceneName.StartsWith("Scene_Menu"))
        {
            NetworkServer.Destroy(GameObject.Find("SpawnSystem(Clone)").gameObject);
            NetworkServer.Destroy(GameObject.Find("GameManagerHearts(Clone)").gameObject);
        }
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">The name of the new scene.</param>
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log(sceneName + "OnServerSceneChanged");
        if (sceneName.StartsWith("Scene_Game"))
        { 
            GameObject spawnSystemInstance = Instantiate(spawnSystemPrefab);
            NetworkServer.Spawn(spawnSystemInstance);
            //GameObject cardManagerInstance = Instantiate(cardManagerPrefab);
            //NetworkServer.Spawn(cardManagerInstance);
            GameObject gameManagerInstance = Instantiate(gameManagerPrefab);
            NetworkServer.Spawn(gameManagerInstance);

        }
    }

    /// <summary>
    /// Called on the server when a client is ready.
    /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

}
