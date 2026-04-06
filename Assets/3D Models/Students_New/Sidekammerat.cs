using System.Collections;
using UnityEngine;

public class Sidekammerat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Idle Settings")]
    public int minIdleIndex = 1;
    public int maxIdleIndex = 5;
    public bool randomizeSwitch = true;

    [Header("Talking Settings")]
    public bool canTalk = false;              // Enable if this student should talk
    public bool isSpecialStudent = false;     // Special mouth animation
    public float talkTime = 60f;              // Time in seconds to start talking
    public TalkDirection talkDirection = TalkDirection.None;

    private bool hasTriggeredTalking = false; // Ensures talking triggers only once
    private float timer = 0f;
    private float currentLoopDuration;

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

    // Start timer on awake of the scene
    void Awake()
    {
        timer = 0f;
    }

    void Update()
    {
        // Auto-trigger talking by time
        if (canTalk && !hasTriggeredTalking && Time.time >= talkTime)
        {
            hasTriggeredTalking = true;

            // Default direction to Right if None
            if (talkDirection == TalkDirection.None)
                talkDirection = TalkDirection.Right;

            TriggerTalking();

            // Stop Update from switching idle after talking
            return;
        }

        // Handle idle switching only if talking hasn't started
        if (hasTriggeredTalking || !randomizeSwitch) return;

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

    // === TALKING METHOD ===
    private void TriggerTalking()
    {
        // Set the correct Animator bools
        if (isSpecialStudent)
            animator.SetBool("isSpecialTalking", true);
        else
            animator.SetBool("isTalking", true);

        // Set direction
        animator.SetInteger("TalkDirection", (int)talkDirection);

    }
}
