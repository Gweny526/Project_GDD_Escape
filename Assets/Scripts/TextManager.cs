using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    // Références aux composants TextMeshPro
    public TMP_Text firstText;
    public TMP_Text secondText;
    public TMP_Text thirdText;

    public TMP_Text fourthText;

    // Fonction pour activer/désactiver les textes
    public void ShowText(bool firstOne, bool secondOne, bool thirdOne, bool fourthOne)
    {
        if (firstText != null) firstText.enabled = firstOne;
        if (secondText != null) secondText.enabled = secondOne;
        if (thirdText != null) thirdText.enabled = thirdOne;
        if (fourthText != null) fourthText.enabled = fourthOne;
    }
}
