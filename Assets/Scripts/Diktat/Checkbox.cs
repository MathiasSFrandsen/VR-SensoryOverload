using UnityEngine;

public class Checkbox : MonoBehaviour
{
    public GameObject crossPrefab; // Prefab af krydset/fluebenet der skal vises når checkboxen markeres

    private GameObject spawnedCross; // Reference til det kryds der bliver oprettet
    private bool isChecked; // Holder styr på om checkboxen er markeret
    public bool IsChecked => isChecked; // Gør det muligt for andre scripts at læse isChecked

    public void Check()
    {
        if (isChecked) return; // Stop hvis checkboxen allerede er markeret

        spawnedCross = Instantiate(crossPrefab, transform); // Opret krydset som child af checkboxen

        // Placer præcis i midten af checkboxen
        spawnedCross.transform.localPosition = Vector3.zero; // Placér krydset i midten af checkboxen

        isChecked = true; // Marker bool som true
    }

    public void Uncheck()
    {
        if (!isChecked) return;  // Stop hvis checkboxen ikke er markeret

        Destroy(spawnedCross); // Slet det oprettede kryds
        isChecked = false;  // Marker checkboxen som ikke valgt
    }
}
