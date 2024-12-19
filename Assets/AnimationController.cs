using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private float _minImpactForce = 20;
    
    [SerializeField] private float _landAnimDuration = 0.1f;
    [SerializeField] private float _attackAnimTime = 0.2f;

    private IPlayerController _player;
    private Animator _anim;
    private SpriteRenderer _renderer;
    
    private float _lockedTill;
    private bool _jumpTriggered;
    private bool _walked;
    private bool _crouched;

    private void Awake()
    {
        if (!TryGetComponent(out IPlayerController player))
        {
            Destroy(this);
            return;
        }
        
        _player = player;
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _player.Jumped += () =>
        {
            _jumpTriggered = true;
        };

        _player.Crouched += () =>
        {
            _crouched = true;
        };

        _player.Walked += () =>
        {
            _walked = true;
        };

    }

    private void Update()
    {
        if (_player.Speed.y == 0)
        {
            _player.isJumped = false;
        }
        
        if (_player.Input.x != 0)
        {
            _renderer.flipX = _player.Input.x < 0;
        }

        var state = GetState();

        _jumpTriggered = false;
        _crouched = false;
        _walked = false;

        if (state == _currentState)
        {
            return;
        }
        
        _anim.CrossFade(state, 0,0);
        _currentState = state;
    }

    private int GetState()
    {
        // if (Time.time < _lockedTill)
        //     return _currentState;

        if (_player.Crouching)
            return Crouch;

        if (_jumpTriggered)
            return Jump;

        if (_walked)
            return _player.Input.x == 0 ? Idle : Walk;

        return _player.Speed.y > 0 ? Jump : Idle;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }

    #region Cached Properties

    private int _currentState;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Crouch = Animator.StringToHash("Crouch");

    #endregion
    
}

public interface IPlayerController {
    public Vector2 Input { get; }
    public Vector2 Speed { get; }
    public bool Crouching { get; }
    
    public bool isJumped { get; set; }

    public event Action Walked; // Grounded - Impact force
    public event Action Jumped;
    public event Action Crouched;
    
}
