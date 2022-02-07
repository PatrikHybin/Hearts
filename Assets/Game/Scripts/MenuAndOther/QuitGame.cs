using UnityEngine;

/// <summary>
/// Script used to exit game.
/// </summary>
public class QuitGame : MonoBehaviour
{
    /// <summary>
    /// Method used to exit game.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Game is exiting");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif    
        Application.Quit();
  
    }
}
