[System.Serializable]
public class Encargo
{
    public enum TipoEncargo { Reparacion, Tuneo }

    public TipoEncargo tipo;
    public string modeloCoche; // Ej: "Toyota Supra"
    public int dineroBase;      // Recompensa base
    public int reputacionBase;  // Reputación base
    public float tiempoEntrega; // Segundos para completarse
    public string descripcion;  // Texto descriptivo

    // Constructor para crear encargos fácilmente
    public Encargo(TipoEncargo tipo, string modelo, int dinero, int reputacion, float tiempo, string descripcion)
    {
        this.tipo = tipo;
        this.modeloCoche = modelo;
        this.dineroBase = dinero;
        this.reputacionBase = reputacion;
        this.tiempoEntrega = tiempo;
        this.descripcion = descripcion;
    }
}