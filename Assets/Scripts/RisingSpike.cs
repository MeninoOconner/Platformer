using System.Collections;
using UnityEngine;

public class RisingSpike : MonoBehaviour
{
    public enum Mode
    {
        Periodic,
        Proximity
    }

    [Header("Modo")]
    public Mode mode = Mode.Periodic;

    [Header("Movimento")]
    public float riseHeight = 0.8f;
    public float riseSpeed = 8f;
    public float lowerSpeed = 3f;

    [Header("Aviso antes de subir")]
    public float warningTime = 0.5f;
    public float warningPeek = 0.2f;

    [Header("Ciclo (modo Periodic)")]
    public float startDelay = 0f;
    public float hiddenTime = 2f;
    public float raisedTime = 1f;

    [Header("Proximidade (modo Proximity)")]
    public float triggerDistance = 2.5f;
    public float proximityRaisedTime = 1.2f;
    public float cooldown = 1.5f;

    [Header("Perigo")]
    [Range(0f, 1f)]
    public float dangerThreshold = 0.5f;

    private Vector3 raisedPosition;
    private Vector3 hiddenPosition;
    private Collider2D hazardCollider;
    private Transform player;

    void Start()
    {
        raisedPosition = transform.position;
        hiddenPosition = raisedPosition + Vector3.down * riseHeight;
        transform.position = hiddenPosition;

        hazardCollider = GetComponent<Collider2D>();
        UpdateDanger();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (mode == Mode.Periodic)
        {
            StartCoroutine(PeriodicRoutine());
        }
        else
        {
            StartCoroutine(ProximityRoutine());
        }
    }

    IEnumerator PeriodicRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            yield return new WaitForSeconds(hiddenTime);
            yield return Warning();
            yield return MoveTo(raisedPosition, riseSpeed);
            yield return new WaitForSeconds(raisedTime);
            yield return MoveTo(hiddenPosition, lowerSpeed);
        }
    }

    IEnumerator ProximityRoutine()
    {
        while (true)
        {
            bool playerIsNear = player != null
                && Mathf.Abs(player.position.x - raisedPosition.x) <= triggerDistance
                && Mathf.Abs(player.position.y - raisedPosition.y) <= 3f;

            if (playerIsNear)
            {
                yield return Warning();
                yield return MoveTo(raisedPosition, riseSpeed);
                yield return new WaitForSeconds(proximityRaisedTime);
                yield return MoveTo(hiddenPosition, lowerSpeed);
                yield return new WaitForSeconds(cooldown);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator Warning()
    {
        float timer = 0f;
        Vector3 peekPosition = hiddenPosition + Vector3.up * warningPeek;

        while (timer < warningTime)
        {
            timer += Time.deltaTime;
            float shake = Mathf.Sin(timer * 60f) * 0.03f;
            transform.position = peekPosition + Vector3.right * shake;
            UpdateDanger();
            yield return null;
        }

        transform.position = hiddenPosition;
        UpdateDanger();
    }

    IEnumerator MoveTo(Vector3 target, float speed)
    {
        while ((transform.position - target).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            UpdateDanger();
            yield return null;
        }

        transform.position = target;
        UpdateDanger();
    }

    void UpdateDanger()
    {
        if (hazardCollider == null) return;

        float raised = Mathf.InverseLerp(hiddenPosition.y, raisedPosition.y, transform.position.y);
        hazardCollider.enabled = raised >= dangerThreshold;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 basePosition = Application.isPlaying ? raisedPosition : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            basePosition + Vector3.left * triggerDistance,
            basePosition + Vector3.right * triggerDistance
        );
    }
}