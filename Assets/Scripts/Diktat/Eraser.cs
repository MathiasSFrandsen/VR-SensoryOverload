using UnityEngine;

public class Eraser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Checkbox box = other.GetComponent<Checkbox>(); // Forsøg at hente Checkbox-scriptet fra objektet der blev ramt

        if (box != null) // Hvis objektet er en checkbox
        {
            box.Uncheck();  // kald unchyeck metode fra Checkbox scriptet
        }
    }
}
