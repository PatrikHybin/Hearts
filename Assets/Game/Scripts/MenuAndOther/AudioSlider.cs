using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Script for working with volume sliders.
/// </summary>
public class AudioSlider : MonoBehaviour
{
    [SerializeField] private Slider menuSlider = null;
    [SerializeField] private Slider gameSlider = null;
    [SerializeField] private TMP_Text menuText = null;
    [SerializeField] private TMP_Text gameText = null;
    private const string PlayerMenuVolumeKey = "PlayerMenuVolumeKey";
    private const string PlayerGameVolumeKey = "PlayerGameVolumeKey";

    void Start()
    {
        if (menuSlider != null) {
            menuSlider.minValue = 0;
            menuSlider.maxValue = 1;
            menuSlider.wholeNumbers = false;
        }
        
        if (gameSlider != null)
        {
            gameSlider.minValue = 0;
            gameSlider.maxValue = 1;
            gameSlider.wholeNumbers = false;
        }

        LoadPreferences();
    }

    private void LoadPreferences()
    {
        if (!PlayerPrefs.HasKey(PlayerMenuVolumeKey))
        {
            if (menuSlider != null)
            {
                menuSlider.value = 0.2f;
                menuText.text = menuSlider.value.ToString("F1");
            }

            if (gameSlider != null) {
                gameSlider.value = 0.2f;
                gameText.text = gameSlider.value.ToString("F1");
            }
            
            return;
        }

        if (menuSlider != null)
        {
            menuSlider.value = float.Parse(PlayerPrefs.GetString(PlayerMenuVolumeKey));
            menuText.text = menuSlider.value.ToString("F1");

            Settings.MenuVolume = float.Parse(PlayerPrefs.GetString(PlayerMenuVolumeKey));
        }

        if (gameSlider != null)
        {
            gameSlider.value = float.Parse(PlayerPrefs.GetString(PlayerGameVolumeKey));
            gameText.text = gameSlider.value.ToString("F1");

            Settings.GameVolume = float.Parse(PlayerPrefs.GetString(PlayerGameVolumeKey));
        }

        Settings.DefaultEffectVolume = float.Parse(PlayerPrefs.GetString(PlayerMenuVolumeKey));  
    }
    /// <summary>
    /// Used for changing menu volume
    /// </summary>
    /// <param name="value"></param>
    public void OnValueMenuChanged(float value) {
        menuSlider.value = value;
        menuText.text = menuSlider.value.ToString("F1");
        PlayerPrefs.SetString(PlayerMenuVolumeKey, menuText.text);
        Settings.DefaultEffectVolume = float.Parse(PlayerPrefs.GetString(PlayerMenuVolumeKey));
        Settings.MenuVolume = float.Parse(PlayerPrefs.GetString(PlayerMenuVolumeKey));
    }
    /// <summary>
    /// Used for changing game volume
    /// </summary>
    /// <param name="value"></param>
    public void OnValueGameChanged(float value)
    {
        gameSlider.value = value;
        gameText.text = gameSlider.value.ToString("F1");
        PlayerPrefs.SetString(PlayerGameVolumeKey, gameText.text);
        Settings.GameVolume = float.Parse(PlayerPrefs.GetString(PlayerGameVolumeKey));
    }
}
