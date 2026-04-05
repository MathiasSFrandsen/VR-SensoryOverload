using UnityEngine;

public class QuestionRow : MonoBehaviour
{
    public string questionName;   // "Question 1", "Question 2".. etc.

    public Checkbox boxOne;
    public Checkbox boxTwo;
    public Checkbox boxThree;

    public Checkbox correctBox;

    public bool IsCorrectAnswer()
    {
       Checkbox selectedBox = null;

        // Find den valgte
        if (boxOne.IsChecked) selectedBox = boxOne;
        if (boxTwo.IsChecked)
        {
            if (selectedBox != null) return false; // flere valgt
            selectedBox = boxTwo;
        }
        if (boxThree.IsChecked)
        {
            if (selectedBox != null) return false; // flere valgt
            selectedBox = boxThree;
        }

        // Ingen valgt
        if (selectedBox == null) return false;

        // Tjek om det er den rigtige
        return selectedBox == correctBox;
    }
}
