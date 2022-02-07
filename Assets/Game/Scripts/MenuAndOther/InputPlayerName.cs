using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for setting up player's name.
/// </summary>
public class InputPlayerName : MonoBehaviour {

    [Header("UI")]
    [SerializeField] private TMP_InputField inputPlayerName = null;
    [SerializeField] private Button confirmPlayerNameButton = null;
    /// <summary>
    /// Property used for showing player name.
    /// </summary>
    public static string DisplayName
    {
        get;
        private set;
    }
    
    private void Start() {
        SetUpInputField();
    }

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(Settings.playerPrefsNameKey))
        {
            return;
        }

        string defaultPlayerName = PlayerPrefs.GetString(Settings.playerPrefsNameKey);
        inputPlayerName.text = defaultPlayerName;
        SetPlayerName(defaultPlayerName);
    }
    /// <summary>
    /// Used for confirming inputted player name. Method makes confirm button interactable after player name is not empty.
    /// </summary>
    /// <param name="playerName"></param>
    public void SetPlayerName(string playerName)
    {
        confirmPlayerNameButton.interactable = !string.IsNullOrEmpty(playerName);
    }
    /// <summary>
    /// Method saves player name into player preferences. 
    /// </summary>
    public void SavePlayerNamePref()
    {
        DisplayName = inputPlayerName.text;

        PlayerPrefs.SetString(Settings.playerPrefsNameKey, DisplayName);
    }
    /// <summary>
    /// Method loads player name from player preferences. 
    /// </summary>
    public void SetDefaultPlayerName() {
        
        DisplayName = PlayerPrefs.GetString(Settings.playerPrefsNameKey);
    }
  
}