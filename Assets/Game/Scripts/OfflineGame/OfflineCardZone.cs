using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Script of zone where user can place cards in offline scene of the game.
/// </summary>
public class OfflineCardZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Use this callback to detect pointer enter events
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponentInParent<OfflinePlayer>())
        {
            GetComponentInParent<OfflinePlayer>().CardZone = gameObject;
        }   
    }

    /// <summary>
    /// Use this callback to detect pointer exit events
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.GetComponentInParent<OfflinePlayer>())
        {
            GetComponentInParent<OfflinePlayer>().CardZone = null;
        }
        
    }
}
