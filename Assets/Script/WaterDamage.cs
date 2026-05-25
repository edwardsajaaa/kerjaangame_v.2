using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Water Damage & Respawn System
/// Taruh script ini di water GameObject (harus punya Collider2D dengan isTrigger = true)
/// Saat player masuk water:
/// 1. Health berkurang
/// 2. Player langsung respawn ke posisi quiz terakhir (RespawnPoint.lastSpawnPosition)
/// </summary>
public class WaterDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1; // Berapa health yang berkurang
    [SerializeField] private float respawnDelay = 0.5f; // Delay sebelum respawn (untuk visual feedback)
    [SerializeField] private float invulnerabilityTime = 1.5f; // Waktu kebal setelah respawn

    private static float lastRespawnTime = 0f; // Static agar shared antar semua water objects

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Cek apakah masih dalam waktu kebal (mencegah trigger berulang)
            if (Time.time - lastRespawnTime < invulnerabilityTime)
            {
                Debug.Log("💧 Player masih kebal, skip water damage");
                return;
            }

            Debug.Log("💧 Player jatuh ke water!");
            StartCoroutine(DamageAndRespawn(collision.gameObject));
        }
    }

    IEnumerator DamageAndRespawn(GameObject player)
    {
        lastRespawnTime = Time.time;

        // 1. Kurangi health
        if (HealthManager.instance != null)
        {
            HealthManager.instance.DecreaseHealth(damageAmount);
            Debug.Log($"💧 Water damage! -{damageAmount} HP");

            // Cek apakah player masih hidup setelah kena damage
            if (!HealthManager.instance.IsAlive())
            {
                Debug.Log("💀 Player mati karena water damage! Game Over ditangani oleh HealthManager.");
                yield break; // Biarkan HealthManager handle game over
            }
        }

        // 2. Tunggu sebentar untuk visual feedback
        yield return new WaitForSeconds(respawnDelay);

        // 3. Respawn player ke posisi quiz terakhir
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.Respawn();
            Debug.Log($"↩️ Player respawned ke posisi quiz terakhir: {RespawnPoint.lastSpawnPosition}");
        }
        else
        {
            // Fallback: langsung pindahkan posisi jika PlayerMovement tidak ditemukan
            player.transform.position = RespawnPoint.lastSpawnPosition;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
            Debug.Log($"↩️ Player respawned (fallback) ke: {RespawnPoint.lastSpawnPosition}");
        }
    }
}
