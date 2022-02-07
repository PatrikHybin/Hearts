using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// Script handles work with cards in online part of the game.
/// </summary>
public class Card : NetworkBehaviour
{
    private Image img;
    public int index;

    private EnumCardValue.CardValue cardValue;
    /// <summary>
    /// Property for card value.
    /// </summary>
    public EnumCardValue.CardValue CardValue
    {
        get { return cardValue; }
        set { cardValue = value; }
    }

    private string cardSuite;
    /// <summary>
    /// Property for card suite.
    /// </summary>
    public string CardSuite
    {
        get { return cardSuite; }
        set { cardSuite = value; }
    }
    
    [SerializeField]
    private int ownerId;
    /// <summary>
    /// Property for id, representing which player owns card. 
    /// </summary>
    public int OwnerId {
        get { return ownerId; }
        set { ownerId = value; }
    }

    private Sprite face;
    /// <summary>
    /// Property for sprite from front side of the card.
    /// </summary>
    public Sprite Face
    {
        get { return face; }
        set { face = value; }
    }

    private Sprite back;
    /// <summary>
    /// Property for sprite from back side of the card.
    /// </summary>
    public Sprite Back
    {
        get { return back; }
        set { back = value; }
    }

    private Card nextCard = null;
    /// <summary>
    /// Property for next card in player hand. 
    /// </summary>
    public Card NextCard {
        get { return nextCard; }
        set { nextCard = value; }
    }

    private Card previousCard = null;
    /// <summary>
    /// Property for previous in player hand. 
    /// </summary>
    public Card PreviousCard
    {
        get { return previousCard; }
        set { previousCard = value; }
    }

    private CardZone currentZone;
    /// <summary>
    /// Property for current zone at which player hovers with chosen card.
    /// </summary>
    public CardZone CurrentZone
    {
        get { return currentZone; }
        set { currentZone = value; }
    }
    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void Start()
    {
        try
        {
            Player player = GetComponentInParent<Player>();
            //Debug.Log(player.PlayerId);
            OwnerId = player.PlayerId;
        }
        catch {}
    }
    /// <summary>
    /// Method for setting visible part of card.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetImage(Sprite sprite) {
        img.sprite = sprite;
        
    }
    /// <summary>
    /// Method of setting aspect of card.
    /// </summary>
    /// <param name="preserve"></param>
    public void SetAspect(bool preserve) {
        img.preserveAspect = preserve;
    }
    /// <summary>
    /// Method for setting current zone for card for every player. 
    /// </summary>
    /// <param name="cardZone"></param>
    public void SetCurrentZone(CardZone cardZone) {
        CmdSetCurrentZone(cardZone);
    }

    [Command]
    private void CmdSetCurrentZone(CardZone cardZone) {
        RpcSetCurrentZone(cardZone);
    }

    [ClientRpc]
    private void RpcSetCurrentZone(CardZone cardZone) {
        currentZone = cardZone;
    }

}
