using UnityEngine;

public class RandomStartTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        // Try to find Animator automatically if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Start the coroutine that triggers the animation
        StartCoroutine(TriggerWithDelay());
    }

    private System.Collections.IEnumerator TriggerWithDelay()
    {
        // Wait a random delay between 0 and 3 seconds
        float delay = Random.Range(0f, 3f);
        yield return new WaitForSeconds(delay);

        // Trigger the "Start" trigger on the animator
        if (animator != null)
            animator.SetTrigger("Start");
    }
}
