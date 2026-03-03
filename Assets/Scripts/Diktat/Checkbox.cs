using UnityEngine;

public class Checkbox : MonoBehaviour
{
    public GameObject crossPrefab;

    private GameObject spawnedCross;
    private bool isChecked;
    public bool IsChecked => isChecked;

    public void Check()
    {
        if (isChecked) return;

        // Spawn som child
        spawnedCross = Instantiate(crossPrefab, transform);

        // Placer præcis i midten af checkboxen
        spawnedCross.transform.localPosition = Vector3.zero;

        // Arv rotation fra parent (meget vigtigt)
        //spawnedCross.transform.localRotation = Quaternion.identity;

        isChecked = true;
    }

    public void Uncheck()
    {
        if (!isChecked) return;

        Destroy(spawnedCross);
        isChecked = false;
    }
}
