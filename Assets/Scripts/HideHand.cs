using UnityEngine;

public class HideHand : MonoBehaviour
{
    [SerializeField] private GameObject hand;

    public void DisableHands()
    {
        hand.SetActive(false);
    }

    public void EnableHand()
    {
        hand.SetActive(false);
    }
}
