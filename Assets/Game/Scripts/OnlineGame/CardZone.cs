using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class CardZone : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Script representing zone where players in online game can place their card.
    /// </summary>
    private int ownerId;
   
    private void Start()
    {
        if (gameObject.name.Contains("UsedCardHolder")) {
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        try
        {
            Player player = GetComponentInParent<Player>();
            ownerId = player.PlayerId;
        } catch {}
        
    }

    /// <summary>
    /// Use this callback to detect pointer enter events
    /// </summary>
    [Client]
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasAuthority)
        {
            Player.players[ownerId].CardZone = gameObject;
            //GetComponentInParent<Player>().CardZone = gameObject;
        }

    }

    /// <summary>
    /// Use this callback to detect pointer exit events
    /// </summary>
    [Client]
    public void OnPointerExit(PointerEventData eventData)
    {

        if (hasAuthority)
        {
            Player.players[ownerId].CardZone = null;
            //GetComponentInParent<Player>().CardZone = null;
        }

    }

}
