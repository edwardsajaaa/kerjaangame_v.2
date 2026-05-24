using UnityEngine;

public class DebugDisabler : MonoBehaviour
{
    void Start()
    {
        // Cari dan disable [Debug Updater]
        GameObject debugUpdater = GameObject.Find("[Debug Updater]");
        if (debugUpdater != null)
        {
            debugUpdater.SetActive(false);
            Debug.Log("[FIXED] Debug Updater disabled!");
        }

        // Atau hapus langsung
        // if (debugUpdater != null) Destroy(debugUpdater);
    }
}
