using UnityEngine;
using Mirror;

/// <summary>
/// Script handling menu player can access in multiplayer game.
/// </summary>
public class GameMenuMP : NetworkBehaviour
{

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
    /// Method that will return player from multiplayer to menu.
    /// </summary>
    public void ReturnToMenu()
    {
        StopConnection();
        //SceneManager.LoadScene("Scene_Menu");
    }
    /// <summary>
    /// Method that will turn off game.
    /// </summary>
    public void QuitGame() {

        StopConnection();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
         Application.Quit();

    }
    /// <summary>
    /// Method that will disconnect player if he is client. And if he is host disconnects all players and stop host.
    /// </summary>
    public void StopConnection() {

        Player player = GetComponentInParent<Player>();

        if (player.IsHost)
        {
            player.StopAll();
            Room.StopHost();
        }
        else
        {
            if (Player.players.Count - 1 < Settings.NumberOfPlayersToPlay)
            {
                player.StopAll();
                Room.StopServer();
            }
            else
            {
                Room.StopClient();
            }
            //Room.StopClient();

        }
    }
    
}

