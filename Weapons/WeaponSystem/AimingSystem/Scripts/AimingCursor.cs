using UnityEngine;
using UnityEngine.InputSystem;

public class AimingCursor : MonoBehaviour
{

    [SerializeField]
    LayerMask layerMask = Physics.DefaultRaycastLayers;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.point;
        }
    }
}
