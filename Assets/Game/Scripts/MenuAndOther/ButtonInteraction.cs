using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Script for detecting interactions with button.
/// </summary>
public class ButtonInteraction : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// Use this callback to detect pointer down events.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        try
        {
            if (gameObject.name == "Button_Leave") {
                GameObject.Find("SoundManager(Clone)").GetComponent<SoundManager>().soundManager.PlaySound("leaveLobby");
                return;
            } 
            GameObject.Find("SoundManager(Clone)").GetComponent<SoundManager>().soundManager.PlaySound("btnClick");
        }
        catch { }
    }

    
}
