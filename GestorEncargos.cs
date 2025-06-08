using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GestorEncargos : MonoBehaviour
{
    public static GestorEncargos Instancia;

    public delegate void DineroCambiadoHandler(int nuevoDinero);
    public event DineroCambiadoHandler OnDineroCambiado;

    [Header("Referencias UI")]
    public TMP_Text dineroTexto;
    public TMP_Text reputacionTexto;
    public TMP_Text[] textosEncargos;
    public TMP_Text[] contadoresClientes;
    public Button[] botonesAceptar;

    [Header("Configuración")]
    public string[] modelosCoches = { "Ferrari", "Toyota", "Ford", "BMW", "Tesla" };
    [SerializeField] private int dineroInicial = 0;
    [SerializeField] private int reputacionInicial = 0;

    [Header("Prefabs (Cargados desde Resources)")]
    public string carpetaPrefabs = "Prefabs/Coches"; // Ruta donde están los prefabs

    // Posiciones FIJAS donde aparecerán los coches (x, y, z)
    private Vector3[] posicionesSpawn = new Vector3[3]
    {
        new Vector3(-68.5f, 8.5f, 0f),  // Posición del coche del Encargo 0
        new Vector3(-62.1f, 4.7f, 0f),    // Posición del coche del Encargo 1
        new Vector3(-54.1f, 0.5f, 0f)     // Posición del coche del Encargo 2
    };

    private int dinero;
    private int reputacion;
    private Encargo[] encargos = new Encargo[3];
    private bool[] enProceso = new bool[3];
    private float[] tiemposRestantes = new float[3];
    private GameObject[] cochesInstanciados = new GameObject[3]; // Para controlar los coches en escena

    private GestorQuests gestorQuests;

    [System.Serializable]
    private class Encargo
    {
        public string modelo;
        public bool esReparacion;
        public int recompensaDinero;
        public int recompensaReputacion;
        public float duracion;
    }

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        gestorQuests = FindObjectOfType<GestorQuests>();

        dinero = PlayerPrefs.GetInt(PlayerPrefsKeys.Dinero, dineroInicial);
        reputacion = PlayerPrefs.GetInt(PlayerPrefsKeys.Reputacion, reputacionInicial);

        // Generar encargos iniciales
        for (int i = 0; i < encargos.Length; i++)
        {
            GenerarEncargo(i);
        }

        // Configurar botones
        for (int i = 0; i < botonesAceptar.Length; i++)
        {
            int index = i;
            botonesAceptar[i].onClick.AddListener(() => AceptarEncargo(index));
        }

        ActualizarUI();
    }

    void GenerarEncargo(int index)
    {
        Encargo nuevoEncargo = new Encargo
        {
            modelo = modelosCoches[Random.Range(0, modelosCoches.Length)],
            esReparacion = Random.value > 0.5f
        };

        if (nuevoEncargo.esReparacion)
        {
            nuevoEncargo.recompensaDinero = Random.Range(100, 300);
            nuevoEncargo.recompensaReputacion = 0; // No reputation reward
            nuevoEncargo.duracion = Random.Range(30f, 60f); // Minimum 30 seconds
        }
        else
        {
            nuevoEncargo.recompensaDinero = Random.Range(500, 1000);
            nuevoEncargo.recompensaReputacion = 0; // No reputation reward
            nuevoEncargo.duracion = Random.Range(30f, 90f); // Minimum 30 seconds
        }

        encargos[index] = nuevoEncargo;
        enProceso[index] = false;
        tiemposRestantes[index] = 0f;
        ActualizarTextoEncargo(index);
    }

    void ActualizarTextoEncargo(int index)
    {
        Encargo encargo = encargos[index];
        string tipo = encargo.esReparacion ? "Reparación" : "Tuneo";

        textosEncargos[index].text =
            $"<b>{tipo} en {encargo.modelo}</b>\n" +
            $"Duración: {encargo.duracion:F1}s\n" +
            $"Recompensa: <color=#FFD700>${encargo.recompensaDinero}</color>";

        botonesAceptar[index].interactable = !enProceso[index];
        contadoresClientes[index].text = enProceso[index] ? $"Client{index + 1}: {tiemposRestantes[index]:F1}s left" : "";
    }

    void AceptarEncargo(int index)
    {
        if (enProceso[index]) return;

        enProceso[index] = true;
        tiemposRestantes[index] = encargos[index].duracion;
        ActualizarTextoEncargo(index);

        // Cargar el prefab del coche desde Resources
        GameObject prefabCoche = Resources.Load<GameObject>($"{carpetaPrefabs}/{encargos[index].modelo}");
        
        if (prefabCoche != null)
        {
            // Destruir coche anterior si existe
            if (cochesInstanciados[index] != null)
            {
                Destroy(cochesInstanciados[index]);
            }

            // Instanciar el coche en su posición fija asignada
            cochesInstanciados[index] = Instantiate(
                prefabCoche,
                posicionesSpawn[index],  // Posición hardcodeada según el índice
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogError($"No se encontró el prefab en: Resources/{carpetaPrefabs}/{encargos[index].modelo}");
        }

        StartCoroutine(ProcesarEncargo(index));
    }

    IEnumerator ProcesarEncargo(int index)
    {
        Encargo encargo = encargos[index];
        textosEncargos[index].text = $"⏳ {encargo.modelo} en proceso...";

        // Esperar a que termine el temporizador
        while (tiemposRestantes[index] > 0)
        {
            tiemposRestantes[index] -= Time.deltaTime;
            contadoresClientes[index].text = $"Client{index + 1}: {tiemposRestantes[index]:F1}s left";
            yield return null;
        }

        // Recompensas
        dinero += encargo.recompensaDinero;
        reputacion += encargo.recompensaReputacion;

        PlayerPrefs.SetInt(PlayerPrefsKeys.Dinero, dinero);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Reputacion, reputacion);
        PlayerPrefs.Save();

        OnDineroCambiado?.Invoke(dinero);

        // Notificar al gestor de quests (si existe)
        gestorQuests?.NotificarEncargoCompletado(encargo.modelo, encargo.esReparacion);

        // Destruir el coche al completar el encargo
        if (cochesInstanciados[index] != null)
        {
            Destroy(cochesInstanciados[index]);
            cochesInstanciados[index] = null;
        }

        GenerarEncargo(index); // Generar nuevo encargo
        ActualizarUI();
    }

    void ActualizarUI()
    {
        dineroTexto.text = $"Money: <color=#00FF00>${dinero}</color>";
        reputacionTexto.text = $"REP: <color=#FFFF00>{reputacion}</color>";

        for (int i = 0; i < encargos.Length; i++)
        {
            ActualizarTextoEncargo(i);
        }
    }

    public void GanarReputacion(int cantidad)
    {
        reputacion += cantidad;
        PlayerPrefs.SetInt(PlayerPrefsKeys.Reputacion, reputacion);
        PlayerPrefs.Save();
        ActualizarUI();
    }

    public void ActualizarDinero()
    {
        dinero = PlayerPrefs.GetInt(PlayerPrefsKeys.Dinero, 0);
        dineroTexto.text = $"Money: <color=#00FF00>${dinero}</color>";
        OnDineroCambiado?.Invoke(dinero);
    }
}