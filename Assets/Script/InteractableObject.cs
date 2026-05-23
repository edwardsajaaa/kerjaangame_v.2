using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private GameObject panelToActivate;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    private bool playerInRange = false;
    private GameObject player;
    private CircleCollider2D rangeCollider;

    void Start()
    {
        // Cari player di scene
        player = GameObject.FindWithTag("Player");
        
        if (panelToActivate == null)
        {
            Debug.LogError("Panel tidak di-assign pada InteractableObject!");
        }
        
        // Buat collider untuk range detection
        rangeCollider = GetComponent<CircleCollider2D>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        rangeCollider.radius = interactionRange;
        rangeCollider.isTrigger = true;
    }

    void Update()
    {
        // Update tidak perlu apa-apa, panel sudah auto trigger
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek jika objek yang masuk adalah player
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player mendekati! Panel ditampilkan otomatis.");
            
            // Aktifkan panel secara otomatis
            Interact();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Cek jika player keluar dari range
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player keluar dari range.");
            
            // Nonaktifkan panel ketika player keluar range
            DeactivatePanel();
        }
    }

    void Interact()
    {
        if (panelToActivate != null)
        {
            // Aktifkan panel
            panelToActivate.SetActive(true);
            Debug.Log("Panel diaktifkan!");
        }
    }

    // Function untuk deaktifkan panel (bisa dipanggil dari button di panel)
    public void DeactivatePanel()
    {
        if (panelToActivate != null)
        {
            panelToActivate.SetActive(false);
            Debug.Log("Panel dinonaktifkan!");
        }
    }

    // Optional: visualisasi range di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
