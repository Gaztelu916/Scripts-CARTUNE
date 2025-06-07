using UnityEngine;

public class PlacerObjetos : MonoBehaviour
{
    public static PlacerObjetos Instancia;

    [Header("Zona válida para colocar objetos")]
    public Collider2D areaColocacion;

    private GameObject objetoFantasma;
    private Inventario.ObjetoInventario objetoAColocar;

    void Awake()
    {
        if (Instancia == null)
            Instancia = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (objetoFantasma == null) return;

        Vector3 posicion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posicion.z = 0f;
        objetoFantasma.transform.position = posicion;

        bool dentroArea = areaColocacion.OverlapPoint(posicion);
        bool espacioLibre = EspacioDisponible(posicion);
        bool valido = dentroArea && espacioLibre;

        SetColor(objetoFantasma, valido ? Color.green : Color.red);

        if (Input.GetMouseButtonDown(0) && valido)
        {
            ColocarObjeto(posicion);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cancelar();
        }
    }

    public void IniciarColocacion(Inventario.ObjetoInventario objeto)
    {
        if (objeto.cantidad <= 0 || objeto.prefabObjeto == null)
        {
            Debug.LogWarning("No hay objetos para colocar o prefab inválido");
            return;
        }

        objetoAColocar = objeto;
        objetoFantasma = Instantiate(objeto.prefabObjeto);
        SetColor(objetoFantasma, Color.green);

        foreach (var col in objetoFantasma.GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
    }

    void ColocarObjeto(Vector3 posicion)
    {
        // Usar GestorObjetosColocados para colocar el objeto
        GestorObjetosColocados.Instancia.ColocarObjeto(objetoAColocar, posicion);

        // Reducir cantidad en inventario y notificar cambio
        objetoAColocar.cantidad--;
        Inventario.Instancia.GuardarInventario();
        Inventario.Instancia.NotificarCambioInventario(); // Usar el método público

        Destroy(objetoFantasma);
        objetoFantasma = null;
    }

    void Cancelar()
    {
        if (objetoFantasma != null)
        {
            Destroy(objetoFantasma);
            objetoFantasma = null;
        }
    }

    bool EspacioDisponible(Vector3 posicion)
    {
        var prefabCollider = objetoAColocar.prefabObjeto.GetComponent<Collider2D>();
        if (prefabCollider == null) return true;

        Vector2 centro = posicion;
        Vector2 size = prefabCollider.bounds.size;

        Collider2D[] colisiones = Physics2D.OverlapBoxAll(centro, size, 0f);
        foreach (var col in colisiones)
        {
            if (col.gameObject != areaColocacion.gameObject)
            {
                return false;
            }
        }
        return true;
    }

    void SetColor(GameObject obj, Color color)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = color;
    }
}