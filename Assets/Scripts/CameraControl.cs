using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothSpeed = 0.125f;

    public float LeftLimit;
    public float RightLimit;
    public float TopLimit;
    public float BottomLimit;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        float height = Camera.main.orthographicSize * 2f;
        float width = Camera.main.aspect * height;

        smoothedPosition = new Vector3
        (
            Mathf.Clamp(smoothedPosition.x, LeftLimit + width / 2, RightLimit - width / 2),
            Mathf.Clamp(smoothedPosition.y, BottomLimit + height / 2, TopLimit - height / 2),
            smoothedPosition.z
        );

        transform.position = smoothedPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector2(LeftLimit, TopLimit), new Vector2(RightLimit, TopLimit));
        Gizmos.DrawLine(new Vector2(LeftLimit, BottomLimit), new Vector2(RightLimit, BottomLimit));
        Gizmos.DrawLine(new Vector2(LeftLimit, TopLimit), new Vector2(LeftLimit, BottomLimit));
        Gizmos.DrawLine(new Vector2(RightLimit, TopLimit), new Vector2(RightLimit, BottomLimit));
    }
}
