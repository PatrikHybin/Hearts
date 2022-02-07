using UnityEngine;

/// <summary>
/// Class for easy access and editing of game settings.
/// </summary>
public static class Settings
{
    public static float DefaultEffectVolume = 0.1f;
    public static float CardEffectVolume = 0.1f;
    public static int LosingScore = 1;
    public static int LosingScoreMp = 1;
    public static string CardSpritesPath = "Sprites/Cards";
    public static Vector3 DistanceCameraPlane = new Vector3(0, 400, 650);
    public static float MenuVolume;
    public static float GameVolume;
    public static float MusicVolume;
    public static int NumberOfPlayersToPlay = 4;
    public const string playerPrefsNameKey = "PlayerNameKey";
    public static string PlayerName = PlayerPrefs.GetString(playerPrefsNameKey);
    public static string PlayerStatisticsUri = "https://localhost:5000/api/PlayerStatistics";
}
