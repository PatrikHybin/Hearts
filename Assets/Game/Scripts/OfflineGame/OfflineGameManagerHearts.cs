using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Script representing game manager for offline scene. Controls spawning bots, timer, who's turn it is, cards, etc.
/// </summary>
public class OfflineGameManagerHearts : MonoBehaviour
{
    /// <summary>
    /// Instance for offline game manager.
    /// </summary>
    public static OfflineGameManagerHearts instanceOfflineManager;
    /// <summary>
    /// GameObject representing prefabs and players in game.
    /// </summary>
    [SerializeField] public GameObject player, enemy1, enemy2, enemy3, cardPrefab, fakeCardPrefab, roundPrefab, playerScorePrefab, panel;

    /// <summary>
    /// List of player in game.
    /// </summary>
    public List<GameObject> players = new List<GameObject>();
    /// <summary>
    /// Representing if game ended.
    /// </summary>
    public bool endOfGame = false;

    private OfflineCard firstPlayedCard;
    /// <summary>
    /// Property representing first played card.
    /// </summary>
    public OfflineCard FirstPlayedCard
    {
        get { return firstPlayedCard; }
        set { firstPlayedCard = value; }
    }

    private OfflineCard highestPlayedCard;
    /// <summary>
    /// Property representing highest played card.
    /// </summary>
    public OfflineCard HighestPlayedCard
    {
        get { return highestPlayedCard; }
        set { highestPlayedCard = value; }
    }

    private List<OfflineCard> playedCards;
    private List<GameObject> losingPlayers;

    private Sprite cardBack;
    private List<GameObject> cards;
    private List<Sprite> spritesForCards;
    private bool startedGame = true;
    private bool decideWhoTakes;
    private int interval = 2;
    private float playTime;
    private bool outOutOfCards;
    private int currentRound = 0;

    private List<string> botNames = new List<string>(new string[] { "James","John","Robert","Michael","William","David","Richard","Joseph","Thomas","Charles","Christopher",
                                                                        "Daniel","Matthew","Anthony","Donald","Mark","Paul","Steven","Andrew","Kenneth","Joshua","Kevin",
                                                                         "Brian","Edward","Ronald","Timothy","Jason","Jeffrey","Ryan","Jacob","Gary","Nicholas","Eric",
                                                                        "Jonathan","Stephen","Larry","Justin","Scott","Brandon","Mary","Patricia","Jennifer","Linda","Elizabeth",
                                                                         "Barbara","Susan","Jessica","Sarah","Karen","Nancy","Lisa","Margaret","Betty","Sandra","Ashley",
                                                                        "Dorothy","Kimberly","Emily","Donna","Michelle","Carol","Amanda","Melissa","Deborah","Stephanie",
                                                                        "Rebecca","Laura","Sharon","Cynthia","Kathleen","Amy","Shirley","Angela","Helen","Anna","Brenda",
                                                                        "Pamela","Nicole","Samantha"});
    private bool setScoreBoardNames;
    private float nextTime;
    private bool timerStart;

    /// <summary>
    /// Property for possible bot names.
    /// </summary>
    public List<string> BotNames {
        get { return botNames; }
    }
        
    private void Awake()
    {
        if (instanceOfflineManager == null)
        {
            instanceOfflineManager = this;
        }
    }

    private void Start()
    {
        playedCards = new List<OfflineCard>();
        spritesForCards = new List<Sprite>();
        cards = new List<GameObject>();

        
    }

