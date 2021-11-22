using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private Animator _animator;
    private SpriteRenderer _renderer;

    [SerializeField] private LayerMask _jumpableGround;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _moveSpeed;
    private enum MovementState { idle, runnning, jumping, falling, ledgeClimb };
    private float _dirX;
    private bool _isFacingRight = true;
    private bool _canMove = true;
    private bool _canFlip = true;
    [SerializeField] private float _gravityScale;

    [Header("Ledge Climb")]
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private Transform _ledgeCheck;

    private float _wallCheckDistance;
    private bool _isTouchingWall = false;
    private bool _isTouchingLedge = false;
    private bool _canClimbLedge = false;
    private bool _ledgeDetected = false;
    private Vector2 _ledgePosBot;
    private Vector2 _ledgePos1;
    private Vector2 _ledgePos2;
    [SerializeField] private Vector2 _ledgeOffset1;
    [SerializeField] private Vector2 _ledgeOffset2;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<BoxCollider2D>();
        _animator = gameObject.GetComponent<Animator>();
        _renderer = gameObject.GetComponent<SpriteRenderer>();

        _wallCheckDistance = _collider.bounds.size.x + 0.05f;
        _rigidbody.gravityScale = _gravityScale;
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        UpdateAnimationState();
        CheckLedgeClimb();
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, CameraControl.Instance.LeftLimit + 0.5f, CameraControl.Instance.RightLimit - 0.5f),
            Mathf.Clamp(transform.position.y, CameraControl.Instance.BottomLimit - 5f, CameraControl.Instance.TopLimit),
            transform.position.z);
    }

    private void CheckInput()
    {
        if (!_canMove)
        {
            Debug.Log("Frozen");
        }
        if (_canMove && _canFlip)
        {
            _dirX = Input.GetAxisRaw("Horizontal");
            _rigidbody.velocity = new Vector2(_dirX * _moveSpeed, _rigidbody.velocity.y);

            if (_dirX > 0)
            {
                _isFacingRight = true;
            }
            else if (_dirX < 0)
            {
                _isFacingRight = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpSpeed);
            }
        }
    }

    private void CheckSurroundings()
    {
        Vector3 facingDirection = (_isFacingRight ? Vector3.right : Vector3.left);
        _isTouchingWall = Physics2D.Raycast(_wallCheck.position, facingDirection, _wallCheckDistance, _jumpableGround);
        _isTouchingLedge = Physics2D.Raycast(_ledgeCheck.position, facingDirection, _wallCheckDistance, _jumpableGround);

        if (_isTouchingWall && !_isTouchingLedge && !_ledgeDetected)
        {
            _ledgeDetected = true;
            _ledgePosBot = _wallCheck.position;
        }
    }

    private void CheckLedgeClimb()
    {
        if (_ledgeDetected && !_canClimbLedge)
        {
            _canClimbLedge = true;

            if (_isFacingRight)
            {
                _ledgePos1 = new Vector2(Mathf.Floor(_ledgePosBot.x + _wallCheckDistance) + _ledgeOffset1.x, Mathf.Floor(_ledgePosBot.y) + _ledgeOffset1.y);
                _ledgePos2 = new Vector2(Mathf.Floor(_ledgePosBot.x + _wallCheckDistance) + _ledgeOffset2.x, Mathf.Floor(_ledgePosBot.y) + _ledgeOffset2.y);
            }
            else
            {
                _ledgePos1 = new Vector2(Mathf.Ceil(_ledgePosBot.x - _wallCheckDistance) - _ledgeOffset1.x, Mathf.Floor(_ledgePosBot.y) + _ledgeOffset1.y);
                _ledgePos2 = new Vector2(Mathf.Ceil(_ledgePosBot.x - _wallCheckDistance) - _ledgeOffset2.x, Mathf.Floor(_ledgePosBot.y) + _ledgeOffset2.y);
            }

            _canMove = false;
            _canFlip = false;

            _rigidbody.gravityScale = 0;
            _rigidbody.velocity = Vector2.zero;
            transform.position = _ledgePos1;
            _animator.SetInteger("State", (int)MovementState.ledgeClimb);
        }
    }

    private void FinishLedgeClimb()
    {
        transform.position = _ledgePos2;
        _rigidbody.gravityScale = _gravityScale;
        _canClimbLedge = false;
        _animator.SetInteger("State", (int)MovementState.idle);
        _canMove = true;
        _canFlip = true;
        _ledgeDetected = false;
    }

    private void UpdateAnimationState()
    {
        if (_canMove && _canFlip)
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

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, 0.1f, _jumpableGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_ledgePos1, _ledgePos2);
        Gizmos.color = Color.red;
        if (_isFacingRight)
        {
            Vector2 wallCheckTarget = new Vector2(_wallCheck.position.x + _wallCheckDistance, _wallCheck.position.y);
            Gizmos.DrawLine(_wallCheck.position, wallCheckTarget);
            Vector2 ledgeCheckTarget = new Vector2(_ledgeCheck.position.x + _wallCheckDistance, _ledgeCheck.position.y);
            Gizmos.DrawLine(_ledgeCheck.position, ledgeCheckTarget);
        }
        else
        {
            Vector2 wallCheckTarget = new Vector2(_wallCheck.position.x - _wallCheckDistance, _wallCheck.position.y);
            Gizmos.DrawLine(_wallCheck.position, wallCheckTarget);
            Vector2 ledgeCheckTarget = new Vector2(_ledgeCheck.position.x - _wallCheckDistance, _ledgeCheck.position.y);
            Gizmos.DrawLine(_ledgeCheck.position, ledgeCheckTarget);
        }
    }
}
