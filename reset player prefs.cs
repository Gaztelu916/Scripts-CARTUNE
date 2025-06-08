using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con botones

public class ResetPlayerPrefs : MonoBehaviour
{
    // Opcional: Referencia al botón si quieres asignarlo desde el Inspector
    [SerializeField] private Button resetButton;

    private void Start()
    {
        // Si asignaste el botón en el Inspector, configura el listener automáticamente
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetAllPlayerData);
        }
    }

    // Método público para resetear los datos
    public void ResetAllPlayerData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Todos los datos de juego han sido reseteados.");


    }
}