using UnityEngine;

/// <summary>
/// Script for object which holds score in online scene.
/// </summary>
public class ScoreHolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Player player = GetComponentInParent<Player>();
            player.AddScoreHolder(this.gameObject);
        }
        catch { }
       
    }

}
