[System.Serializable]
public class Quest
{
    public enum TipoObjetivo { RepararModelo, TuneoModelo }

    public TipoObjetivo tipo;
    public string modeloObjetivo;
    public int cantidadObjetivo;
    public int progreso;
    public int recompensaReputacion;

    public bool EstaCompletada => progreso >= cantidadObjetivo;

    public string ObtenerTexto()
    {
        string accion = tipo == TipoObjetivo.RepararModelo ? "Repara" : "Tunea";
        return $"{accion} {cantidadObjetivo} {modeloObjetivo} ({progreso}/{cantidadObjetivo})";
    }

    public void RegistrarProgreso(string modelo, bool esReparacion)
    {
        if (EstaCompletada) return;

        bool coincide =
            tipo == TipoObjetivo.RepararModelo && esReparacion && modelo == modeloObjetivo ||
            tipo == TipoObjetivo.TuneoModelo && !esReparacion && modelo == modeloObjetivo;

        if (coincide)
            progreso++;
    }
}
