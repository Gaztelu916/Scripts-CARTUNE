using UnityEngine;

public class ForzarResolucion : MonoBehaviour
{
    [Header("Resolución deseada")]
    public int ancho = 540;    
    public int alto = 960;
    public bool pantallaCompleta = false;

    void Awake()
    {
        // Esto forzará la resolución ANTES de que se muestre la ventana si es posible
        Screen.SetResolution(ancho, alto, pantallaCompleta);

        // Opcional: sobreescribir PlayerPrefs por si Unity los vuelve a guardar
        PlayerPrefs.SetInt("Screenmanager Resolution Width", ancho);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", alto);
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", pantallaCompleta ? 1 : 0);
        PlayerPrefs.Save();
    }
}
