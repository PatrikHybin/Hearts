using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

/// <summary>
/// Script for manager for catching player's inputs.
/// </summary>
public class InputManager : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    private Card card;

    /// <summary>
    /// When dragging is occurring this will be called every time the cursor is moved.
    /// </summary>
    [Client]
    public void OnDrag(PointerEventData eventData)
    {
        if (card != null) {
            
            if (card.hasAuthority)
            {
                Vector3 screenPoint = eventData.position;
                screenPoint.z = Settings.DistanceCameraPlane.z;
                var vector3 = Camera.main.ScreenToWorldPoint(screenPoint);
                Player.players[card.OwnerId].MoveCard(vector3, card);
            }
        }

    }

    /// <summary>
    /// Use this callback to detect pointer down events.
    /// </summary>
    [Client]
    public void OnPointerDown(PointerEventData eventData)
    {
        card = eventData.pointerCurrentRaycast.gameObject.GetComponent<Card>();

        if (card != null)
        {
            if (card.hasAuthority)
            {
                GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
                //CardManager.cardManager.SetSelectedCard(eventData.pointerCurrentRaycast.gameObject.GetComponent<Card>());
                Player.players[card.OwnerId].SetSelectedCard(card);
                //GetComponentInParent<Player>().SetSelectedCard(card);
            }

        }

    }


    /// <summary>
    /// Use this callback to detect pointer up events.
    /// </summary>
    [Client]
    public void OnPointerUp(PointerEventData eventData)
    {
        if (card != null) {
            if (card.hasAuthority)
            {
                GetComponentInChildren<CanvasGroup>().blocksRaycasts = true;
                Player.players[card.OwnerId].ReleaseCard(card);
                card = null;
            }
        }

    }

}
