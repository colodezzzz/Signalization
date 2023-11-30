using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThiefMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody2D _rigidbody;
    private float _deltaX;
    private bool _isMoving;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _deltaX = Input.GetAxisRaw("Horizontal");
        _isMoving = _deltaX != 0;
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            _rigidbody.velocity = new Vector2(_deltaX * _speed, _rigidbody.velocity.y);
        }
    }
}
