/* 1. description: UI logic for selecting level
 * 2. @author: Linden/Matthew
 * 3. @date: 20-12-24
 * 4. @version: 1.0
 */
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelect : MonoBehaviour
{
    // desc: run when button pressed in UI, loads main level
    // pre: none
    // post: none
    public void LoadLevel(string levelName){
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(levelName);
    }
}
