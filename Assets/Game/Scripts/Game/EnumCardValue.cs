using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for working with enum card value.
/// </summary>
public class EnumCardValue
{
    /// <summary>
    /// Enum of card values.
    /// </summary>
    public enum CardValue {
        Default = -1,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        UnderKnave = 11,
        UpperKnave = 12,
        King = 13,
        Ace = 14
    }
    /// <summary>
    /// Returns value of card based on its name.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CardValue GetValue(string value) {
        
        switch (value) {
            case "7":
                return CardValue.Seven;
            case "8":
                return CardValue.Eight;
            case "9":
                return CardValue.Nine;
            case "10":
                return CardValue.Ten;
            case "underKnave":
                return CardValue.UnderKnave;
            case "upperKnave":
                return CardValue.UpperKnave;
            case "king":
                return CardValue.King;
            case "ace":
                return CardValue.Ace;
            default:
                return CardValue.Default;
        }
    }
}
