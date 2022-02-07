using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Script handles timer in offline game. It's for counting down how much time player how in respective phase of the game.
/// </summary>
public class OfflineTimer : MonoBehaviour
{
    /// <summary>
    /// Instance of offline timer.
    /// </summary>
    public static OfflineTimer timer;
    private float currentTime = 0f;
    private float giveTime = 5f;
    private float playTime = 15f;
    private string msg = "start";
    private bool giveCards;
    private bool playCard;
    private bool botsGiveCard;
    private Color giveCardColor;

    private void Awake()
    {
        if (timer == null) {
            timer = this;
        }
    }
    void Start()
    {
        giveCardColor.a = 1; //transparency
        giveCardColor.r = 1;
        giveCardColor.g = 1;
        giveCardColor.b = 1;
    }

    void Update()
    {
        if (msg == "GiveCards" && giveCards && !FindObjectOfType<OfflinePlayer>().endOfGame)
        {
            
            currentTime -= 1 * Time.deltaTime;
            FindObjectOfType<OfflinePlayer>().SetText(currentTime.ToString("0"));
            giveCardColor.a = 1;
            FindObjectOfType<OfflinePlayer>().giveHolder.GetComponent<Image>().color = giveCardColor;
            if (currentTime.ToString("0") == "3" && botsGiveCard)
            {
                botsGiveCard = false;
                OfflineGameManagerHearts.instanceOfflineManager.PlaceRandomCardsForBotsIntoGiveHolder();

            }
            if (currentTime.ToString("0") == "0")
            {
                giveCards = false;
                OfflineGameManagerHearts.instanceOfflineManager.SendGiveHolderCards();
            }
        }
        else {
            giveCardColor.a = 0;
            FindObjectOfType<OfflinePlayer>().giveHolder.GetComponent<Image>().color = giveCardColor;
        }

        if (msg == "PlayerTime" && playCard)
        {
            currentTime -= 1 * Time.deltaTime;
            FindObjectOfType<OfflinePlayer>().SetText(currentTime.ToString("0"));
            // || !FindObjectOfType<OfflinePlayer>().myTurn
            if (currentTime.ToString("0") == "0") {
                playCard = false;
                //FindObjectOfType<OfflinePlayer>().SetText(" ");
                FindObjectOfType<OfflinePlayer>().PlayRandomCard();
            }
            if (!FindObjectOfType<OfflinePlayer>().myTurn) {
                playCard = false;
                FindObjectOfType<OfflinePlayer>().SetText(" ");
            }
        }

    }
    /// <summary>
    /// Method that is used for starting timer based on phase of the game. 
    /// </summary>
    /// <param name="message"></param>
    public void StartTime(string message)
    {
        msg = message;
        if (msg == "GiveCards") {
            currentTime = giveTime;
            giveCards = true;
            botsGiveCard = true;
        }

        if (msg == "PlayerTime") {
            currentTime = playTime;
            playCard = true;
        }
    }
}
