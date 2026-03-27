using UnityEngine;
using System.Collections;

/// <summary>
/// Drives the avatar's autonomous wandering behaviour.
/// idle 2–4s → pick random point → walk → idle → repeat.
/// Paused when the player taps; auto-resumes after watering completes.
/// </summary>
public class AvatarRandomWander : MonoBehaviour
{
    [Header("Wander Settings")]
    [SerializeField] private float idleTimeMin = 2f;
    [SerializeField] private float idleTimeMax = 4f;

    private AvatarController _avatar;
    private Coroutine        _wanderCoroutine;
    private bool             _paused;

    private void OnDestroy()
    {
        StopWander();
        UnsubscribeFromAvatar();
    }

    public void SetAvatar(AvatarController avatar)
    {
        UnsubscribeFromAvatar();
        _avatar = avatar;

        if (_avatar != null)
        {
            _avatar.OnWateringComplete += OnWateringComplete;
            _paused = false;
            StartWander();
        }
    }

    public void Pause()
    {
        _paused = true;
        StopWander();
    }

    public void Resume()
    {
        if (_avatar == null) return;
        _paused = false;
        StartWander();
    }

    private void StartWander()
    {
        StopWander();
        _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    private void StopWander()
    {
        if (_wanderCoroutine != null)
        {
            StopCoroutine(_wanderCoroutine);
            _wanderCoroutine = null;
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (!_paused && _avatar != null)
        {
            float wait = Random.Range(idleTimeMin, idleTimeMax);
            float elapsed = 0f;
            while (elapsed < wait)
            {
                if (_paused) yield break;
                elapsed += Time.deltaTime;
                yield return null;
            }

            while (_avatar != null && _avatar.IsBusy)
            {
                if (_paused) yield break;
                yield return null;
            }

            if (_paused || _avatar == null) yield break;

            Vector2 target = _avatar.GetRandomPointInBounds();
            _avatar.SetTarget(target, AvatarAction.Walk);

            while (_avatar != null && _avatar.HasTarget)
            {
                if (_paused) yield break;
                yield return null;
            }
        }
        _wanderCoroutine = null;
    }

    private void OnWateringComplete()
    {
        if (!_paused) return;
        Resume();
    }

    private void UnsubscribeFromAvatar()
    {
        if (_avatar != null)
            _avatar.OnWateringComplete -= OnWateringComplete;
    }
}
