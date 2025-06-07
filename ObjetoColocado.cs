using UnityEngine;

public class ObjetoColocado : MonoBehaviour
{
    private int indiceTienda;

    public void Configurar(int indice)
    {
        indiceTienda = indice;
    }

    void OnMouseDown()
    {
        GestorObjetosColocados.Instancia.AlmacenarObjeto(gameObject, indiceTienda);
    }
}