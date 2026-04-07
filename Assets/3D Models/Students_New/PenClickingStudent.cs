using System.Collections;
using UnityEngine;

public class PenClickingStudent : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Idle Settings")]
    public int minIdleIndex = 1;
    public int maxIdleIndex = 5;
    public bool randomizeSwitch = true;

    [Header("Pen Clicking Settings")]
    public float penClickStartTime = 60f; // Time in seconds to start
    public int penClickIndex = 6;         // IdleIndex value for PenClick

    private float timer = 0f;
    private float currentLoopDuration;

    private bool hasStartedPenClicking = false;

    void Awake()
    {
        timer = 0f;
    }

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Start with a random idle
        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);
        animator.speed = Random.Range(0.9f, 1.1f);

        if (randomizeSwitch)
            currentLoopDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        else
            animator.Play($"Idle{startIdle}");
    }

    void Update()
    {
        // Trigger Pen Clicking (permanent switch)
        if (!hasStartedPenClicking && Time.time >= penClickStartTime)
        {
            hasStartedPenClicking = true;
            StartPenClicking();
            return;
        }

        // Stop idle system once pen clicking has started
        if (hasStartedPenClicking) return;

        // Normal idle cycling
        if (!randomizeSwitch) return;

        timer += Time.deltaTime;
        if (timer >= currentLoopDuration)
        {
            timer = 0f;
            StartCoroutine(SwitchIdle());
        }
    }

    private IEnumerator SwitchIdle()
    {
        int newIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", newIdle);
        yield return null;

        currentLoopDuration = animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private void StartPenClicking()
    {
        // Lock into PenClick idle in blend tree
        animator.SetInteger("IdleIndex", penClickIndex);
    }
}