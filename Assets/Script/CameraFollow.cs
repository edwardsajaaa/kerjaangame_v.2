using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 10f;

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogError("Player tidak ditemukan pada CameraFollow!");
            return;
        }

        // Target position dengan offset
        Vector3 desiredPosition = player.position + offset;

        // Clamp position agar camera tidak keluar dari boundary
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Smooth follow menggunakan Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
