using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Referencias a los botones TMP
    [SerializeField] private Button ButtonPlay;
    [SerializeField] private Button ButtonSettings;
    [SerializeField] private Button ButtonExit;

    private void Start()
    {
        // Asignar los listeners a los botones
        if (ButtonPlay != null)
        {
            ButtonPlay.onClick.AddListener(LoadGameScene);
        }
        else
        {
            Debug.LogWarning("ButtonPlay no está asignado en el inspector");
        }

        if (ButtonSettings != null)
        {
            ButtonSettings.onClick.AddListener(LoadOptionsScene);
        }
        else
        {
            Debug.LogWarning("ButtonSettings no está asignado en el inspector");
        }

        if (ButtonExit != null)
        {
            ButtonExit.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogWarning("ButtonExit no está asignado en el inspector");
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void LoadOptionsScene()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    private void ExitGame()
    {
        #if UNITY_EDITOR
            // Si estamos en el editor, detenemos la reproducción
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si estamos en una build, cerramos la aplicación
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Limpiar los listeners para evitar memory leaks
        if (ButtonPlay != null)
        {
            ButtonPlay.onClick.RemoveListener(LoadGameScene);
        }

        if (ButtonSettings != null)
        {
            ButtonSettings.onClick.RemoveListener(LoadOptionsScene);
        }

        if (ButtonExit != null)
        {
            ButtonExit.onClick.RemoveListener(ExitGame);
        }
    }
}