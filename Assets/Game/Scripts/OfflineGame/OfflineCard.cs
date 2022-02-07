using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Script for work with card in offline scene.
/// </summary>
public class OfflineCard : MonoBehaviour
{
    private Image img;
    public int index;

    private Sprite face;
    /// <summary>
    /// Property for sprite from front side of card.
    /// </summary>
    public Sprite Face
    {
        get { return face; }
        set { face = value; }
    }

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

    private int ownerId;
    /// <summary>
    /// Property for id, representing which player owns card. 
    /// </summary>
    public int OwnerId
    {
        get { return ownerId; }
        set { ownerId = value; }
    }
    private void Awake()
    {
        img = GetComponent<Image>();
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
    public void setAspect(bool preserve) {
        img.preserveAspect = preserve;
    }
}
