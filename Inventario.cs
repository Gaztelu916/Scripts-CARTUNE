using UnityEngine;
using System.Collections.Generic;

public class Inventario : MonoBehaviour
{
    public static Inventario Instancia { get; private set; }

    [System.Serializable]
    public class ObjetoInventario
    {
        public string nombre;
        public int precio;
        public int cantidad;
        public GameObject prefabObjeto;
        public int indiceTienda;

        public ObjetoInventario(string nombre, int precio, int cantidad, GameObject prefabObjeto, int indiceTienda)
        {
            this.nombre = nombre;
            this.precio = precio;
            this.cantidad = cantidad;
            this.prefabObjeto = prefabObjeto;
            this.indiceTienda = indiceTienda;
        }
    }

    private List<ObjetoInventario> objetos = new List<ObjetoInventario>();
    public delegate void InventarioCambio();
    public event InventarioCambio OnInventarioCambiado;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CargarInventario();
    }

    public void AñadirObjeto(string nombre, int precio, int cantidad, GameObject prefabObjeto, int indiceTienda)
    {
        ObjetoInventario objetoExistente = objetos.Find(obj => obj.indiceTienda == indiceTienda);
        if (objetoExistente != null)
        {
            objetoExistente.cantidad += cantidad;
        }
        else
        {
            objetos.Add(new ObjetoInventario(nombre, precio, cantidad, prefabObjeto, indiceTienda));
        }

        GuardarInventario();
        NotificarCambioInventario(); // Usar método para invocar el evento
    }

    public List<ObjetoInventario> ObtenerObjetos()
    {
        return objetos;
    }

    public void InstanciarObjeto(ObjetoInventario objeto)
    {
        if (objeto.prefabObjeto != null && objeto.cantidad > 0)
        {
            Instantiate(objeto.prefabObjeto, Vector3.zero, Quaternion.identity);
            objeto.cantidad--;
            GuardarInventario();
            NotificarCambioInventario(); // Usar método para invocar el evento
            Debug.Log($"Instanciado {objeto.nombre}. Cantidad restante: {objeto.cantidad}");
        }
        else
        {
            Debug.LogWarning($"No se puede instanciar {objeto.nombre}: Sin prefab o cantidad insuficiente.");
        }
    }

    // Cambiado de private a public
    public void GuardarInventario()
    {
        PlayerPrefs.SetInt("Inventario_Cantidad", objetos.Count);

        for (int i = 0; i < objetos.Count; i++)
        {
            PlayerPrefs.SetString($"Inventario_{i}_Nombre", objetos[i].nombre);
            PlayerPrefs.SetInt($"Inventario_{i}_Precio", objetos[i].precio);
            PlayerPrefs.SetInt($"Inventario_{i}_Cantidad", objetos[i].cantidad);
            PlayerPrefs.SetInt($"Inventario_{i}_IndiceTienda", objetos[i].indiceTienda);
        }

        PlayerPrefs.Save();
        Debug.Log("Inventario guardado.");
    }

    private void CargarInventario()
    {
        objetos.Clear();
        int cantidad = PlayerPrefs.GetInt("Inventario_Cantidad", 0);
        GestorShop gestorShop = FindObjectOfType<GestorShop>();

        for (int i = 0; i < cantidad; i++)
        {
            string nombre = PlayerPrefs.GetString($"Inventario_{i}_Nombre", "");
            int precio = PlayerPrefs.GetInt($"Inventario_{i}_Precio", 0);
            int cantidadObjeto = PlayerPrefs.GetInt($"Inventario_{i}_Cantidad", 0);
            int indiceTienda = PlayerPrefs.GetInt($"Inventario_{i}_IndiceTienda", -1);

            if (!string.IsNullOrEmpty(nombre) && indiceTienda >= 0 && gestorShop != null)
            {
                var itemTienda = gestorShop.ObtenerItemTienda(indiceTienda);
                if (itemTienda != null)
                {
                    objetos.Add(new ObjetoInventario(nombre, precio, cantidadObjeto, itemTienda.prefabObjeto, indiceTienda));
                }
            }
        }

        Debug.Log($"Inventario cargado con {objetos.Count} objetos.");
    }

    // Nuevo método para invocar el evento
    public void NotificarCambioInventario()
    {
        OnInventarioCambiado?.Invoke();
    }

    public void LimpiarInventario()
    {
        objetos.Clear();
        PlayerPrefs.DeleteKey("Inventario_Cantidad");
        for (int i = 0; i < 100; i++)
        {
            PlayerPrefs.DeleteKey($"Inventario_{i}_Nombre");
            PlayerPrefs.DeleteKey($"Inventario_{i}_Precio");
            PlayerPrefs.DeleteKey($"Inventario_{i}_Cantidad");
            PlayerPrefs.DeleteKey($"Inventario_{i}_IndiceTienda");
        }
        PlayerPrefs.Save();
        NotificarCambioInventario();
        Debug.Log("Inventario limpiado.");
    }
}