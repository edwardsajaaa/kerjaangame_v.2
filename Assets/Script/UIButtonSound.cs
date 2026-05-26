using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UIButtonSound — Tambahkan script ini ke setiap Button di UI
/// Otomatis memutar suara klik saat button ditekan
/// dan suara hover saat mouse masuk ke button
/// 
/// Setup:
/// - Drag script ini ke GameObject Button
/// - Tidak perlu assign apapun — otomatis terhubung ke AudioManager
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private bool playHoverSound = true; // Centang jika ingin suara hover

    // Dipanggil saat mouse masuk ke area button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHoverSound)
            AudioManager.Hover();
    }

    // Dipanggil saat button diklik
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Click();
    }
}
