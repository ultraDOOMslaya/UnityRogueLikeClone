using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public float translationSpeed = 60f;
    public float altitude = 40f;
    public float zoomSpeed = 30f;

    private Camera _camera;
    private RaycastHit _hit;
    private Ray _ray;

    private Vector3 _forwardDir;
    private int _mouseOnScreenBorder;
    private Coroutine _mouseOnScreenCoroutine;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _forwardDir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        _mouseOnScreenBorder = -1;
        _mouseOnScreenCoroutine = null;
    }

    void Update()
    {
        if (_mouseOnScreenBorder >= 0)
        {
            _TranslateCamera(_mouseOnScreenBorder);
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
                    _TranslateCamera(0);
            else if (Input.GetKey(KeyCode.RightArrow))
                _TranslateCamera(1);
            else if (Input.GetKey(KeyCode.DownArrow))
                _TranslateCamera(2);
            else if (Input.GetKey(KeyCode.LeftArrow))
                _TranslateCamera(3);
        }

        if (Math.Abs(Input.mouseScrollDelta.y) > 0f)
            _Zoom(Input.mouseScrollDelta.y > 0f ? 1 : -1);
    }

    public void OnMouseEnterScreenBorder(int borderIndex)
    {
        _mouseOnScreenCoroutine = StartCoroutine(_SetMouseOnScreenBorder(borderIndex));
    }

    public void OnMouseExitScreenBorder()
    {
        StopCoroutine(_mouseOnScreenCoroutine);
        _mouseOnScreenBorder = -1;
    }

    private void _Zoom(int zoomDir)
    {
        // apply zoom
        _camera.orthographicSize += zoomDir * Time.deltaTime * zoomSpeed;
        // clamp camera distance
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 8f, 50f);
    }

    private void _TranslateCamera(int dir)
    {
        if (dir == 0)       // top
            transform.Translate(_forwardDir * Time.deltaTime * translationSpeed, Space.World);
        else if (dir == 1)  // right
            transform.Translate(transform.right * Time.deltaTime * translationSpeed);
        else if (dir == 2)  // bottom
            transform.Translate(-_forwardDir * Time.deltaTime * translationSpeed, Space.World);
        else if (dir == 3)  // left
            transform.Translate(-transform.right * Time.deltaTime * translationSpeed);

        // translate camera at proper altitude: cast a ray to the ground
        // and move up the hit point
        _ray = new Ray(transform.position, Vector3.up * -1000f);
        if (Physics.Raycast(
                _ray,
                out _hit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            transform.position = _hit.point + Vector3.up * altitude;
    }

    private IEnumerator _SetMouseOnScreenBorder(int borderIndex)
    {
        yield return new WaitForSeconds(0.3f);
        _mouseOnScreenBorder = borderIndex;
    }
}