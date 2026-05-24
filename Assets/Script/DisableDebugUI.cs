using UnityEngine;
using UnityEngine.Rendering;

public class DisableDebugUI : MonoBehaviour
{
    void Awake()
    {
        // Disable debug runtime UI yang muncul saat play
        DebugManager.instance.enableRuntimeUI = false;
        Debug.Log("[DEBUG] Runtime UI disabled!");
    }
}
