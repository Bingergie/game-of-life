using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSpeed;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize,
            _camera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * 100, Time.deltaTime * scrollSpeed);
    }
}