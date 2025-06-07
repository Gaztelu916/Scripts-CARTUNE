using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [Header("Zoom")]
    public float zoomSpeed = 20f;         // Ajustado para tamaño grande
    public float minZoom = 20f;           // Zoom máximo (más cerca)
    public float maxZoom = 80f;           // Zoom mínimo (más lejos)

    [Header("Pan")]
    public float panSpeed = 0.2f;         // Más lento para precisión

    private Vector3 lastMousePosition;

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed * Time.deltaTime * 100;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(1)) // Botón derecho presionado
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // Botón derecho mantenido
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x * panSpeed * Time.deltaTime, -delta.y * panSpeed * Time.deltaTime, 0f);

            // Convertir de pantalla a mundo
            Camera.main.transform.Translate(move);

            lastMousePosition = Input.mousePosition;
        }
    }
}
