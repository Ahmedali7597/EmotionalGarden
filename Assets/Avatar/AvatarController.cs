using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Locomotion and animation engine for the avatar.
/// Handles Rigidbody2D movement, animator parameter driving, bounds clamping.
/// Does NOT auto-roam — AvatarRandomWander manages roaming externally.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AvatarController : MonoBehaviour
{
    // ── Animator parameter hashes ─────────────────────────────────────
    private static readonly int HashMoveX      = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY      = Animator.StringToHash("MoveY");
    private static readonly int HashIsMoving   = Animator.StringToHash("IsMoving");
    private static readonly int HashLastMoveX  = Animator.StringToHash("LastMoveX");
    private static readonly int HashLastMoveY  = Animator.StringToHash("LastMoveY");
    private static readonly int HashIsWatering = Animator.StringToHash("IsWatering");
    private static readonly int HashWaterX     = Animator.StringToHash("WaterX");
    private static readonly int HashWaterY     = Animator.StringToHash("WaterY");

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Garden Bounds")]
    [SerializeField] private Collider2D gardenBounds;

    // ── Runtime state ─────────────────────────────────────────────────
    private Animator     _animator;
    private Rigidbody2D  _rb;
    private Vector2      _lastDirection    = Vector2.down;
    private Vector2      _currentCardinal  = Vector2.down;
    private Vector2      _targetPosition;
    private bool         _hasTarget;
    private bool         _isWatering;
    private AvatarAction _pendingAction;
    private bool         _hasArrivalFacing;
    private Vector2      _arrivalFacing;
    private Coroutine    _waterCoroutine;

    // ── Public read properties ────────────────────────────────────────
    public bool         IsWatering   => _isWatering;
    public bool         HasTarget    => _hasTarget;
    public bool         IsBusy       => _isWatering || _hasTarget;
    public Vector2      LastDirection => _lastDirection;
    public Collider2D   GardenBounds => gardenBounds;

    // ── Events ────────────────────────────────────────────────────────
    /// <summary>Fired when the avatar finishes walking to its target. Carries the action that was requested.</summary>
    public event Action<AvatarAction> OnArrived;
    /// <summary>Fired when a Water animation sequence completes.</summary>
    public event Action OnWateringComplete;

    // ─────────────────────────────────────────────────────────────────
    #region Unity Messages

    private void Awake()
    {
        _animator          = GetComponent<Animator>();
        _rb                = GetComponent<Rigidbody2D>();
        _rb.gravityScale   = 0f;
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (_isWatering) return;

        if (_hasTarget)
        {
            Vector2 pos  = _rb.position;
            Vector2 diff = _targetPosition - pos;

            if (diff.magnitude < 0.15f)
            {
                // Arrived
                _rb.linearVelocity = Vector2.zero;
                _hasTarget         = false;

                // Apply arrival facing override if supplied
                if (_hasArrivalFacing)
                {
                    _lastDirection    = _arrivalFacing;
                    _hasArrivalFacing = false;
                }

                SetAnimatorIdle();
                AvatarAction arrivedAction = _pendingAction;
                OnArrived?.Invoke(arrivedAction);

                if (arrivedAction == AvatarAction.WalkAndWater)
                {
                    if (_waterCoroutine != null) StopCoroutine(_waterCoroutine);
                    _waterCoroutine = StartCoroutine(WaterRoutine());
                }
                return;
            }

            Vector2 dir  = ChooseCardinalDirection(diff);
            _currentCardinal   = dir;
            _lastDirection     = dir;
            _rb.linearVelocity = dir * moveSpeed;
            SetAnimatorMoving(dir);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            SetAnimatorIdle();
        }

        ClampToBounds();
    }

    #endregion

    // ─────────────────────────────────────────────────────────────────
    #region Public API

    /// <summary>
    /// Set the position the avatar should walk toward.
    /// </summary>
    /// <param name="position">World-space destination.</param>
    /// <param name="action">What to do on arrival.</param>
    /// <param name="arrivalFacing">
    ///   If supplied, overrides _lastDirection on arrival so idle/water
    ///   animations face the overall start→dest direction rather than
    ///   the last walking cardinal.
    /// </param>
    public void SetTarget(Vector2 position,
                          AvatarAction action        = AvatarAction.Walk,
                          Vector2?     arrivalFacing = null)
    {
        _pendingAction = action;
        _targetPosition = position;
        _hasTarget      = true;

        if (arrivalFacing.HasValue)
        {
            _arrivalFacing    = arrivalFacing.Value;
            _hasArrivalFacing = true;
        }
        else
        {
            _hasArrivalFacing = false;
        }
    }

    /// <summary>Immediately teleport the avatar to a world position and halt all actions.</summary>
    public void TeleportTo(Vector2 position)
    {
        StopAll();
        _rb.position = position;
    }

    /// <summary>Cancel all movement, watering, and coroutines. Leaves the avatar idle.</summary>
    public void StopAll()
    {
        _hasTarget        = false;
        _hasArrivalFacing = false;

        if (_waterCoroutine != null)
        {
            StopCoroutine(_waterCoroutine);
            _waterCoroutine = null;
        }

        _isWatering              = false;
        _rb.linearVelocity       = Vector2.zero;
        _animator.SetBool(HashIsMoving,   false);
        _animator.SetBool(HashIsWatering, false);
    }

    /// <summary>
    /// Wire up the garden boundary collider at runtime (called by AvatarSpawner).
    /// </summary>
    public void SetGardenBounds(Collider2D bounds)
    {
        gardenBounds = bounds;
    }

    /// <summary>
    /// Returns a random point inside the garden bounds, optionally excluding a rectangular region.
    /// Makes up to 30 attempts before giving up on the exclusion.
    /// </summary>
    public Vector2 GetRandomPointInBounds(Bounds? exclude = null)
    {
        if (gardenBounds == null)
            return (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 2f;

        Bounds b   = gardenBounds.bounds;
        float  pad = 0.35f;
        Vector2 point;
        int attempts = 0;

        do
        {
            float x = UnityEngine.Random.Range(b.min.x + pad, b.max.x - pad);
            float y = UnityEngine.Random.Range(b.min.y + pad, b.max.y - pad);
            point   = new Vector2(x, y);
            attempts++;
        }
        while (exclude.HasValue
               && exclude.Value.Contains(new Vector3(point.x, point.y, 0f))
               && attempts < 30);

        return point;
    }

    /// <summary>Legacy compatibility: walk to near a flower position, then water.</summary>
    public void GoWaterFlower(Vector2 flowerPosition)
    {
        if (_isWatering) return;
        Vector2 offset = ((Vector2)transform.position - flowerPosition).normalized * 0.4f;
        SetTarget(flowerPosition + offset, AvatarAction.WalkAndWater);
    }

    #endregion

    // ─────────────────────────────────────────────────────────────────
    #region Private Helpers

    private IEnumerator WaterRoutine()
    {
        _isWatering = true;
        _rb.linearVelocity = Vector2.zero;

        _animator.SetFloat(HashWaterX,     _lastDirection.x);
        _animator.SetFloat(HashWaterY,     _lastDirection.y);
        _animator.SetBool(HashIsWatering,  true);
        _animator.SetBool(HashIsMoving,    false);

        yield return new WaitForSeconds(2f);

        _animator.SetBool(HashIsWatering, false);
        _isWatering     = false;
        _waterCoroutine = null;

        OnWateringComplete?.Invoke();
    }

    /// <summary>
    /// Pick the best single-axis cardinal direction to move toward diff,
    /// using momentum to avoid axis-switching jitter.
    /// </summary>
    private Vector2 ChooseCardinalDirection(Vector2 diff)
    {
        float absX = Mathf.Abs(diff.x);
        float absY = Mathf.Abs(diff.y);

        bool horizAligned = absX < 0.2f;
        bool vertAligned  = absY < 0.2f;

        if (horizAligned && !vertAligned)
            return new Vector2(0f, Mathf.Sign(diff.y));
        if (vertAligned && !horizAligned)
            return new Vector2(Mathf.Sign(diff.x), 0f);
        if (_currentCardinal.x != 0f && absX > 0.2f)
            return new Vector2(Mathf.Sign(diff.x), 0f);
        if (_currentCardinal.y != 0f && absY > 0.2f)
            return new Vector2(0f, Mathf.Sign(diff.y));

        return absX >= absY
            ? new Vector2(Mathf.Sign(diff.x), 0f)
            : new Vector2(0f, Mathf.Sign(diff.y));
    }

    private void ClampToBounds()
    {
        if (gardenBounds == null) return;
        Bounds  b   = gardenBounds.bounds;
        Vector2 pos = _rb.position;
        pos.x       = Mathf.Clamp(pos.x, b.min.x, b.max.x);
        pos.y       = Mathf.Clamp(pos.y, b.min.y, b.max.y);
        _rb.position = pos;
    }

    private void SetAnimatorMoving(Vector2 dir)
    {
        _animator.SetFloat(HashMoveX,     dir.x);
        _animator.SetFloat(HashMoveY,     dir.y);
        _animator.SetFloat(HashLastMoveX, dir.x);
        _animator.SetFloat(HashLastMoveY, dir.y);
        _animator.SetBool(HashIsMoving,   true);
        _animator.SetBool(HashIsWatering, false);
    }

    private void SetAnimatorIdle()
    {
        _animator.SetFloat(HashMoveX,     0f);
        _animator.SetFloat(HashMoveY,     0f);
        _animator.SetFloat(HashLastMoveX, _lastDirection.x);
        _animator.SetFloat(HashLastMoveY, _lastDirection.y);
        _animator.SetBool(HashIsMoving,   false);
    }

    #endregion
}
