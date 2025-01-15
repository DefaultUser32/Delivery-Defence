/* 1. description: handles logic for game state (health, controlling waves, money, etc)
 * 2. @author: Linden/Matthew
 * 3. @date: 21-12-24
 * 4. @version: 1.0
 */

using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static float health;
    public static float money;

    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text highestRoundText;
    [SerializeField] string highestRoundFilePathFromAssets;

    private WaveManager waveManager; // reference to the component that spawns enemies

    // desc: start is run before the first frame when the object is created (UNITY FUNCTION)
    // pre: none
    // post: none
    void Start()
    {
        money = 500;
        health = 150;

        waveManager = GetComponent<WaveManager>();
        waveManager.StartNextWave(false);

        waveManager.SetRoundCounter();
        StartCoroutine(HandleWaves());
    }

    // desc: FixedUpdate is run a set number of times per second (UNITY FUNCTION)
    // pre: none
    // post: none
    void FixedUpdate(){
        moneyText.text = "$" + money.ToString();
        healthText.text = "Health: " + health.ToString();
    }

    // desc: unity function called once per frame
    // pre: none
    // post: none
    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }


        if (Input.GetKey(KeyCode.F11)) {
            Die();
        }


    }

    // desc: ends the game, and returns you to menu after setting high score
    // pre: none
    // post: none
    public void Die()
    {
        string highScorePath = Application.dataPath + highestRoundFilePathFromAssets;
        string highScoreText = File.ReadAllText(highScorePath);
        int highestScore = waveManager.GetRoundNumber();
        if (int.TryParse(highScoreText, out int result))
        {
            highestScore = Mathf.Max(highestScore, result);
        }
        File.WriteAllText(Application.dataPath + highestRoundFilePathFromAssets, "" + highestScore);

        Time.timeScale = 0.25f;
        highestRoundText.text = "You Survived " + waveManager.GetRoundNumber() + " Rounds";
        FindObjectsByType<UIStateManager>(FindObjectsSortMode.None)[0].gameOver = true;
    }
    // desc: Update is run each frame (UNITY FUNCTION)
    // pre: none
    // post: none
    private IEnumerator HandleWaves()
    {
        while (true) {
            yield return new WaitForSeconds(.5f);
            
            if (FindObjectsByType<PathFollow>(FindObjectsSortMode.None).Length > 0 || waveManager.isWaveActive)
            {
                continue;
            }
            money += 100;
            yield return new WaitForSeconds(1.5f);
            waveManager.StartNextWave(false);
        }
    }

}
