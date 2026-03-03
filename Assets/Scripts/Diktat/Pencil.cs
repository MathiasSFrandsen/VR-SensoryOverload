using UnityEngine;

public class Pencil : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Checkbox box = other.GetComponent<Checkbox>();

        if (box != null)
        {
            box.Check();
        }
    }
}
