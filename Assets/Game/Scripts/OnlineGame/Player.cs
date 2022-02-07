using Mirror;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

/// <summary>
/// Script for handling player in online scene.
/// </summary>
public class Player : NetworkBehaviour
{
    /// <summary>
    /// GameObjects representing prefabs for player's holders.
    /// </summary>
    [SerializeField] public GameObject parentHolder, handHolderPrefab, showHolderPrefab, enemyShowHolderPrefab, giveHolderPrefab, playAreaHolderPrefab, usedCardHolderPrefab, camera;
    [SerializeField] private TMP_Text text = null;
    [SerializeField] private GameObject cardPrefab, fakeCardPrefab;
    /// <summary>
    /// GameObjects for showing score board.
    /// </summary>
    [SerializeField] public GameObject scoreBoard, panel;
    [SerializeField] private GameObject playerScorePrefab, roundPrefab;
    /// <summary>
    /// GameObjects representing where can be put player's cards.
    /// </summary>
    public GameObject handHolder, showHolder, giveHolder, playAreaHolder, usedCardHolder;

    /// <summary>
    /// List of players.
    /// </summary>
    public static List<Player> players = new List<Player>();
    /// <summary>
    /// Property representing if player won.
    /// </summary>
    public bool Win { get; internal set; }
    /// <summary>
    /// Property for counting how many times did player collect all cards.
    /// </summary>
    public int AllCards { get; internal set; }
    /// <summary>
    /// Property for counting how many time did player collect all point cards but not all cards.
    /// </summary>
    public int AllPoints { get; internal set; }

    /// <summary>
    /// For representing player id in online game.
    /// </summary>
    [SyncVar]
    public int PlayerId = -1;

    /// <summary>
    /// Indicating that it's this player turn.
    /// </summary>
    [SyncVar]
    public bool myTurn = false;

    [SyncVar]
    private int roundScore = 0;
    /// <summary>
    /// Property for calculating score for one round..
    /// </summary>
    public int RoundScore
    {
        get { return roundScore; }
        set { roundScore = value; }
    }

    [SyncVar]
    private int totalScore = 0;
    /// <summary>
    /// Property for sum of all round scores.
    /// </summary>
    public int TotalScore
    {
        get { return totalScore; }
        set { totalScore = value; }
    }

    private string phase;
    /// <summary>
    /// Property for indicating current phase of game.
    /// </summary>
    public string Phase
    {
        get { return phase; }
        set { phase = value; }
    }

    private Sprite cardBack;
    private int cardCount;

    private Card selectedCard;
    /// <summary>
    /// Property for selected card.
    /// </summary>
    public Card SelectedCard
    {
        get { return selectedCard; }
        set { selectedCard = value; }
    }
    private GameObject cardZone;
    /// <summary>
    /// Property for card zone from which player picked card.
    /// </summary>
    public GameObject CardZone
    {
        get { return cardZone; }
        set { cardZone = value; }
    }
    private GameObject currentCardZone;
    /// <summary>
    /// Property for current card zone above which player hovers with cursor.
    /// </summary>
    public GameObject CurrentCardZone
    {
        get { return currentCardZone; }
        set { currentCardZone = value; }
    }

    private GameObject fakeCard;
    private List<Sprite> spritesForCards;
        

    [SyncVar]
    private List<GameObject> cards;
    /// <summary>
    /// List holding all game objects for name holders.
    /// </summary>
    public List<GameObject> ScoreBoardNameHolders { get; internal set; }
    
    private List<GameObject> ScoreBoardScoreHolders;

    [SerializeField]
    [SyncVar]
    private bool canPlayThatCard = false;
    private bool endOfGame;

    [SerializeField]
    [SyncVar]
    private bool isHost;
    public bool IsHost
    {
        set { isHost = value; }
        get { return isHost; }
    }

    private NetworkManagerHearts room;
    private SoundManager soundManager;
    private Color giveCardColor;

