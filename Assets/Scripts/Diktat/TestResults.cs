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
 
    public void SubmitTest() // Funktion der retter testen og gemmer resultatet
    {
        string path = Application.persistentDataPath + "/TestResult.txt";  // Opretter stien til tekstfilen
        int totalQuestions = rows.Length;    // Gemmer det samlede antal spørgsmål
        int correctAnswers = 0;  // Ind til at gamme antal korrekte 

        using (StreamWriter writer = new StreamWriter(path)) // Opretter en StreamWriter til at skrive til filen
        {
            foreach (QuestionRow row in rows) // Gennemløber alle spørgsmål
            {
                if (row.IsCorrectAnswer()) // Tjekker om spørgsmålet er besvaret korrekt
                {
                    writer.WriteLine(row.questionName + ": Correct"); // Skriver at spørgsmålet var korrekt
                    correctAnswers++; // Lægger 1 til antallet af korrekte svar
                }
                else
                {
                    writer.WriteLine(row.questionName + ": Incorrect"); // Skriver at spørgsmålet var forkert
                }
            }

            // Add final score line
            writer.WriteLine("Score: " + correctAnswers + "/" + totalQuestions);   // Skriver den endelige score nederst i filen
        }

        Debug.Log("Test result saved to: " + path + " | Score: " + correctAnswers + "/" + totalQuestions); // Viser filplacering og score i Unity Console
    }
}
