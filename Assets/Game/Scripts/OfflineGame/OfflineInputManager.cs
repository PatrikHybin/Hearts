using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script which handles user picking up, releasing and dragging cards in offline part of the game.
/// </summary>
public class OfflineInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        //Screen space - camera
        Vector3 screenPoint = eventData.position;
        screenPoint.z = Settings.DistanceCameraPlane.z;
        var vector3 = Camera.main.ScreenToWorldPoint(screenPoint);
        if (gameObject.GetComponentInParent<OfflinePlayer>()) {
            gameObject.GetComponentInParent<OfflinePlayer>().MoveCard(vector3);
        }
        //CardManager.cardManager.MoveCard(vector3);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down " + gameObject.name);
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<OfflineCard>() != null)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (gameObject.GetComponentInParent<OfflinePlayer>())
            {
                gameObject.GetComponentInParent<OfflinePlayer>().SetSelectedCard(eventData.pointerCurrentRaycast.gameObject.GetComponent<OfflineCard>());
            }
            
        } 
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (gameObject.GetComponentInParent<OfflinePlayer>())
        {
            gameObject.GetComponentInParent<OfflinePlayer>().ReleaseCard();
        }
        //CardManager.cardManager.ReleaseCard();
    }
}
