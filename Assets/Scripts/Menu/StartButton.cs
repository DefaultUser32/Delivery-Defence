/* 1. description: UI logic for back button
 * 2. @author: Linden/Matthew
 * 3. @date: 20-12-24
 * 4. @version: 1.0
 */
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject levelPanel;

    // desc: called when button pressed, opens start menu
    // pre: none
    // post: none
    public void OnButtonClick(){
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);
    }
}
