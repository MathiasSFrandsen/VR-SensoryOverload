using UnityEngine;

public class QuestionRow : MonoBehaviour
{
    public string questionName;   // "Question 1", "Question 2".. etc.

    public Checkbox boxOne;
    public Checkbox boxTwo;

    public Checkbox correctBox;

    public bool IsCorrectAnswer()
    {
        // Kun korrekt svar hvis: den rigtige er checked & den anden ikke er checked

        if (correctBox == boxOne)
            return boxOne.IsChecked && !boxTwo.IsChecked;
        else
            return boxTwo.IsChecked && !boxOne.IsChecked;
    }
}
