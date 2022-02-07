using System.Collections.Generic;
/// <summary>
/// Class representing player statistics model.
/// </summary>
public class PlayerStatistics
{
    /// <summary>
    /// Property representing actual player name which played on this pc.
    /// </summary>
    public string PlayerName { get; set; }
    /// <summary>
    /// Property representing how many game player played.
    /// </summary>
    public int GamesPlayed { get; set; }
    /// <summary>
    /// Property representing how many games player won.
    /// </summary>
    public int GamesWon { get; set; }
    /// <summary>
    /// Property representing how many times did player collect all cards.
    /// </summary>
    public int AllCardsCollected { get; set; }
    /// <summary>
    /// Property representing how many times did player collect only all cards worth points.
    /// </summary>
    public int AllPointCardsCollected { get; set; }
    /// <summary>
    /// Property representing how many time did player ending game with less than 50 points total.
    /// </summary>
    public int LessThan50Points { get; set; }
    /// <summary>
    /// Property representing how many time did player ending game with less than 25 points total.
    /// </summary>
    public int LessThan25Points { get; set; }
    /// <summary>
    /// Property representing player's played games.
    /// </summary>
    public List<GameScore> GameScores { get; set; }

}
