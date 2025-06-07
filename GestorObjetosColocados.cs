using UnityEngine;
using System.Collections.Generic;

public class GestorObjetosColocados : MonoBehaviour
{
    public static GestorObjetosColocados Instancia { get; private set; }

    [System.Serializable]
    public class ObjetoColocadoData
    {
        public int indiceTienda;
        public Vector3 posicion;
    }

    private List<GameObject> objetosColocados = new List<GameObject>();
    private List<ObjetoColocadoData> datosObjetosColocados = new List<ObjetoColocadoData>();

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

        CargarObjetosColocados();
    }

    public void ColocarObjeto(Inventario.ObjetoInventario objeto, Vector3 posicion)
    {
        GameObject nuevoObjeto = Instantiate(objeto.prefabObjeto, posicion, Quaternion.identity);
        objetosColocados.Add(nuevoObjeto);
        datosObjetosColocados.Add(new ObjetoColocadoData { indiceTienda = objeto.indiceTienda, posicion = posicion });

        // Añadir script ObjetoColocado para interacción
        var script = nuevoObjeto.AddComponent<ObjetoColocado>();
        script.Configurar(objeto.indiceTienda);

        GuardarObjetosColocados();
    }

    public void AlmacenarObjeto(GameObject objeto, int indiceTienda)
    {
        int index = objetosColocados.IndexOf(objeto);
        if (index != -1)
        {
            objetosColocados.RemoveAt(index);
            datosObjetosColocados.RemoveAt(index);
        }

        Destroy(objeto);

        // Añadir al inventario usando información del GestorShop
        GestorShop gestorShop = FindObjectOfType<GestorShop>();
        var itemTienda = gestorShop.ObtenerItemTienda(indiceTienda);
        if (itemTienda != null)
        {
            Inventario.Instancia.AñadirObjeto(itemTienda.nombre, itemTienda.precio, 1, itemTienda.prefabObjeto, indiceTienda);
        }

        GuardarObjetosColocados();
    }

    private void GuardarObjetosColocados()
    {
        PlayerPrefs.SetInt("ObjetosColocados_Cantidad", datosObjetosColocados.Count);
        for (int i = 0; i < datosObjetosColocados.Count; i++)
        {
            PlayerPrefs.SetInt($"ObjetoColocado_{i}_IndiceTienda", datosObjetosColocados[i].indiceTienda);
            PlayerPrefs.SetFloat($"ObjetoColocado_{i}_PosX", datosObjetosColocados[i].posicion.x);
            PlayerPrefs.SetFloat($"ObjetoColocado_{i}_PosY", datosObjetosColocados[i].posicion.y);
            PlayerPrefs.SetFloat($"ObjetoColocado_{i}_PosZ", datosObjetosColocados[i].posicion.z);
        }
        PlayerPrefs.Save();
    }

    private void CargarObjetosColocados()
    {
        int cantidad = PlayerPrefs.GetInt("ObjetosColocados_Cantidad", 0);
        GestorShop gestorShop = FindObjectOfType<GestorShop>();

        for (int i = 0; i < cantidad; i++)
        {
            int indiceTienda = PlayerPrefs.GetInt($"ObjetoColocado_{i}_IndiceTienda", -1);
            float posX = PlayerPrefs.GetFloat($"ObjetoColocado_{i}_PosX", 0f);
            float posY = PlayerPrefs.GetFloat($"ObjetoColocado_{i}_PosY", 0f);
            float posZ = PlayerPrefs.GetFloat($"ObjetoColocado_{i}_PosZ", 0f);
            Vector3 posicion = new Vector3(posX, posY, posZ);

            if (indiceTienda >= 0 && gestorShop != null)
            {
                var itemTienda = gestorShop.ObtenerItemTienda(indiceTienda);
                if (itemTienda != null)
                {
                    GameObject nuevoObjeto = Instantiate(itemTienda.prefabObjeto, posicion, Quaternion.identity);
                    objetosColocados.Add(nuevoObjeto);
                    datosObjetosColocados.Add(new ObjetoColocadoData { indiceTienda = indiceTienda, posicion = posicion });

                    var script = nuevoObjeto.AddComponent<ObjetoColocado>();
                    script.Configurar(indiceTienda);
                }
            }
        }
    }
}