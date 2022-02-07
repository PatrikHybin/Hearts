using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Script for unselecting buttons.
/// </summary>
public class ButtonSelectReset : MonoBehaviour
{
    /// <summary>
    /// Method used for deselecting button in lobby
    /// </summary>
    public void UnselectButton() {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
