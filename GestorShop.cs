using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestorShop : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text[] textosItems;
    public Button[] botonesComprar;

    [Header("Configuración de la Tienda")]
    [SerializeField] private ItemTienda[] itemsTienda;

    private GestorEncargos gestorEncargos;

    [System.Serializable]
    public class ItemTienda
    {
        public string nombre;
        public int precio;
        public GameObject prefabObjeto; // Prefab para instanciar el objeto
        [HideInInspector] public int unidadesCompradas;
    }

    private void Start()
    {
        gestorEncargos = Object.FindFirstObjectByType<GestorEncargos>();
        if (gestorEncargos == null)
        {
            Debug.LogError("GestorEncargos no encontrado.");
        }

        // Cargar unidades compradas desde PlayerPrefs
        for (int i = 0; i < itemsTienda.Length; i++)
        {
            itemsTienda[i].unidadesCompradas = PlayerPrefs.GetInt($"Item_{i}_Compras", 0);
        }

        // Configurar botones de la tienda
        for (int i = 0; i < botonesComprar.Length; i++)
        {
            int index = i;
            botonesComprar[i].onClick.AddListener(() => ComprarItem(index));
            ActualizarTextoItem(index);
        }
    }

    private void ActualizarTextoItem(int index)
    {
        if (index >= itemsTienda.Length) return;

        ItemTienda item = itemsTienda[index];
        int unidadesRestantes = 3 - item.unidadesCompradas;

        textosItems[index].text =
            $"<b>{item.nombre}</b>\n" +
            $"<color=#FFD700>Precio: ${item.precio}</color>\n" +
            $"Comprados: {item.unidadesCompradas}/3";

        botonesComprar[index].interactable = gestorEncargos != null &&
                                             ObtenerDinero() >= item.precio &&
                                             unidadesRestantes > 0;
    }

    private int ObtenerDinero()
    {
        return PlayerPrefs.GetInt("Money", 0);
    }

    private void ComprarItem(int index)
    {
        if (index >= itemsTienda.Length) return;

        ItemTienda item = itemsTienda[index];
        int dineroActual = ObtenerDinero();

        if (dineroActual >= item.precio && item.unidadesCompradas < 3)
        {
            int nuevoDinero = dineroActual - item.precio;
            PlayerPrefs.SetInt("Money", nuevoDinero);

            item.unidadesCompradas++;
            PlayerPrefs.SetInt($"Item_{index}_Compras", item.unidadesCompradas);

            // Añadir al inventario
            Inventario.Instancia?.AñadirObjeto(item.nombre, item.precio, 1, item.prefabObjeto, index);

            // Actualizar UI del dinero
            gestorEncargos?.ActualizarDinero();

            // Actualizar todos los textos de la tienda
            for (int i = 0; i < botonesComprar.Length; i++)
            {
                ActualizarTextoItem(i);
            }

            Debug.Log($"¡Comprado {item.nombre} por ${item.precio}! Unidades: {item.unidadesCompradas}/3");
        }
        else
        {
            Debug.LogWarning("No tienes suficiente dinero o el límite de compras (3) fue alcanzado.");
        }
    }

    public ItemTienda ObtenerItemTienda(int index)
    {
        if (index >= 0 && index < itemsTienda.Length)
        {
            return itemsTienda[index];
        }
        return null;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < botonesComprar.Length; i++)
        {
            botonesComprar[i].onClick.RemoveAllListeners();
        }
    }
}