using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

/// <summary>
/// Script used for getting and writing player's statistics and game results.
/// </summary>
public class ReadGameResults : MonoBehaviour
{
    [SerializeField] private GameObject scoreBoardPrefab, playerScorePrefab, roundPrefab, panel_results, panel_scoreHolder;
    /// <summary>
    /// Method for getting history of player's games using api, creating and populating tables with results from these games.
    /// </summary>
    /// <returns></returns>
    public async Task GetPlayerGameHistory()
    {
        try
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(Settings.PlayerStatisticsUri + "/" + Settings.PlayerName);
            string responseString = await response.Content.ReadAsStringAsync();
            var playerStatistics = JsonConvert.DeserializeObject<PlayerStatistics>(responseString);
            Debug.Log(playerStatistics.PlayerName);
            var playerGameScores = playerStatistics.GameScores ?? new List<GameScore>();

            GameObject scoreBoard = null;
            foreach (GameScore gameScore in playerGameScores)
            {
                GameObject game = Instantiate(scoreBoardPrefab, panel_results.transform);
                scoreBoard = game;
                foreach (Transform gameObject in scoreBoard.GetComponentsInChildren<Transform>())
                {
                    FillObjectsWithNames(gameObject, gameScore);
                    FillObjectsWithScores(gameObject, gameScore);

                }

                foreach (Transform gameObject in scoreBoard.GetComponentsInChildren<Transform>())
                {

                    if (gameObject.gameObject.name == "Panel_RoundNumber")
                    {
                        GameObject round = Instantiate(roundPrefab);
                        round.transform.SetParent(gameObject, false);
                        foreach (Transform transform in round.GetComponentsInChildren<Transform>())
                        {
                            if (transform.name == "Round")
                            {
                                transform.gameObject.GetComponent<TextMeshProUGUI>().text = gameScore.Round.ToString();
                            }
                        }
                    }
                }

                foreach (Transform gameObject in scoreBoard.GetComponentsInChildren<Transform>())
                {

                    if (gameObject.gameObject.name == "ScoreBoard_Title")
                    {

                        if (gameScore.Win)
                        {
                            gameObject.GetComponent<TextMeshProUGUI>().text = "Win";
                            gameObject.GetComponent<TextMeshProUGUI>().color = Color.green;
                        }
                        else
                        {
                            gameObject.GetComponent<TextMeshProUGUI>().text = "Loss";
                            gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
                        }
                    }
                }

            }
        }
        catch (Exception)
        {
        }
        
    }
    /// <summary>
    /// Method used for showing names on display.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="gameScore"></param>
    private void FillObjectsWithNames(Transform gameObject, GameScore gameScore)
    {
        if (gameObject.gameObject.name == "Panel_PlayerName0")
        {
            gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = gameScore.PlayerOneName;
        }
        if (gameObject.gameObject.name == "Panel_PlayerName1")
        {
            gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = gameScore.PlayerTwoName;
        }
        if (gameObject.gameObject.name == "Panel_PlayerName2")
        {
            gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = gameScore.PlayerThreeName;
        }
        if (gameObject.gameObject.name == "Panel_PlayerName3")
        {
            gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = gameScore.PlayerFourName;
        }
    }
    /// <summary>
    /// Method used for showing scores on display.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="gameScore"></param>
    private void FillObjectsWithScores(Transform gameObject, GameScore gameScore)
    {
        if (gameObject.gameObject.name == "Panel_Player0")
        {
            GameObject score = Instantiate(playerScorePrefab);
            score.transform.SetParent(gameObject, false);
            foreach (Transform transform in score.GetComponentsInChildren<Transform>())
            {
                if (transform.name == "PlayerScore")
                {
                    transform.gameObject.GetComponent<TextMeshProUGUI>().text = gameScore.PlayerOneScore.ToString();
                }
            }
        }
        if (gameObject.gameObject.name == "Panel_Player1")
        {
            GameObject score = Instantiate(playerScorePrefab);
            score.transform.SetParent(gameObject, false);
            foreach (Transform transform in score.GetComponentsInChildren<Transform>())
            {
                if (transform.name == "PlayerScore")
                {
                    transform.gameObject.GetComponent<TextMeshProUGUI>().text = gameScore.PlayerTwoScore.ToString();
                }
            }
        }
        if (gameObject.gameObject.name == "Panel_Player2")
        {
            GameObject score = Instantiate(playerScorePrefab);
            score.transform.SetParent(gameObject, false);
            foreach (Transform transform in score.GetComponentsInChildren<Transform>())
            {
                if (transform.name == "PlayerScore")
                {
                    transform.gameObject.GetComponent<TextMeshProUGUI>().text = gameScore.PlayerThreeScore.ToString();
                }
            }
        }
        if (gameObject.gameObject.name == "Panel_Player3")
        {
            GameObject score = Instantiate(playerScorePrefab);
            score.transform.SetParent(gameObject, false);
            foreach (Transform transform in score.GetComponentsInChildren<Transform>())
            {
                if (transform.name == "PlayerScore")
                {
                    transform.gameObject.GetComponent<TextMeshProUGUI>().text = gameScore.PlayerFourScore.ToString();
                }
            }
        }
    }
    /// <summary>
    /// Method used for getting player's statistics using api and showing this statistics on display.
    /// </summary>
    /// <returns></returns>
    public async Task GetPlayerStats() {

        try
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(Settings.PlayerStatisticsUri + "/" + Settings.PlayerName);
            string responseString = await response.Content.ReadAsStringAsync();
            var statistics = JsonConvert.DeserializeObject<PlayerStatistics>(responseString);

            if (statistics != null)
            {
                GameObject.Find("Text_GamesWonValue").GetComponent<TextMeshProUGUI>().text = statistics.GamesWon + "";
                GameObject.Find("Text_GamesLostValue").GetComponent<TextMeshProUGUI>().text = statistics.GamesPlayed - statistics.GamesWon + "";
                GameObject.Find("Text_AllCardsValue").GetComponent<TextMeshProUGUI>().text = statistics.AllCardsCollected + "";
                GameObject.Find("Text_AllPointsValue").GetComponent<TextMeshProUGUI>().text = statistics.AllPointCardsCollected + "";
                GameObject.Find("Text_TotalGamesPlayedValue").GetComponent<TextMeshProUGUI>().text = statistics.GamesPlayed + "";
                GameObject.Find("Text_LessThan50Value").GetComponent<TextMeshProUGUI>().text = statistics.LessThan50Points + "";
                GameObject.Find("Text_LessThan25Value").GetComponent<TextMeshProUGUI>().text = statistics.LessThan25Points + "";
            }
        }
        catch (Exception)
        {
        }
        
    }

}
