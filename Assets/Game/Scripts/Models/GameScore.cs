/// <summary>
/// Class representing game score model.
/// </summary>
public class GameScore
{
    /// <summary>
    /// Property representing unique number.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Property representing name of player one.
    /// </summary>
    public string PlayerOneName { get; set; }
    /// <summary>
    /// Property representing score at the end of the game for player one.
    /// </summary>
    public int PlayerOneScore { get; set; }
    /// <summary>
    /// Property representing name of player two.
    /// </summary>
    public string PlayerTwoName { get; set; }
    /// <summary>
    /// Property representing score at the end of the game for player two.
    /// </summary>
    public int PlayerTwoScore { get; set; }
    /// <summary>
    /// Property representing name of player three.
    /// </summary>
    public string PlayerThreeName { get; set; }
    /// <summary>
    /// Property representing score at the end of the game for player three.
    /// </summary>
    public int PlayerThreeScore { get; set; }
    /// <summary>
    /// Property representing name of player four.
    /// </summary>
    public string PlayerFourName { get; set; }
    /// <summary>
    /// Property representing score at the end of the game for player four.
    /// </summary>
    public int PlayerFourScore { get; set; }
    /// <summary>
    /// Property representing if player won game.
    /// </summary>
    public bool Win { get; set; }
    /// <summary>
    /// Property representing number of round that were played.
    /// </summary>
    public int Round { get; set; }
    /// <summary>
    /// Property representing player statistics.
    /// </summary>
    public PlayerStatistics PlayerStatistics { get; set; }
    /// <summary>
    /// Property representing actual player name which played on this pc.
    /// </summary>
    public string PlayerName { get; set; }
}
