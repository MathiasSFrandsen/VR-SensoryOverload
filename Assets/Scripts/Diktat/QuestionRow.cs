using UnityEngine;

public class QuestionRow : MonoBehaviour
{
    public string questionName; // Navnet på spørgsmålet

     // De tre svarmuligheder
    public Checkbox boxOne;
    public Checkbox boxTwo;
    public Checkbox boxThree;

    public Checkbox correctBox; // Reference til det korrekte svar
 
    public bool IsCorrectAnswer() // Returnerer true hvis brugeren har valgt det korrekte svar
    {
       Checkbox selectedBox = null; // Variabel til at gemme den valgte checkbox

        if (boxOne.IsChecked) selectedBox = boxOne; // Tjekker om første checkbox er markeret, hvis ja sæt den som selected bix
        if (boxTwo.IsChecked) // Tjekker om anden checkbox er markeret
        {
            if (selectedBox != null) return false;  // Hvis der allerede er fundet en markeret checkbox (dvs, man har markeret 2),returnerer false fordi flere svar er valgt
            selectedBox = boxTwo; // // Gemmer boxTwo som den valgte checkbox
        }
        if (boxThree.IsChecked)
        {
            if (selectedBox != null) return false; // flere valgt
            selectedBox = boxThree;
        }

        // Ingen valgt
        if (selectedBox == null) return false;

        // Tjek om det er den rigtige
        return selectedBox == correctBox; // Sammenligner den valgte checkbox med den korrekte checkbox. Returnerer true hvis de er ens, ellers false
    }
}
