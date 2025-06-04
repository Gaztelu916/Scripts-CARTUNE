using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GestorEncargos : MonoBehaviour
{
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

    private int dinero;
    private int reputacion;
    private Encargo[] encargos = new Encargo[3];
    private bool[] enProceso = new bool[3];
    private float[] tiemposRestantes = new float[3];

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

    private void Start()
    {
        gestorQuests = FindObjectOfType<GestorQuests>();

        dinero = PlayerPrefs.GetInt("Money :", dineroInicial);
        reputacion = PlayerPrefs.GetInt("REP :", reputacionInicial);

        for (int i = 0; i < encargos.Length; i++)
        {
            GenerarEncargo(i);
        }

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
            nuevoEncargo.recompensaReputacion = 1;
            nuevoEncargo.duracion = Random.Range(2f, 5f);
        }
        else
        {
            nuevoEncargo.recompensaDinero = Random.Range(500, 1000);
            nuevoEncargo.recompensaReputacion = 2;
            nuevoEncargo.duracion = Random.Range(10f, 20f);
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
        StartCoroutine(ProcesarEncargo(index));
    }

    IEnumerator ProcesarEncargo(int index)
    {
        Encargo encargo = encargos[index];
        textosEncargos[index].text = $"⏳ {encargo.modelo} en proceso...";

        while (tiemposRestantes[index] > 0)
        {
            tiemposRestantes[index] -= Time.deltaTime;
            contadoresClientes[index].text = $"Client{index + 1}: {tiemposRestantes[index]:F1}s left";
            yield return null;
        }

        dinero += encargo.recompensaDinero;
        reputacion += encargo.recompensaReputacion;

        PlayerPrefs.SetInt("Money :", dinero);
        PlayerPrefs.SetInt("REP :", reputacion);
        PlayerPrefs.Save();

        gestorQuests?.NotificarEncargoCompletado(encargo.modelo, encargo.esReparacion);

        GenerarEncargo(index);
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
        PlayerPrefs.SetInt("REP :", reputacion);
        PlayerPrefs.Save();
        ActualizarUI();
    }

    // ✅ NUEVO método para refrescar dinero desde otro script
    public void ActualizarDinero()
    {
        dinero = PlayerPrefs.GetInt("Money :", 0);
        dineroTexto.text = $"Money: <color=#00FF00>${dinero}</color>";
    }
}
