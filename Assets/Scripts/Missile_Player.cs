using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Missile_Player : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float speed = 7;
    private GameObject[] _potentialTargets;
    private GameObject _target;
    private Rigidbody2D _rb;
    private Vector3 _moveTowards;
    private Vector3 _direction;
    private float _targetDistance;

    private void Awake()
    {
        //Debug.Break();
        DetectTarget();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Movement();
        DestroySelf();
    }

    private void Movement()
    {
        if (!_target)
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }
        else if (_target.transform.position.y >= 7.2f)
        {
            _target = null;
        }
        else
        {
            _direction = _target.transform.position - transform.position;
            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
            _rb.rotation = angle;
        
            _moveTowards = (Vector3.MoveTowards(transform.position, _target.transform.position, speed * Time.deltaTime));
            transform.position = _moveTowards;
        }
    }

    private void DetectTarget()
    {
        _targetDistance = Mathf.Infinity;
        _potentialTargets = GameObject.FindGameObjectsWithTag("Enemy");
        if (_potentialTargets == null) return;

        foreach (var enemy in _potentialTargets)
        {
            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < _targetDistance)
            {
                _targetDistance = distance;
                _target = enemy;
            }
        }
    }

    private void DestroySelf()
    {
        switch (transform.position.y)
        {
            case < -6.0f:
                Destroy(gameObject);
                break;
            case > 8.0f:
                Destroy(gameObject);
                break;
        }

        switch (transform.position.x)
        {
            case > 11f:
                Destroy(gameObject);
                break;
            case < -11f:
                Destroy(gameObject);
                break;
        }
    }
}


