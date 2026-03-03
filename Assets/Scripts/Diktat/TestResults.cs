using UnityEngine;
using System.IO;
using System.Collections;

public class TestResults : MonoBehaviour
{
    public QuestionRow[] rows;

    private void Start()
    {
        // Start the 15-second countdown as soon as the scene starts
        StartCoroutine(CountdownAndSubmit(15));
    }

    // Coroutine for countdown
    private IEnumerator CountdownAndSubmit(int seconds)
    {
        int remaining = seconds;

        while (remaining > 0)
        {
            Debug.Log("Time remaining: " + remaining);
            yield return new WaitForSeconds(1f); // wait 1 second
            remaining--;
        }

        Debug.Log("Time's up! Submitting test...");
        SubmitTest();
    }

    public void SubmitTest()
    {
        string path = Application.dataPath + "/TestResult.txt";
        int totalQuestions = rows.Length;
        int correctAnswers = 0;

        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (QuestionRow row in rows)
            {
                if (row.IsCorrectAnswer())
                {
                    writer.WriteLine(row.questionName + ": Correct");
                    correctAnswers++;
                }
                else
                {
                    writer.WriteLine(row.questionName + ": Incorrect");
                }
            }

            // Add final score line
            writer.WriteLine("Score: " + correctAnswers + "/" + totalQuestions);
        }

        Debug.Log("Test result saved to: " + path + " | Score: " + correctAnswers + "/" + totalQuestions); 
    }
}
