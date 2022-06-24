using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform rectTransform;

    private Transform _target;
    private Vector3 _lastTargetPosition;
    private Vector2 _pos;
    private float _yOffset;
    private Transform _camera;
    private Vector3 _lastCameraPosition;

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (
            _lastCameraPosition == _camera.position &&
            _target && _lastTargetPosition == _target.position
        )
            return;
        SetPosition();
    }

    public void Initialize(Transform target, float yOffset)
    {
        _target = target;
        _yOffset = yOffset;
    }

    public void SetPosition()
    {
        if (!_target) return;
        _pos = Camera.main.WorldToScreenPoint(_target.position);
        _pos.y += _yOffset;
        rectTransform.anchoredPosition = _pos;
        _lastTargetPosition = _camera.position;
    }
}
