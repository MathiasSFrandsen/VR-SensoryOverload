using UnityEngine;
using UnityEngine.InputSystem;

public class animationController : MonoBehaviour
{
    // Parameters
    public InputActionProperty grabAction;
    public InputActionProperty triggerAction;
    public Animator myAnimator;

    // Update is called once per frame
    void Update()
    {
        float grabValue = grabAction.action.ReadValue<float>();
        myAnimator.SetFloat("grab", grabValue);

        float triggerValue = triggerAction.action.ReadValue<float>();
        myAnimator.SetFloat("trigger", triggerValue);
    }

    public void ResetHand()
    {
        myAnimator.SetFloat("trigger", 0);
        myAnimator.SetFloat("grab", 0);
    }
}
