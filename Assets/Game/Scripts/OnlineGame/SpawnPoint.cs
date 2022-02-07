using UnityEngine;

/// <summary>
/// Script for work with spawn points.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        SpawnSystem.AddSpawnPoint(transform);
    }

    private void OnDestroy()
    {
        SpawnSystem.RemoveSpawnPoint(transform);
    }
}