    private NetworkManagerHearts Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerHearts;
        }
 
    }

    public void Awake()
    {
        //string name = InputPlayerName.DisplayName;
        //Debug.Log(name);

        players.Add(this);
        PlayerId = players.Count - 1;
        ScoreBoardNameHolders = new List<GameObject>();
        ScoreBoardScoreHolders = new List<GameObject>();
    }

    private void Start()
    {
        giveCardColor.a = 1; //transparency
        giveCardColor.r = 1;
        giveCardColor.g = 1;
        giveCardColor.b = 1;
    }

    private void Update()
    {
        try
        {
            GameObject.Find("PlayAreaHolderMaster").GetComponent<Transform>().rotation = Quaternion.Euler(
                GameObject.Find("PlayAreaHolderMaster").GetComponent<Transform>().eulerAngles.x,
                this.GetComponent<Transform>().eulerAngles.y,
                GameObject.Find("PlayAreaHolderMaster").GetComponent<Transform>().eulerAngles.z);
        }
        catch { }

        if (giveHolder != null) {
            if (Phase == "GivePhase")
            {
                giveCardColor.a = 1;
                giveHolder.GetComponent<Image>().color = giveCardColor;
            }
            else
            {
                giveCardColor.a = 0;
                giveHolder.GetComponent<Image>().color = giveCardColor;
            }
        }
        
        if (Input.GetKey(KeyCode.Tab) == true)
        {
            if (hasAuthority)
            {
                //GameObject gameObject = GetComponentInChildren<ScoreBoard>().gameObject;
                GetComponentInChildren<ScoreBoard>().GetComponent<CanvasGroup>().blocksRaycasts = true;
                scoreBoard.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f, 0.3f);
                scoreBoard.GetComponent<RectTransform>().anchorMax = new Vector2(0.7f, 0.7f);
                scoreBoard.transform.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                scoreBoard.transform.transform.localScale = new Vector3(0, 0, 0);
                GetComponentInChildren<ScoreBoard>().GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
        else
        {
            if (hasAuthority)
            {
                //GameObject gameObject = GetComponentInChildren<ScoreBoard>().gameObject;
                GetComponentInChildren<ScoreBoard>().GetComponent<CanvasGroup>().blocksRaycasts = false;
                scoreBoard.transform.transform.localScale = new Vector3(0, 0, 0);

            }
            else
            {
                scoreBoard.transform.transform.localScale = new Vector3(0, 0, 0);
                GetComponentInChildren<ScoreBoard>().GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

        }

        if (hasAuthority)
        {
            if (Input.GetKey(KeyCode.Escape) && !GameManagerHearts.gameManager.gameEnded)
            {
                panel.SetActive(true);
                panel.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                panel.SetActive(false);
            }
        }
        else {
            panel.transform.localScale = new Vector3(0, 0, 0);
            panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        

        if (GameManagerHearts.gameManager.gameEnded) {
            if (hasAuthority) {
                scoreBoard.transform.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                //scoreBoard.transform.position = new Vector3(scoreBoard.transform.position.x, scoreBoard.transform.position.y, -300);
                scoreBoard.GetComponent<RectTransform>().anchorMin = new Vector2(0.4f, 0.4f);
                scoreBoard.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0.8f);
                panel.SetActive(true);
            }
        }

        if (this.TotalScore >= Settings.LosingScoreMp && !endOfGame) {
            endOfGame = true;
            Debug.Log("endOfGame");
            CmdEndOfGame();
        }

    }

    [Command]
    private void CmdEndOfGame()
    {
        RpcEndOfGame();
    }

    [ClientRpc]
    private void RpcEndOfGame()
    {
        GameManagerHearts.gameManager.gameEnded = true;
        endOfGame = true;
        parentHolder.SetActive(false);
        int losingScore = Settings.LosingScoreMp;

        foreach (Player player in players) {
            if (player.TotalScore > losingScore) {
                losingScore = player.TotalScore;
            }
        }
        List<Player> losingPlayers = new List<Player>();
        foreach (Player player in players)
        {
            if (player.TotalScore == losingScore)
            {
                losingPlayers.Add(player);
                player.Win = false;
            } else
            {
                player.Win = true;
            }
        }

        foreach (Player player in players)
        {
            if (player.hasAuthority)
            {
                WriteMpGameResults gameScore = new WriteMpGameResults();
                gameScore.WritePlayerStats(players, player.Win, GameManagerHearts.gameManager.currentRound, player);
                //gameScore.WritePlayerStats(player);
            }
        }

    }


    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient()
    {
        players.Remove(this);

        ScoreBoard score = GetComponentInChildren<ScoreBoard>();
        score.transform.parent = null;
        DontDestroyOnLoad(score);

        base.OnStopClient();
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority()
    {
        camera.gameObject.SetActive(true);
        CmdGameManagerGiveCardTime();
        Debug.Log("Start Auth");
        try
        {
            soundManager = GameObject.Find("SoundManager(Clone)").GetComponent<SoundManager>();
        }
        catch { }
    }
    
    [Command]
    private void CmdGameManagerGiveCardTime()
    {
        GameManagerHearts.gameManager.GiveCardTime();
    }

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        spritesForCards = new List<Sprite>();
    }

    
    /// <summary>
    /// Method that will play random card for player if his timer runs out.
    /// </summary>
    public void PlayRandomCard()
    {        
        bool haveToMatch = false;
        List<Card> tmpCards = new List<Card>();
        foreach (Card card in handHolder.GetComponentsInChildren<Card>())
        {
            NetworkIdentity foundNetworkIdentity = null;
            NetworkIdentity.spawned.TryGetValue(PlayArea.playArea.FirstPlayedCard, out foundNetworkIdentity);
            if (foundNetworkIdentity)
            {
                if (NetworkIdentity.spawned[PlayArea.playArea.FirstPlayedCard].GetComponent<Card>().CardSuite == card.CardSuite)
                {
                    tmpCards.Add(card);
                    haveToMatch = true;
                }
            }
        }
        if (SelectedCard != null && !haveToMatch)
        {
            CardZone = playAreaHolder;
            StartCoroutine(WaitForAuthorityAssignment(SelectedCard));
            return;
        }
        if (SelectedCard != null && haveToMatch) {
            ReleaseCard(SelectedCard);
        }
        if (!haveToMatch)
        {
            tmpCards.Clear();
            foreach (Card card in handHolder.GetComponentsInChildren<Card>())
            {
                tmpCards.Add(card);
            }
        }
        
        Card selectedCard;
        Random.InitState(DateTime.Now.Second);
        int idOfCard = Random.Range(0, tmpCards.Count - 1);
        selectedCard = tmpCards[idOfCard];
        CardZone = playAreaHolder;
        Debug.Log("selected " +selectedCard);
        SetSelectedCard(selectedCard);
        StartCoroutine(WaitForAuthorityAssignment(selectedCard));

    }

    private IEnumerator WaitForAuthorityAssignment(Card selectedCard)
    {
        yield return new WaitForSeconds(0.2f);
        
        ReleaseCard(selectedCard);
    }
    /// <summary>
    /// Method for setting selected card and allows player to "pick" card from his hand.
    /// </summary>
    /// <param name="card"></param>
    public void SetSelectedCard(Card card)
    {
        
        int selectedCardIndex = card.transform.GetSiblingIndex();
        int indexOfPlayer = card.OwnerId;
        card.SetCurrentZone(card.GetComponentInParent<CardZone>());

        SelectedCard = card;

        GameObject zone = SelectedCard.transform.parent.transform.gameObject;
        CurrentCardZone = zone;

        GetFakeCard(indexOfPlayer).SetActive(true);

        GetFakeCard(indexOfPlayer).transform.SetSiblingIndex(selectedCardIndex);


        SelectedCard.transform.SetParent(Player.players[indexOfPlayer].parentHolder.transform);

        cardCount = Player.players[indexOfPlayer].handHolder.transform.childCount;

        if (selectedCardIndex + 1 < cardCount)
        {
            SelectedCard.NextCard = Player.players[indexOfPlayer].handHolder.transform.GetChild(selectedCardIndex + 1).GetComponent<Card>();
        }
        else
        {
            SelectedCard.NextCard = null;
        }

        if (selectedCardIndex - 1 >= 0)
        {
            //card that will replace me
            SelectedCard.PreviousCard = Player.players[indexOfPlayer].handHolder.transform.GetChild(selectedCardIndex - 1).GetComponent<Card>();
        }
        else
        {
            SelectedCard.PreviousCard = null;
        }
        CmdRemoveObjectAuthority(GameObject.Find("PlayAreaHolderMaster"));
        CmdAssignObjectAuthority(GameObject.Find("PlayAreaHolderMaster"));
        if (SelectedCard.hasAuthority)
        {
            CmdCheckIfPlayerCanPlayThatCard(card);
        }
        try
        {
            soundManager.PlayCardSound("playCard");
        }
        catch { }
    }
    /// <summary>
    /// Method that is used for releasing card. Checks if we can release card, if its correct area or phase.
    /// </summary>
    /// <param name="card"></param>
    public void ReleaseCard(Card card)
    {
        int indexOfPlayer = card.OwnerId;
        
        if (SelectedCard != null)
        {
            GetFakeCard(indexOfPlayer).SetActive(false);

            CheckGivePhase();

            if (CardZone != null)
            {
                
                CmdUpdateCard(SelectedCard, CardZone);             
                if (cardZone.name.Contains("PlayAreaHolderPlayer") && players[card.OwnerId].myTurn && players[card.OwnerId].canPlayThatCard) {        
                    PlayArea playArea = GameObject.Find("PlayAreaHolderMaster").GetComponent<PlayArea>();
                    if (card.hasAuthority)
                    {
                        //CmdAssignObjectAuthority(playArea.gameObject);
                        if (playArea.hasAuthority) {
                            
                            playArea.PlayCard(this, card);
                        }
                        CmdRemoveObjectAuthority(playArea.gameObject);
                        
                    }
                    else
                    {
                        Debug.Log("nemas autoritu na kartou");
                    }
                }
                
            }
            else
            {
                CmdUpdateCard(SelectedCard, CurrentCardZone);
            }
  
            

            SelectedCard.transform.SetSiblingIndex(fakeCard.transform.GetSiblingIndex());
            GetFakeCard(indexOfPlayer).transform.SetParent(players[indexOfPlayer].parentHolder.transform, false);

            SelectedCard = null;
            if (hasAuthority) {
                CmdCantPlayThatCard(this);
            }
        }
    }

    [Command]
    private void CmdCantPlayThatCard(Player player)
    {
        RpcCantPlayThatCard(player);
    }
    [ClientRpc]
    private void RpcCantPlayThatCard(Player player)
    {
        player.canPlayThatCard = false;
    }

    [Command]
    private void CmdCheckIfPlayerCanPlayThatCard(Card releasedcard)
    {
        Player player = players[releasedcard.OwnerId];
        int notSameSuite = 0;

        uint netId = PlayArea.playArea.FirstPlayedCard;


        NetworkIdentity foundNetworkIdentity = null;
        NetworkIdentity.spawned.TryGetValue(netId, out foundNetworkIdentity);
        if (!foundNetworkIdentity)
        {
            player.canPlayThatCard = true;
            RpcCheckIfPlayerCanPlayThatCard(releasedcard, true);
            return;
        }

        if (releasedcard.CardSuite == NetworkIdentity.spawned[netId].GetComponent<Card>().CardSuite)
        {
            player.canPlayThatCard = true;
            RpcCheckIfPlayerCanPlayThatCard(releasedcard, true);
            return;
        }
        else
        {
            foreach (Card card in player.handHolder.GetComponentsInChildren<Card>())
            {

                if (card.CardSuite == NetworkIdentity.spawned[netId].GetComponent<Card>().CardSuite)
                {
                    player.canPlayThatCard = false;
                    RpcCheckIfPlayerCanPlayThatCard(releasedcard, false);
                   
                    return;
                }
                else
                {
                    notSameSuite++;
                }

            }
            if (notSameSuite == player.handHolder.GetComponentsInChildren<Card>().Count<Card>())
            {
                player.canPlayThatCard = true;
                RpcCheckIfPlayerCanPlayThatCard(releasedcard, true); 
            }

        }  
        
    }

    [ClientRpc]
    private void RpcCheckIfPlayerCanPlayThatCard(Card releasedcard, bool decision)
    {
        foreach (Player gamer in players) {
            if (gamer.PlayerId == players[releasedcard.OwnerId].PlayerId) {
                players[releasedcard.OwnerId].canPlayThatCard = decision;
            }
        }

    }

    private void CheckGivePhase()
    {
        GameObject zone;
        if (CardZone != null)
        {
            zone = CardZone;
        }
        else
        {
            zone = CurrentCardZone;
        }

        if (phase == "GivePhase" && !zone.transform.name.Contains("GiveHolder"))
        {
            CardZone = null;
        }
    }
    /// <summary>
    /// Method that will check each players hand. If no one has cards prompts server to start new round.  
    /// </summary>
    [Command]
    public void CmdCheckHand()
    {
        int i = 0;
        foreach (Player player in players)
        {  
            if (player.handHolder.GetComponentsInChildren<Card>().Length == 0)
            {
                i++;
            }
        }
        
        if (i == players.Count)
        {
            PlayArea.playArea.FirstPlayedCard = 9999;
            GameManagerHearts.gameManager.giveCards = true;
        }
    }

    
    /// <summary>
    /// Method for setting that it is player's turn.
    /// </summary>
    /// <param name="player"></param>
    [Command]
    public void CmdMyTurn(Player player)
    {
        RpcMyTurn(player);
        
    }

    [ClientRpc]
    private void RpcMyTurn(Player player)
    {  
        int indexOfPlayer = (player.PlayerId + 1) % players.Count;
        players[player.PlayerId].myTurn = false;
        players[indexOfPlayer].myTurn = true;
        
        if (hasAuthority)
        {
            CmdPlayerPlayTime();
        }
    }
    /// <summary>
    /// Method counts down player's time for decision.
    /// </summary>
    [Command]
    public void CmdPlayerPlayTime()
    {
        GameManagerHearts.gameManager.PlayerPlayTime();
    }
    /// <summary>
    /// Method for updating position of card.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="card"></param>
    [Client]
    public void MoveCard(Vector3 position, Card card)
    {
        //int indexOfPlayer = card.OwnerId;
        if (SelectedCard != null)
        {
            SelectedCard.transform.position = position;
            //CheckNextCard();
            //CheckPreviousCard();
        }

    }
    /// <summary>
    /// Method that is used to write out how much time player has for decision.
    /// </summary>
    /// <param name="msg"></param>
    public void SetText(string msg) {
        text.text = msg;
    }

    /// <summary>
    /// Method that will trigger updating card for clients.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="cardZone"></param>
    [Command]
    public void CmdUpdateCard(Card card, GameObject cardZone)
    {
        RpcUpdateCard(card, cardZone);
    }
    /// <summary>
    /// Method that is called for each player. It will update position, visibility, ownership of card
    /// </summary>
    /// <param name="card"></param>
    /// <param name="cardZone"></param>
    [ClientRpc]
    public void RpcUpdateCard(Card card, GameObject cardZone)
    {      
        if (cardZone.transform.name.Contains("ShowHolder")) {
            
            card.SetImage(card.Face);
            if (!hasAuthority)
            {
                foreach (CardZone zone in GetComponentsInChildren<CardZone>())
                {
                    if (zone.name.Contains("ShowHolder"))
                    {
                        Debug.Log("Nasiel som enemyHolder");
                        cardZone = zone.gameObject;
                    }
                }
            }
        } 

        if (cardZone.transform.name.Contains("HandHolder") || cardZone.transform.name.Contains("GiveHolder")) { 
            if (card.hasAuthority) {
                card.SetImage(card.Face);
                try
                {
                    soundManager.PlayCardSound("placeCard");
                }
                catch { }
            } else {
                card.SetImage(card.Back);
            }
            if (cardZone.transform.name.Contains("GiveHolder") && cardZone.GetComponentsInChildren<Card>().Count<Card>() >= 2)
            {
                cardZone = card.CurrentZone.gameObject;

            }
            if (cardZone.transform.name.Contains("GiveHolder") && phase == "PlayPhase") {
                cardZone = card.CurrentZone.gameObject;
            }
            
        }

        if (cardZone.transform.name.Contains("PlayAreaHolder"))
        {
            
            if (players[card.OwnerId].myTurn && players[card.OwnerId].canPlayThatCard)
            {
                card.SetImage(card.Face);
                cardZone = GameObject.Find("PlayAreaHolderMaster");
            }
            if (!players[card.OwnerId].myTurn || !players[card.OwnerId].canPlayThatCard) {
                if (!card.CurrentZone)
                {
                    card.CurrentZone = players[card.OwnerId].handHolder.GetComponent<CardZone>();
                }
                else {
                    cardZone = card.CurrentZone.gameObject;
                }
            }
            if (cardZone.transform.name.Contains("PlayAreaHolderPlayer")) {
                if (!card.CurrentZone)
                {
                    card.CurrentZone = players[card.OwnerId].handHolder.GetComponent<CardZone>();
                }
                else {
                    cardZone = card.CurrentZone.gameObject;
                }
                
            }
            
        }
       
        if (cardZone.transform.name.Contains("UsedCardHolder"))
        {
            card.SetImage(card.Back);
        }

        card.transform.SetParent(cardZone.transform, false);
        //card.SetCurrentZone(cardZone.GetComponent<CardZone>());   
        card.transform.position = new Vector3(cardZone.transform.position.x, cardZone.transform.position.y, cardZone.transform.position.z);
        card.transform.rotation = cardZone.transform.rotation;

        if (cardZone.name == "PlayAreaHolderMaster")
        {
            card.transform.localScale = new Vector2(1.4f, 1.9f);
            foreach (Player player in players) {
                if (player.hasAuthority) {
                    try
                    {
                        soundManager.PlayCardSound("placeCard");
                    }
                    catch { }
                }
            }  
        }
        else
        {
            card.transform.localScale = new Vector2(1f, 1f);
        }

        if (hasAuthority) {
            CmdCheckHand();
        }

    }
    
    [Client]
    private GameObject GetFakeCard(int indexOfPlayer)
    {
        if (fakeCard != null)
        {
            if (fakeCard.transform.parent != players[indexOfPlayer].handHolder.transform)
            {
                fakeCard.transform.SetParent(players[indexOfPlayer].handHolder.transform);
            }

        }
        else
        {
            fakeCard = Instantiate(fakeCardPrefab);
            fakeCard.name = "Fake";
            fakeCard.transform.SetParent(players[indexOfPlayer].handHolder.transform);
        }

        return fakeCard;
    }

    [Command]
    private void CmdAssignPlayerAuthorityOverObject(Player player, GameObject gameObject)
    {
        gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(player.connectionToClient);
    }

    [Command]
    private void CmdAssignObjectAuthority(GameObject gameObject)
    {
        gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }
    
    [Command]
    private void CmdRemoveObjectAuthority(GameObject gameObject)
    {
        gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
    }
    /// <summary>
    /// Method used on server to assign authority over object to player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="gameObject"></param>
    [Server]
    public void ServerAssignPlayerAuthorityOverObject(Player player, GameObject gameObject)
    {
        gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(player.connectionToClient);
    }

    [Server]
    public void ServerAssignObjectAuthority(GameObject gameObject)
    {
        gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }
    /// <summary>
    /// Method used on server to remove authority over object for player.
    /// </summary>
    /// <param name="gameObject"></param>
    [Server]
    public void ServerRemoveObjectAuthority(GameObject gameObject)
    {
        gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
    }
    /// <summary>
    /// Method that adds name holder to players name holders.
    /// </summary>
    /// <param name="nameHolder"></param>
    public void AddNameHolder(GameObject nameHolder)
    {
        ScoreBoardNameHolders.Add(nameHolder);
    }
    /// <summary>
    /// Method that adds score holder to players score holders.
    /// </summary>
    /// <param name="scoreHolder"></param>
    public void AddScoreHolder(GameObject scoreHolder)
    {
        ScoreBoardScoreHolders.Add(scoreHolder);
    }
    /// <summary>
    /// Method used for disconnecting all players.
    /// </summary>
    public void StopAll()
    {
        CmdStopAll();
    }

    [Command]
    private void CmdStopAll()
    {
        RpcStopAll();
    }

    [ClientRpc]
    private void RpcStopAll()
    {
        Room.StopClient();
    }

}
