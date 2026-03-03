using UnityEngine;

public class Eraser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Checkbox box = other.GetComponent<Checkbox>();

        if (box != null)
        {
            box.Uncheck();
        }
    }
}
