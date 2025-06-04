using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelWindow : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelWindow;  // El panel/ventana que se mostrará
    public Button openButton;      // Botón para abrir el panel
    public Button closeButton;     // Botón dentro del panel para cerrarlo

    // Evento estático para notificar cuando un panel se abre
    public static event Action<GameObject> OnPanelOpened;

    void Start()
    {
        // Configuración inicial
        panelWindow.SetActive(false);

        // Asignar eventos a los botones
        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenPanel);
        }
        else
        {
            Debug.LogError("OpenButton no asignado en el Inspector");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogError("CloseButton no asignado en el Inspector");
        }

        // Suscribirse al evento de panel abierto
        OnPanelOpened += HandlePanelOpened;
    }

    // Abre el panel y notifica a los demás
    public void OpenPanel()
    {
        panelWindow.SetActive(true);
        OnPanelOpened?.Invoke(panelWindow);
    }

    // Cierra el panel
    public void ClosePanel()
    {
        panelWindow.SetActive(false);
    }

    // Maneja el evento de apertura de paneles
    private void HandlePanelOpened(GameObject openedPanel)
    {
        if (openedPanel != panelWindow)
        {
            ClosePanel();
        }
    }

    // Limpieza de eventos
    void OnDestroy()
    {
        if (openButton != null)
        {
            openButton.onClick.RemoveListener(OpenPanel);
        }
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePanel);
        }
        OnPanelOpened -= HandlePanelOpened;
    }
}