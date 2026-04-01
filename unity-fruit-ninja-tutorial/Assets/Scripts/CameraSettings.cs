using UnityEngine;
using UnityEngine.UI;

public class CameraSettings : MonoBehaviour
{
    public static bool MirrorX { get; private set; } = false;

    [SerializeField] private Toggle mirrorToggle;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        MirrorX = PlayerPrefs.GetInt("MirrorX", 0) == 1;

        if (mirrorToggle != null)
        {
            mirrorToggle.isOn = MirrorX;
            mirrorToggle.onValueChanged.AddListener(OnMirrorChanged);
        }
    }

    private void OnMirrorChanged(bool isOn)
    {
        MirrorX = isOn;
        PlayerPrefs.SetInt("MirrorX", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnStartClicked()
    {
        // nothing to do
    }
}