using UnityEngine;

public class FontToggleButton : MonoBehaviour
{
    public void ToggleFont()
    {
        if (PersistentFontSwitcher.Instance != null)
        {
            PersistentFontSwitcher.Instance.ToggleFonts();
        }
    }
}
