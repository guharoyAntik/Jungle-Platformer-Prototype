using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    private Vector3 _upperLimit;
    private Vector3 _lowerLimit;

    private bool _goingDown = false;

    [SerializeField] private float _speed = 1f;

    private void Start()
    {
        _upperLimit = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        _lowerLimit = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
    }

    void Update()
    {
        if (_goingDown)
        {
            transform.position = Vector3.Lerp(transform.position, _lowerLimit, Time.deltaTime * _speed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _upperLimit, Time.deltaTime * _speed);
        }

        if (transform.position == _lowerLimit)
        {
            _goingDown = false;
        }
        if (transform.position == _upperLimit)
        {
            _goingDown = true;
        }
    }
}