    private void Update()
    {
        if (players.Count == 4 && startedGame && !endOfGame) {
            if (!setScoreBoardNames) {
                SetScoreBoardNames();
                player.GetComponent<OfflinePlayer>().AllPoints = 0;
                player.GetComponent<OfflinePlayer>().AllCards = 0;
            }
            SetPlayersId();
            OfflineTimer.timer.StartTime("GiveCards");
            LoadCards("Sprites/Cards");
            SpawnCards();
            SuffleCards();
            AssignCards();
            player.GetComponent<OfflinePlayer>().myTurn = false;
            startedGame = false;
            player.GetComponent<OfflinePlayer>().Phase = "GivePhase";
        }

        if (player.GetComponent<OfflinePlayer>().myTurn && !timerStart)
        {
            timerStart = true;
            OfflineTimer.timer.StartTime("PlayerTime");
        }
        if (!player.GetComponent<OfflinePlayer>().myTurn)
        {
            timerStart = false;
        }

        if (Time.time >= playTime)
        {
            if (decideWhoTakes)
            {
                DecideWhoTakes();
                playedCards.Clear();
                decideWhoTakes = false;
                
            }  
        }

        if (endOfGame) {
            player.GetComponent<OfflinePlayer>().endOfGame = true;
            SetTurnToPlayer(-1);
            //endOfGame = false;
            outOutOfCards = false;
            panel.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Escape) || endOfGame) {
            panel.SetActive(true);
        } else
        {
            panel.SetActive(false);
        }

        
        if (outOutOfCards)
        {
            if (Time.time >= nextTime)
            {
                //CountScore();
                if (!player.GetComponent<OfflinePlayer>().endOfGame && !endOfGame)
                {
                    SetTurnToPlayer(-1);
                    OfflineTimer.timer.StartTime("GiveCards");
                    outOutOfCards = false;
                    
                    SuffleCards();
                    AssignCards();
                    player.GetComponent<OfflinePlayer>().Phase = "GivePhase";
                }
                
                nextTime += 1;
            }

        }
        
    }

    private void SetScoreBoardNames()
    {
        List<Transform> nameHolders = new List<Transform>();
        foreach (Transform transform in player.GetComponent<OfflinePlayer>().scoreBoard.GetComponentsInChildren<Transform>())
        {
            if (transform.gameObject.name.Contains("Panel_PlayerName"))
            {
                nameHolders.Add(transform.gameObject.transform);

            }

        }
        int index = 0;
        foreach (Transform transform in nameHolders)
        {

            if (index < players.Count)
            {
                transform.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = players[index].name;
            }
            index++;
        }
    }

    private void SetPlayersId()
    {
        for (int i = 0; i < players.Count; i++) {
            if (players[i].GetComponent<OfflineBot>())
            {
                players[i].GetComponent<OfflineBot>().PlayerId = i;
            }
            if (players[i].GetComponent<OfflinePlayer>())
            {
                players[i].GetComponent<OfflinePlayer>().PlayerId = i;
            }
        }
    }

    private void LoadCards(string path)
    {
        Sprite[] cardSprites;
        cardSprites = Resources.LoadAll<Sprite>(path);

        foreach (Sprite card in cardSprites)
        {
            spritesForCards.Add(card);

        }
    }

    private void SpawnCards()
    {
        cards = new List<GameObject>();
        cardBack = spritesForCards.Find(item => item.name.Contains("background"));
        spritesForCards.RemoveAll(item => item.name.Contains("background"));
        string[] splitSpriteName;

        
        foreach (Sprite cardSprite in spritesForCards)
        {
            splitSpriteName = cardSprite.name.Split("_"[0]);
            GameObject card = Instantiate(cardPrefab);
            card.name = "Card " + splitSpriteName[0] + " " + splitSpriteName[1];
            cards.Add(card);
            card.GetComponent<OfflineCard>().SetImage(cardSprite);
            card.GetComponent<OfflineCard>().CardValue = EnumCardValue.GetValue(splitSpriteName[0]);
            card.GetComponent<OfflineCard>().CardSuite = splitSpriteName[1];
            card.GetComponent<OfflineCard>().Face = cardSprite;
            
        }
    }

    private void AssignCards()
    {
        int index = 0;
        int player = 0;

        foreach (GameObject cardGM in cards)
        {
            OfflineCard card = cardGM.GetComponent<OfflineCard>();

            if (players[player].GetComponent<OfflinePlayer>())
            {
                card.transform.SetParent(players[player].GetComponent<OfflinePlayer>().handHolder.transform, false);
                card.GetComponent<OfflineCard>().OwnerId = players[player].GetComponent<OfflinePlayer>().PlayerId;
                card.transform.rotation = card.transform.parent.rotation;
                card.transform.eulerAngles = new Vector3(card.transform.eulerAngles.x + 20, card.transform.eulerAngles.y, card.transform.eulerAngles.z);

            }
            if (players[player].GetComponent<OfflineBot>())
            {
                card.transform.SetParent(players[player].GetComponent<OfflineBot>().handHolder.transform, false);
                card.GetComponent<OfflineCard>().OwnerId = players[player].GetComponent<OfflineBot>().PlayerId;
                card.transform.rotation = card.transform.parent.rotation;
                card.transform.eulerAngles = new Vector3(card.transform.eulerAngles.x + 20, card.transform.eulerAngles.y, card.transform.eulerAngles.z);
            }

            if (card.GetComponent<OfflineCard>().OwnerId != this.player.GetComponent<OfflinePlayer>().PlayerId)
            {
                card.GetComponent<OfflineCard>().SetImage(cardBack);
            }
            else {
                card.GetComponent<OfflineCard>().SetImage(card.Face);
            }
            index++;
            if (index % 8 == 0)
            {
                player++;
                if (player == players.Count)
                {
                    break;
                }
            }
        }

    }

    private void SuffleCards()
    {
        Random.InitState(DateTime.Now.Second);
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject temp = cards[i];
            int randomIndex = Random.Range(0, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
    /// <summary>
    /// Method that is called when player plays card.
    /// </summary>
    /// <param name="selectedCard"></param>
    /// <param name="player"></param>
    public void PlayCard(OfflineCard selectedCard, GameObject player)
    {
        selectedCard.SetImage(selectedCard.Face);
        if (playedCards.Count == 0) {
            FirstPlayedCard = selectedCard;
            HighestPlayedCard = selectedCard;
        }
        if (HighestPlayedCard.CardValue < selectedCard.CardValue)
        {
            HighestPlayedCard = selectedCard;
        }
        playedCards.Add(selectedCard);

        if (playedCards.Count != players.Count) {
            ChangeTurn(player);
        }
        if (playedCards.Count == players.Count) {
            decideWhoTakes = true;
            playTime = Time.time + interval;
        }
    }

    private void ChangeTurn(GameObject player)
    {
        int id;
        if (player.GetComponent<OfflinePlayer>()) {
            id = player.GetComponent<OfflinePlayer>().PlayerId;
            int indexOfPlayer = (id + 1) % players.Count;
            players[indexOfPlayer].GetComponent<OfflineBot>().myTurn = true;
        }
        if (player.GetComponent<OfflineBot>()) {
            id = player.GetComponent<OfflineBot>().PlayerId;
            int indexOfPlayer = (id + 1) % players.Count;
            if (players[indexOfPlayer].GetComponent<OfflineBot>())
            {
                players[indexOfPlayer].GetComponent<OfflineBot>().myTurn = true;
            }
            if (players[indexOfPlayer].GetComponent<OfflinePlayer>()) 
            {
                players[indexOfPlayer].GetComponent<OfflinePlayer>().myTurn = true;
            }
            
        }
    }
    /// <summary>
    /// Method that prompts bots to fill their give holders.
    /// </summary>
    public void PlaceRandomCardsForBotsIntoGiveHolder()
    {
        foreach (GameObject player in players)
        {
            if (player.GetComponent<OfflineBot>())
            {
                player.GetComponent<OfflineBot>().PlaceRandomCardsIntoGiveHolder();
            }
        }
    }
    /// <summary>
    /// Method that send cards in give holder to next player.
    /// </summary>
    public void SendGiveHolderCards()
    {
        CheckPlayerGiveHolder();
        for (int i = 0; i < players.Count; i++) {
            int indexOfPlayer = (i + 1) % players.Count;
            //Will be bot
            if (players[indexOfPlayer].GetComponent<OfflineBot>()) { 
                if (players[i].GetComponent<OfflineBot>())
                {
                    foreach (OfflineCard card in players[i].GetComponent<OfflineBot>().giveHolder.GetComponentsInChildren<OfflineCard>())
                    {
                        card.transform.SetParent(players[indexOfPlayer].GetComponent<OfflineBot>().handHolder.transform, false);
                        card.OwnerId = players[indexOfPlayer].GetComponent<OfflineBot>().PlayerId;
                        
                    }
                }
                if (players[i].GetComponent<OfflinePlayer>())
                {
                    foreach (OfflineCard card in players[i].GetComponent<OfflinePlayer>().giveHolder.GetComponentsInChildren<OfflineCard>())
                    {
                        card.transform.SetParent(players[indexOfPlayer].GetComponent<OfflineBot>().handHolder.transform, false);
                        card.OwnerId = players[indexOfPlayer].GetComponent<OfflineBot>().PlayerId;
                        card.SetImage(cardBack);
                    }
                }
            }
            //Will be player
            if (players[indexOfPlayer].GetComponent<OfflinePlayer>())
            {
                foreach (OfflineCard card in players[i].GetComponent<OfflineBot>().giveHolder.GetComponentsInChildren<OfflineCard>()) {
                    card.transform.SetParent(player.GetComponent<OfflinePlayer>().handHolder.transform, false);
                    card.OwnerId = players[indexOfPlayer].GetComponent<OfflinePlayer>().PlayerId;
                    card.SetImage(card.Face);
                } 
            }
        }
        player.GetComponent<OfflinePlayer>().Phase = "PlayPhase";
        player.GetComponent<OfflinePlayer>().myTurn = true;
        
    }

    private void CheckPlayerGiveHolder()
    {
        int numberOfCards = player.GetComponent<OfflinePlayer>().giveHolder.GetComponentsInChildren<OfflineCard>().Length;
        if (numberOfCards < 2) {
            player.GetComponent<OfflinePlayer>().FillPlayerGiveHolder(2 - numberOfCards);
        }
    }

    private void DecideWhoTakes()
    {
        
        OfflineCard highestCard = FirstPlayedCard;

        foreach (OfflineCard card in playedCards) {
            if (card.CardSuite == FirstPlayedCard.CardSuite) {
                if (card.CardValue > highestCard.CardValue) {
                    highestCard = card;
                }
            }
        }
        FirstPlayedCard = null;
        GiveWonCardToPlayer(highestCard.OwnerId);
        SetTurnToPlayer(highestCard.OwnerId);
        
    }

    private void SetTurnToPlayer(int ownerId)
    {

        foreach (GameObject player in players) {
            if (player.GetComponent<OfflineBot>())
            {
                player.GetComponent<OfflineBot>().myTurn = false;
            }
            if (player.GetComponent<OfflinePlayer>())
            {
                player.GetComponent<OfflinePlayer>().myTurn = false;
            }
        }
        if (ownerId != -1)
        {
            if (players[ownerId].GetComponent<OfflineBot>())
            {
                players[ownerId].GetComponent<OfflineBot>().myTurn = true;
            }
            if (players[ownerId].GetComponent<OfflinePlayer>())
            {
                players[ownerId].GetComponent<OfflinePlayer>().myTurn = true;
                OfflineTimer.timer.StartTime("PlayerTime");
            }
        }
        
    }

    private void GiveWonCardToPlayer(int ownerId)
    {
        foreach (OfflineCard card in playedCards)
        {
            if (players[ownerId].GetComponent<OfflineBot>())
            {
                card.transform.SetParent(players[ownerId].GetComponent<OfflineBot>().usedCardHolder.transform, false);
                card.transform.localScale = new Vector3(1f, 1f, 1f);
                card.SetImage(cardBack);
            }
            if (players[ownerId].GetComponent<OfflinePlayer>())
            {
                card.transform.SetParent(players[ownerId].GetComponent<OfflinePlayer>().usedCardHolder.transform, false);
                card.transform.localScale = new Vector3(1f, 1f, 1f);
                card.SetImage(cardBack);
            }
        }
        CheckNumberOfCards();
    }

    private void CheckNumberOfCards()
    {
        int count = 0;
        foreach (GameObject player in players) {
            if (player.GetComponent<OfflineBot>())
            {
                if (player.GetComponent<OfflineBot>().handHolder.GetComponentsInChildren<OfflineCard>().Length == 0) {
                    count++;
                }
            }
            if (player.GetComponent<OfflinePlayer>())
            {
                if (player.GetComponent<OfflinePlayer>().handHolder.GetComponentsInChildren<OfflineCard>().Length == 0)
                {
                    count++;
                }
            }
        }
        if (count == players.Count) {
            CountScore();
            outOutOfCards = true;
        }
    }

    private void CountScore()
    {
        GameObject playerWithAll = null;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<OfflineBot>())
            {
                if (player.GetComponent<OfflineBot>().usedCardHolder.GetComponentsInChildren<OfflineCard>().Length == cards.Count) {
                    playerWithAll = player;
                    break;
                }
            }
            if (player.GetComponent<OfflinePlayer>())
            {
                if (player.GetComponent<OfflinePlayer>().usedCardHolder.GetComponentsInChildren<OfflineCard>().Length == cards.Count)
                {
                    player.GetComponent<OfflinePlayer>().AllCards++;
                    playerWithAll = player;
                    break;
                }
            }
        }
        
        if (playerWithAll != null) {
            Debug.Log(playerWithAll.name);
            foreach (GameObject player in players)
            {
                if (playerWithAll == player)
                {
                    if (player.GetComponent<OfflineBot>())
                    {
                        player.GetComponent<OfflineBot>().TotalScore -= 32;
                    }
                    if (player.GetComponent<OfflinePlayer>())
                    {
                        player.GetComponent<OfflinePlayer>().TotalScore -= 32;
                    }
                }
                else {
                    if (player.GetComponent<OfflineBot>())
                    {
                        player.GetComponent<OfflineBot>().TotalScore += 32;
                    }
                    if (player.GetComponent<OfflinePlayer>())
                    {
                        player.GetComponent<OfflinePlayer>().TotalScore += 32;
                    }
                }
            }
            
            PutScoreOnScoreBoard();
            return;
        }
        int score = 0;
        foreach (GameObject player in players)
        {
            
            if (player.GetComponent<OfflineBot>())
            {
                player.GetComponent<OfflineBot>().TotalScore += GetPlayerScore(player.GetComponent<OfflineBot>().usedCardHolder.GetComponentsInChildren<OfflineCard>()); ;
            }
            if (player.GetComponent<OfflinePlayer>())
            {
                player.GetComponent<OfflinePlayer>().TotalScore += GetPlayerScore(player.GetComponent<OfflinePlayer>().usedCardHolder.GetComponentsInChildren<OfflineCard>());
                score += GetPlayerScore(player.GetComponent<OfflinePlayer>().usedCardHolder.GetComponentsInChildren<OfflineCard>());
            }
        }
        if (score == 32) {
            player.GetComponent<OfflinePlayer>().AllPoints++;
        }
        CheckIfSomeoneLostGame();
        PutScoreOnScoreBoard();
    }

    private void PutScoreOnScoreBoard()
    {
        GameObject scoreBoard = player.GetComponentInChildren<ScoreBoard>().gameObject;

        foreach (Transform gameObject in scoreBoard.GetComponentsInChildren<Transform>())
        {
            if (gameObject.gameObject.name == "Panel_RoundNumber")
            {
                GameObject number = Instantiate(roundPrefab);
                number.transform.SetParent(gameObject, false);
                foreach (Transform transform in number.GetComponentsInChildren<Transform>())
                {
                    if (transform.name == "Round")
                    {
                        transform.gameObject.GetComponent<TextMeshProUGUI>().text = currentRound.ToString();
                    }
                }

            }
            foreach (GameObject player in players)
            {
                if (player.GetComponent<OfflinePlayer>()) {
                    if (gameObject.gameObject.name == "Panel_Player" + player.GetComponent<OfflinePlayer>().PlayerId)
                    {
                        GameObject score = Instantiate(playerScorePrefab);
                        score.transform.SetParent(gameObject, false);
                        foreach (Transform transform in score.GetComponentsInChildren<Transform>())
                        {
                            if (transform.name == "PlayerScore")
                            {
                                transform.gameObject.GetComponent<TextMeshProUGUI>().text = player.GetComponent<OfflinePlayer>().TotalScore.ToString();
                            }
                        }
                    }
                }

                if (player.GetComponent<OfflineBot>())
                {
                    if (gameObject.gameObject.name == "Panel_Player" + player.GetComponent<OfflineBot>().PlayerId)
                    {
                        GameObject score = Instantiate(playerScorePrefab);
                        score.transform.SetParent(gameObject, false);
                        foreach (Transform transform in score.GetComponentsInChildren<Transform>())
                        {
                            if (transform.name == "PlayerScore")
                            {
                                transform.gameObject.GetComponent<TextMeshProUGUI>().text = player.GetComponent<OfflineBot>().TotalScore.ToString();
                            }
                        }
                    }
                }

            }
        }
        currentRound++;
        //CheckIfSomeoneLostGame();
    }

    private void CheckIfSomeoneLostGame()
    {
        List<GameObject> losingPlayers = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<OfflineBot>())
            {
                OfflineBot bot = player.GetComponent<OfflineBot>();
                if (player.GetComponent<OfflineBot>().TotalScore >= Settings.LosingScore)
                {

                    losingPlayers.Add(player);
                    bot.Win = false;
                }
                else {
                    bot.Win = true;
                }
            }
            if (player.GetComponent<OfflinePlayer>())
            {
                OfflinePlayer offlinePlayer = player.GetComponent<OfflinePlayer>();

                if (player.GetComponent<OfflinePlayer>().TotalScore >= Settings.LosingScore) {
                    
                    losingPlayers.Add(player);
                    offlinePlayer.Win = false;
                } else
                {
                    offlinePlayer.Win = true;
                }
            }
        }
        if (losingPlayers.Count != 0) {
            endOfGame = true;
            //WritePlayersScore();
            WritePlayerStats();
        } 
        
    }
    private int GetPlayerScore(OfflineCard[] offlineCards)
    {
        int score = 0;

        foreach (OfflineCard card in offlineCards)
        {
            if (card.name.Contains("heart"))
            {
                score++;
            }
            if (card.name.Contains("upperKnave leaf"))
            {
                score += 12;

            }
            if (card.name.Contains("upperKnave acorn"))
            {
                score += 8;

            }
            if (card.name.Contains("upperKnave ball"))
            {
                score += 4;

            }
        }
        return score;
    }

    private async void WritePlayerStats()
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

            if (player.GetComponent<OfflinePlayer>().Win)
            {
                stat.GamesWon = statistics.GamesWon + 1;
            }

            stat.AllCardsCollected = player.GetComponent<OfflinePlayer>().AllCards + statistics.AllCardsCollected;
            stat.AllPointCardsCollected = player.GetComponent<OfflinePlayer>().AllPoints + statistics.AllPointCardsCollected;

            if (player.GetComponent<OfflinePlayer>().TotalScore < 50)
            {
                stat.LessThan50Points = statistics.LessThan50Points + 1;
            }
            if (player.GetComponent<OfflinePlayer>().TotalScore < 25)
            {
                stat.LessThan25Points = statistics.LessThan25Points + 1;
            }

            GameScore gameScore = new GameScore()
            {
                PlayerOneName = players[0].name,
                PlayerOneScore = players[0].GetComponent<OfflinePlayer>().TotalScore,
                PlayerTwoName = players[1].name,
                PlayerTwoScore = players[1].GetComponent<OfflineBot>().TotalScore,
                PlayerThreeName = players[2].name,
                PlayerThreeScore = players[2].GetComponent<OfflineBot>().TotalScore,
                PlayerFourName = players[3].name,
                PlayerFourScore = players[3].GetComponent<OfflineBot>().TotalScore,
                Round = currentRound,
                Win = player.GetComponent<OfflinePlayer>().Win,
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
