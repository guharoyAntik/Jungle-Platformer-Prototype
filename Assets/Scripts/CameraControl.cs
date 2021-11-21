using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothSpeed = 0.125f;

    [SerializeField] private float _leftLimit;
    [SerializeField] private float _rightLimit;
    [SerializeField] private float _topLimit;
    [SerializeField] private float _bottomLimit;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        float height = Camera.main.orthographicSize * 2f;
        float width = Camera.main.aspect * height;

        smoothedPosition = new Vector3
        (
            Mathf.Clamp(smoothedPosition.x, _leftLimit + width / 2, _rightLimit - width / 2),
            Mathf.Clamp(smoothedPosition.y, _bottomLimit + height / 2, _topLimit - height / 2),
            smoothedPosition.z
        );

        transform.position = smoothedPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector2(_leftLimit, _topLimit), new Vector2(_rightLimit, _topLimit));
        Gizmos.DrawLine(new Vector2(_leftLimit, _bottomLimit), new Vector2(_rightLimit, _bottomLimit));
        Gizmos.DrawLine(new Vector2(_leftLimit, _topLimit), new Vector2(_leftLimit, _bottomLimit));
        Gizmos.DrawLine(new Vector2(_rightLimit, _topLimit), new Vector2(_rightLimit, _bottomLimit));
    }
}
