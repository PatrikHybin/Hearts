using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Script for work with game menu in offline scene. Accessible by pressing Tab.
/// </summary>
public class OfflineInGameMenu : MonoBehaviour
{
    /// <summary>
    /// Method that resets offline game.
    /// </summary>
    public void PlayAgain() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Method that returns player to menu.
    /// </summary>
    public void ReturnToMenu ()
    {
        ScoreBoard score = GetComponentInChildren<ScoreBoard>();
        score.transform.parent = null;
        DontDestroyOnLoad(score);
        SceneManager.LoadScene("Scene_Menu");
    }
    /// <summary>
    /// Method that turns off game.
    /// </summary>
    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }
}
