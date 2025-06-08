using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public void IrAInitialScene()
    {
        SceneManager.LoadScene("InitialScene");
    }
}
