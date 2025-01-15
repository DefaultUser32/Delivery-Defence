/* 1. description: UI logic for exit button
 * 2. @author: Linden/Matthew
 * 3. @date: 20-12-24
 * 4. @version: 1.0
 */
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // desc: run when pressed in UI, closes app
    // pre: none
    // post: none
    public void OnButtonClick(){
        Application.Quit();
    }
}
