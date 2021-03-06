using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Script representing play area in online scene. Decides who played highest card, which player is next to play.
/// </summary>
public class PlayArea : NetworkBehaviour
{
    /// <summary>
    /// Field for access to play area.
    /// </summary>
    public static PlayArea playArea;
    private bool initiated = false;


    [SyncVar]
    private uint firstPlayedCard;
    /// <summary>
    /// Property for first card played.
    /// </summary>
    public uint FirstPlayedCard
    {
        get { return firstPlayedCard; }
        set { firstPlayedCard = value; }
    }

    [SyncVar]
    private List<uint> playedCards = null;

    [SyncVar]
    private int countPlayedCards;

    [SyncVar]
    private uint highestCard;

    private void Awake()
    {
        playArea = this;
    }

    /// <summary>
    /// Method that is called when card is played. Gives turn to next player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="card"></param>
    public void PlayCard(Player player, Card card) {

        if (!initiated) {
            InitPlayCardRound();
        }

        NetworkIdentity foundNetworkIdentity = null;
        NetworkIdentity.spawned.TryGetValue(firstPlayedCard, out foundNetworkIdentity);
        if (!foundNetworkIdentity) {
            FirstPlayedCard = card.netId;
            CmdUpdateFirstPlayedCard(FirstPlayedCard);
        }
        playedCards[player.PlayerId] = card.netId;

        CmdUpdatePlayedCards(playedCards);

        countPlayedCards = countPlayedCards + 1;

        CmdUpdateCountPlayedCards(countPlayedCards);
        if (countPlayedCards == Player.players.Count) {
            DecideWhoHadHighestCard();
        } else
        {
            player.CmdMyTurn(player);
        }
        
    }
    /// <summary>
    /// Method that is used to decide which player had highest card with correct suite last mini-round and to give him the cards.
    /// </summary>
    public void DecideWhoHadHighestCard() {    
        this.highestCard = firstPlayedCard;
        CmdUpdateHighestCard(firstPlayedCard);
        Card card;
        int index = 0;
        foreach (uint cardUint in playedCards) {
           
            card = NetworkIdentity.spawned[cardUint].GetComponent<Card>();

            if (card != null) {
                if (card.CardSuite == NetworkIdentity.spawned[FirstPlayedCard].GetComponent<Card>().CardSuite)
                {
                    if (card.CardValue > NetworkIdentity.spawned[highestCard].GetComponent<Card>().CardValue)
                    {
                        highestCard = card.netId;
                        CmdUpdateHighestCard(highestCard);
                        Debug.Log(NetworkIdentity.spawned[highestCard].GetComponent<Card>().OwnerId + " highest");
                        
                    }
                }
            }
            index++;
        }
        CmdMoveToWinnerPlayerHolder();
    }

    [Command]
    private void CmdUpdateHighestCard(uint highestCard)
    {
        RpcUpdateHighestCard(highestCard);
    }

    [ClientRpc]
    private void RpcUpdateHighestCard(uint highestCard)
    {
        this.highestCard = highestCard;
    }

    [Command]
    private void CmdUpdateFirstPlayedCard(uint firstPlayedCard)
    {
        RpcUpdateFirstPlayedCard(firstPlayedCard);
    }
    [ClientRpc]
    private void RpcUpdateFirstPlayedCard(uint firstPlayedCard)
    {
        this.FirstPlayedCard = firstPlayedCard;
    }

    [Command]
    private void CmdUpdatePlayedCards(List<uint> playedCards)
    {
        RpcUpdatePlayedCards(playedCards);
    }
    [ClientRpc]
    private void RpcUpdatePlayedCards(List<uint> playedCards)
    {
        this.playedCards = playedCards;
    }

    [Command]
    private void CmdUpdateCountPlayedCards(int countPlayedCards)
    {
        RpcUpdateCountPlayedCards(countPlayedCards);
    }
    [ClientRpc]
    private void RpcUpdateCountPlayedCards(int countPlayedCards)
    {
        this.countPlayedCards = countPlayedCards;
    }

    [Command]
    public void CmdMoveToWinnerPlayerHolder()
    {
        RpcMoveToWinnerPlayerHolder();
    }

    [ClientRpc]
    private void RpcMoveToWinnerPlayerHolder()
    {
        NetworkIdentity foundNetworkIdentity = null;
        NetworkIdentity.spawned.TryGetValue(highestCard, out foundNetworkIdentity);
        if (!foundNetworkIdentity) { return; }
        Card winnerCard = NetworkIdentity.spawned[highestCard].GetComponent<Card>();
        foreach (Player player in Player.players) {
            player.myTurn = false;
        }
        foreach (Player player in Player.players) {
            
            if (player.hasAuthority) {
                
                
                foreach (CardZone zone in Player.players[winnerCard.OwnerId].GetComponentsInChildren<CardZone>())
                {
                    if (zone.name.Contains("UsedCardHolder"))
                    {
                        foreach (uint cardUintId in playedCards) {
                            if (NetworkIdentity.spawned[cardUintId].GetComponent<Card>().hasAuthority)
                            player.CmdUpdateCard(NetworkIdentity.spawned[cardUintId].GetComponent<Card>(), zone.gameObject);
                           
                        }
                        
                    }
                    Player.players[winnerCard.OwnerId].myTurn = true;
                    
                }
                player.CmdPlayerPlayTime();
            }
        }

        firstPlayedCard = 9999;
        countPlayedCards = 0;
        //GameManagerHearts.gameManager.ResetPlayerPlayTime();
    }
    
    private void InitPlayCardRound() {

        if (playedCards == null) {
            playedCards = new List<uint>();
            for (int i = 0; i < Player.players.Count; i++)
            {
                playedCards.Add(9999);
            }
        }
        initiated = true;
    }
}
