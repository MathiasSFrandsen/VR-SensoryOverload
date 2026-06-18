using UnityEngine;

public class Pencil : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Checkbox box = other.GetComponent<Checkbox>(); // Forsøg at hente Checkbox-scriptet fra objektet der blev ramt

        if (box != null)  // Hvis objektet er en checkbox
        {
            box.Check(); // Marker checkboxen
        }
    }
}
