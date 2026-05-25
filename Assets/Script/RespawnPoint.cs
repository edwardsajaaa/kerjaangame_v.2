using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RespawnPoint Manager
/// Menyimpan posisi spawn terakhir yang valid (dari quiz tanpa timer)
/// </summary>
public class RespawnPoint : MonoBehaviour
{
    public static Vector3 lastSpawnPosition;

    void Start()
    {
        // Set default spawn di starting position ini
        lastSpawnPosition = transform.position;
        Debug.Log($"🚩 Default spawn point set: {lastSpawnPosition}");
    }
}
