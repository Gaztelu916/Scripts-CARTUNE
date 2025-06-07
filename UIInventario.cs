using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIInventario : MonoBehaviour
{
    public GameObject contenedorContenido; // ScrollView/Viewport/Content
    public GameObject prefabBotonInventario;

    private void OnDisable()
    {
        if (Inventario.Instancia != null)
            Inventario.Instancia.OnInventarioCambiado -= ActualizarInventario;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Inventario.Instancia != null);

        ActualizarInventario();
        Inventario.Instancia.OnInventarioCambiado += ActualizarInventario;
    }

    void ActualizarInventario()
    {
        // Eliminar botones anteriores
        foreach (Transform hijo in contenedorContenido.transform)
        {
            Destroy(hijo.gameObject);
        }

        foreach (var objeto in Inventario.Instancia.ObtenerObjetos())
        {
            GameObject nuevoBoton = Instantiate(prefabBotonInventario, contenedorContenido.transform);

            // Sprite
            var image = nuevoBoton.transform.Find("ImagenObjeto")?.GetComponent<Image>();
            if (image != null && objeto.prefabObjeto != null)
            {
                Sprite sprite = objeto.prefabObjeto.GetComponent<SpriteRenderer>()?.sprite;
                if (sprite != null)
                    image.sprite = sprite;
            }

            // Texto cantidad
            var textoCantidad = nuevoBoton.transform.Find("TextoCantidad")?.GetComponent<TMP_Text>();
            if (textoCantidad != null)
            {
                textoCantidad.text = $"{objeto.cantidad}/3";
            }

            // Añadir funcionalidad de clic para iniciar colocación
            nuevoBoton.GetComponent<Button>().onClick.AddListener(() => PlacerObjetos.Instancia.IniciarColocacion(objeto));
        }
    }
}