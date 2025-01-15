/* 1. description: UI logic for back button
 * 2. @author: Linden/Matthew
 * 3. @date: 20-12-24
 * 4. @version: 1.0
 */
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject levelPanel;

    // desc: run when button pressed in UI
    // pre: none
    // post: none
    public void OnButtonClick(){
        menuPanel.SetActive(true);
        levelPanel.SetActive(false);
    }
}
