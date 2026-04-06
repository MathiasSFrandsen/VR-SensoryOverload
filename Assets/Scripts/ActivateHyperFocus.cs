using UnityEngine;
using System.Collections;

public class ActivateHyperFocus : MonoBehaviour
{
    public float delay = 3f;     // Time before changing tag
    public string targetTag = "TargetObject";     // Tag to assign

    private void Start()
    {
        StartCoroutine(SetTagAfterDelay());
    }

    IEnumerator SetTagAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        gameObject.tag = targetTag;
    }
}
