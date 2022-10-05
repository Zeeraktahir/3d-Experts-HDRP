#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [Space]
    [SerializeField] Toggle toggleFreePivot;
    [SerializeField] CameraControl freePivotController;

    [System.NonSerialized] bool initialized; // Not serialized to detect domain reload as UI event references are not serialized
    void OnEnable()
    {
        if (!initialized)
            Initialize();
    }

    void Start()
    {
        SetDefaults();
    }

    void Initialize()
    {
        toggleFreePivot.onValueChanged.AddListener(HandleFreePivotModeToggle);
        initialized = true;
    }

    void SetDefaults()
    {
        ForceToggleValue(toggleFreePivot, true);
    }

    void ForceToggleValue(Toggle toggle, bool shouldBeOn)
    {
        if (toggle.isOn == shouldBeOn)
        {
            toggle.onValueChanged.Invoke(shouldBeOn);
        }
        else
        {
            toggle.isOn = shouldBeOn;
        }
    }

    void HandleFreePivotModeToggle(bool enable)
    {
        if (enable)
        {
            freePivotController.ControlEnable();
        }
    }
}