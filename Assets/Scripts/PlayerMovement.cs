using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _renderer;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _moveSpeed;

    private enum MovementState { idle, runnning, jumping, falling };
    private float _dirX;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _dirX = Input.GetAxisRaw("Horizontal");
        _rigidbody.velocity = new Vector2(_dirX * _moveSpeed, _rigidbody.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpSpeed);
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (_dirX > 0)
        {
            state = MovementState.runnning;
            _renderer.flipX = false;
        }
        else if (_dirX < 0)
        {
            state = MovementState.runnning;
            _renderer.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (_rigidbody.velocity.y > 0.05f)
        {
            state = MovementState.jumping;
        }
        else if (_rigidbody.velocity.y < -0.05f)
        {
            state = MovementState.falling;
        }

        _animator.SetInteger("State", (int)state);
    }
}
