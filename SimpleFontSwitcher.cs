using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PersistentFontSwitcher : MonoBehaviour
{
    public TMP_FontAsset font1;
    public TMP_FontAsset font2;
    private bool usingFont1 = true;

    public static PersistentFontSwitcher Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
            usingFont1 = PlayerPrefs.GetInt("CurrentFont", 1) == 1;
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Limpiar suscripción
        }
    }

    public void ToggleFonts()
    {
        usingFont1 = !usingFont1;
        PlayerPrefs.SetInt("CurrentFont", usingFont1 ? 1 : 0); // Guarda preferencia
        PlayerPrefs.Save();
        ApplyCurrentFont();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyCurrentFont(); // Aplica la fuente cada vez que se carga una nueva escena
    }

    private void ApplyCurrentFont()
    {
        var allTexts = FindObjectsOfType<TMP_Text>(true);
        foreach (var text in allTexts)
        {
            text.font = usingFont1 ? font1 : font2;
        }
    }
}
