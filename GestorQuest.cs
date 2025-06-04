using UnityEngine;
using TMPro;

public class GestorQuests : MonoBehaviour
{
    public TMP_Text[] textosQuests;
    public string[] modelosCoches;
    public int reputacionPorDefecto = 1;

    private Quest[] quests = new Quest[3];
    private GestorEncargos gestorEncargos; // Referencia al gestor para sumar reputación

    private void Start()
    {
        gestorEncargos = FindObjectOfType<GestorEncargos>();

        for (int i = 0; i < quests.Length; i++)
        {
            quests[i] = GenerarQuestAleatoria();
        }

        ActualizarUI();
    }

    Quest GenerarQuestAleatoria()
    {
        bool esReparacion = Random.value > 0.5f;

        return new Quest
        {
            tipo = esReparacion ? Quest.TipoObjetivo.RepararModelo : Quest.TipoObjetivo.TuneoModelo,
            modeloObjetivo = modelosCoches[Random.Range(0, modelosCoches.Length)],
            cantidadObjetivo = Random.Range(2, 5),
            progreso = 0,
            recompensaReputacion = reputacionPorDefecto
        };
    }

    public void NotificarEncargoCompletado(string modelo, bool esReparacion)
    {
        for (int i = 0; i < quests.Length; i++)
        {
            quests[i].RegistrarProgreso(modelo, esReparacion);

            if (quests[i].EstaCompletada)
            {
                gestorEncargos.GanarReputacion(quests[i].recompensaReputacion);
                quests[i] = GenerarQuestAleatoria();
            }
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        for (int i = 0; i < textosQuests.Length; i++)
        {
            textosQuests[i].text = quests[i].ObtenerTexto();
        }
    }
}