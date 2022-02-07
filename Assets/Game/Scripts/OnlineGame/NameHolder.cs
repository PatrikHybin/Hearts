using UnityEngine;

/// <summary>
/// Script representing place for player's name in online scene.
/// </summary>
public class NameHolder : MonoBehaviour
{ 
    void Awake()
    {
        Player player = GetComponentInParent<Player>();
        player.AddNameHolder(this.gameObject);    
    }
}
