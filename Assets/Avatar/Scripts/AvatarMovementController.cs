using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Reads pointer taps via the new Input System.
/// Tap inside the garden boundary → avatar walks there and waters.
/// Ignores taps on UI elements and settings overlay.
/// Pauses random wander on tap; wander resumes after watering completes.
/// Uses Pointer.current which handles both mouse and touch input.
/// </summary>
public class AvatarMovementController : MonoBehaviour
{
    private Collider2D         _boundaryCollider;
    private AvatarRandomWander _randomWander;
    private Camera             _mainCamera;
    private AvatarController   _avatar;

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_avatar == null || _avatar.IsWatering) return;

        // Don't process if settings is open
        if (SettingsUI.isOpen) return;

        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 screenPos = pointer.position.ReadValue();
        Vector3 screen3   = new Vector3(screenPos.x, screenPos.y,
                                        Mathf.Abs(_mainCamera.transform.position.z));
        Vector2 worldPos  = _mainCamera.ScreenToWorldPoint(screen3);

        // Use bounds.Contains instead of OverlapPoint — works regardless of trigger/physics settings
        if (_boundaryCollider != null)
        {
            Bounds b = _boundaryCollider.bounds;
            if (!b.Contains(new Vector3(worldPos.x, worldPos.y, b.center.z)))
                return;
        }

        _randomWander?.Pause();

        Vector2 startPos       = _avatar.transform.position;
        Vector2 cardinalFacing = DominantCardinal(worldPos - startPos);
        _avatar.SetTarget(worldPos, AvatarAction.WalkAndWater, cardinalFacing);
    }

    // ── Public setters (called by GardenManager) ──────────────────────
    public void SetAvatar(AvatarController avatar)   => _avatar = avatar;
    public void SetBoundary(Collider2D boundary)     => _boundaryCollider = boundary;
    public void SetWander(AvatarRandomWander wander) => _randomWander = wander;

    private static Vector2 DominantCardinal(Vector2 v)
    {
        if (Mathf.Abs(v.x) >= Mathf.Abs(v.y))
            return v.x >= 0f ? Vector2.right : Vector2.left;
        return v.y >= 0f ? Vector2.up : Vector2.down;
    }
}
