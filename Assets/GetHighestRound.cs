/* 1. description: script for logic setting high score text on main menu
 * 2. @author: Linden/Matthew
 * 3. @date: 14-01-25
 * 4. @version: 1.0
 */
using System.IO;
using TMPro;
using UnityEngine;

public class GetHighestRound : MonoBehaviour
{
    [SerializeField] string filePathFromAssets;
    [SerializeField] TMP_Text textField;
    void Start()
    {
        if (!File.Exists(Application.dataPath + filePathFromAssets))
        {
            File.WriteAllText(Application.dataPath + filePathFromAssets, "-1");
        }
        string highScorePath = Application.dataPath + filePathFromAssets;
        string highScoreText = File.ReadAllText(highScorePath);
        textField.text = "";
        if (int.TryParse(highScoreText, out int result))
        {
            if (result <= 0) { return; }
            textField.text = "High Score: " + highScoreText + " Rounds";

        }
    }
}
