using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AutoScrollingBackground : MonoBehaviour
{
    [Header("Configuración de desplazamiento")]
    [Tooltip("Velocidad de desplazamiento en X e Y")]
    public Vector2 scrollSpeed = new Vector2(0.5f, 0f);
    
    [Tooltip("Si está activado, el fondo se moverá independientemente del Time.timeScale")]
    public bool unscaledTime = false;
    
    private Material backgroundMaterial;
    private Vector2 initialOffset;

    void Start()
    {
        // Obtener el material del SpriteRenderer
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        backgroundMaterial = renderer.material;
        
        // Guardar el offset inicial para restaurarlo después
        initialOffset = backgroundMaterial.mainTextureOffset;
        
        // Configurar el material para repetirse
        backgroundMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
    }

    void Update()
    {
        // Calcular el nuevo offset
        float time = unscaledTime ? Time.unscaledTime : Time.time;
        Vector2 offset = new Vector2(
            time * scrollSpeed.x,
            time * scrollSpeed.y
        );
        
        // Aplicar el offset
        backgroundMaterial.mainTextureOffset = offset;
    }

    void OnDisable()
    {
        // Restaurar el offset original cuando se desactiva
        if (backgroundMaterial != null)
        {
            backgroundMaterial.mainTextureOffset = initialOffset;
        }
    }

    void OnDestroy()
    {
        // Limpieza - importante para evitar memory leaks
        if (backgroundMaterial != null && Application.isPlaying)
        {
            Destroy(backgroundMaterial);
        }
    }
}