using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// Class used to write player's game results and edit his statistics.
/// </summary>
internal class WriteMpGameResults
{
    /// <summary>
    /// Method using api to save player's statistics and result of game to database. 
    /// </summary>
    /// <param name="players"></param>
    /// <param name="win"></param>
    /// <param name="round"></param>
    /// <param name="player"></param>
    public async void WritePlayerStats(List<Player> players, bool win, int round, Player player)
    {
        try
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(Settings.PlayerStatisticsUri + "/" + Settings.PlayerName);
            var responseString = await response.Content.ReadAsStringAsync();
            var statistics = JsonConvert.DeserializeObject<PlayerStatistics>(responseString);

            PlayerStatistics stat = new PlayerStatistics()
            {
                PlayerName = statistics.PlayerName,
                GamesPlayed = statistics.GamesPlayed + 1,
            };

            if (player.Win)
            {
                stat.GamesWon = statistics.GamesWon + 1;
            }

            stat.AllCardsCollected = player.GetComponent<Player>().AllCards + statistics.AllCardsCollected;
            stat.AllPointCardsCollected = player.GetComponent<Player>().AllPoints + statistics.AllPointCardsCollected;

            if (player.TotalScore < 50)
            {
                stat.LessThan50Points = statistics.LessThan50Points + 1;
            }
            if (player.TotalScore < 25)
            {
                stat.LessThan25Points = statistics.LessThan25Points + 1;
            }

            GameScore gameScore = new GameScore()
            {
                PlayerOneName = players[0].name,
                PlayerOneScore = players[0].TotalScore,
                PlayerTwoName = players[1].name,
                PlayerTwoScore = players[1].TotalScore,
                PlayerThreeName = players[2].name,
                PlayerThreeScore = players[2].TotalScore,
                PlayerFourName = players[3].name,
                PlayerFourScore = players[3].TotalScore,
                Round = round,
                Win = player.Win,
                PlayerName = Settings.PlayerName
            };
            stat.GameScores = new List<GameScore>();
            stat.GameScores.Add(gameScore);

            await client.PutAsync(Settings.PlayerStatisticsUri + "/" + Settings.PlayerName, new StringContent(
                JsonConvert.SerializeObject(stat), Encoding.UTF8, "application/json"));
        }
        catch (Exception)
        {
        }

    }


}