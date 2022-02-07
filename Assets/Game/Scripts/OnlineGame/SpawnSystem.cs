using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

/// <summary>
/// Script used for spawning players into game in online scene.
/// </summary>
public class SpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    

    private static List<Transform> spawnPoints = new List<Transform>();
    /// <summary>
    /// For counting how many players are connected.
    /// </summary>
    public int index = 0;

    private NetworkManagerHearts room;
    
    private NetworkManagerHearts Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerHearts;
        }
    }
    /// <summary>
    /// Method for adding spawn point.
    /// </summary>
    /// <param name="transform"></param>
    public static void AddSpawnPoint(Transform transform) {

        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();

    }
    /// <summary>
    /// Method for removing spawn point.
    /// </summary>
    /// <param name="transform"></param>
    public static void RemoveSpawnPoint(Transform transform) {

        spawnPoints.Remove(transform);
    }

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        NetworkManagerHearts.OnServerReadied += SpawnPlayer;
        NetworkManagerHearts.OnServerReadied += CheckToSpawnCards;
        base.OnStartServer();
    }

    //OnDestroy
    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer()
    {
        NetworkManagerHearts.OnServerReadied -= SpawnPlayer;
        NetworkManagerHearts.OnServerReadied -= CheckToSpawnCards;

    }
    /// <summary>
    /// Method used to spawn player on one of spawn points.
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    public void SpawnPlayer(NetworkConnection conn) {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(index);

        if (spawnPoint == null)
        {
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(playerInstance, conn);

        index++;
    }
    /// <summary>
    /// Method that check if every player is ready to play game.
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    public void CheckToSpawnCards(NetworkConnection conn)
    {

        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count)
        {
            return;
        }
        Debug.Log("Spawn karty v spawnsysteme");
        GameManagerHearts.gameManager.StartGame();
        
    }

}
