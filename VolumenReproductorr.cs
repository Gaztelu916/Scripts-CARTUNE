using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        // Establece el valor inicial del slider según el volumen actual
        if (PersistentMusicPlayer.Instance != null)
        {
            volumeSlider.value = PersistentMusicPlayer.Instance.GetVolume();
        }

        // Escucha cambios
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        if (PersistentMusicPlayer.Instance != null)
        {
            PersistentMusicPlayer.Instance.SetVolume(value);
        }
    }
}
