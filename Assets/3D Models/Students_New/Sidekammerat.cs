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
    public bool canTalk = false;          // Enable talking
    public float talkTime = 60f;          // Time in seconds to start talking

    private bool hasTriggeredTalking = false;
    private float timer = 0f;
    private float currentLoopDuration;

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
        // Trigger talking at specific time (does NOT interrupt idle)
        if (canTalk && !hasTriggeredTalking && Time.time >= talkTime)
        {
            hasTriggeredTalking = true;
            TriggerTalking();
        }

        // Idle switching continues as normal
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

    // Talking method (mouth layer) ===
    private void TriggerTalking()
    {
        animator.SetBool("isSpecialTalking", true);

        // Ensure mouth layer is active
        animator.SetLayerWeight(1, 1f);
    }
}