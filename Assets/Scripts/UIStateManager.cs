/* 1. description: handles the logic of UI activity for the game menu
 * 2. @author: Linden/Matthew
 * 3. @date: 12-01-25
 * 4. @version: 1.0
 */
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIStateManager : MonoBehaviour
{
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject shopOpenButton;
    [SerializeField] GameObject upgradePanel;

    // GAME OVER STUFF
    [SerializeField] List<GameObject> miscUI;
    [SerializeField] GameObject gameOverObject;

    [SerializeField] Tower.Upgrade maxedUpgrade; // this is a fucked up way of doing this
    [SerializeField] Image towerIcon;
    [SerializeField] TMP_Text path1Name;
    [SerializeField] TMP_Text path1Description;
    [SerializeField] TMP_Text path2Name;
    [SerializeField] TMP_Text path2Description;

    Tower activeTower;
    public bool gameOver = false;

    // desc: basic unity function called on start
    // pre: none
    // post: none
    private void Start()
    {
        gameOverObject.SetActive(false);
        CloseAll();
    }


    // desc: unity function called once per frame
    // pre: none
    // post: none
    private void Update()
    {
        if (gameOver)
        {
            shopPanel.SetActive(false);
            shopOpenButton.SetActive(false);
            upgradePanel.SetActive(false);
            foreach (GameObject g in miscUI) { 
                g.SetActive(false);
            }
            gameOverObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
            if (shopPanel.activeSelf || upgradePanel.activeSelf)
                CloseAll();
        }
    }

    // desc: opens the shop panel
    // pre: none
    // post: none
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        shopOpenButton.SetActive(false);
        upgradePanel.SetActive(false);
    }

    // desc: opens/updates the upgrade panel
    // pre: valid tower
    // post: none
    public void OpenUpgrade(Tower t)
    {
        activeTower = t;
        UpdateUpgradePanel();

    }

    // desc: closes all open panels
    // pre: none
    // post: none
    public void CloseAll()
    {
        activeTower = null;
        shopPanel.SetActive(false);
        shopOpenButton.SetActive(true);
        upgradePanel.SetActive(false);
    }

    // desc: handles logic for attempting to buy upgrades
    // pre: none
    // post: none
    public void ApplyUpgrade(bool isFirst)
    {
        if (activeTower == null) return; 
        Tower.Upgrade u = activeTower.GetNextUpgrade(isFirst);
        u ??= maxedUpgrade;
        if (u.price > GameManager.money)
        {
            return;
        }

        GameManager.money -= u.price;

        activeTower.UpgradePath(isFirst);

        UpdateUpgradePanel();
    }

    // desc: updates visuals and text for the upgrade panel
    // pre: valid active tower set
    // post: none
    void UpdateUpgradePanel()
    {

        shopPanel.SetActive(false);
        shopOpenButton.SetActive(false);
        upgradePanel.SetActive(true);

        towerIcon.sprite = activeTower.sr.sprite;
        int path1Progress = activeTower.GetPathProgress(true);
        int path2Progress = activeTower.GetPathProgress(false);
        // GET UPGRADE REFS
        Tower.Upgrade path1Upgrade = activeTower.GetNextUpgrade(true);
        path1Upgrade ??= maxedUpgrade;

        Tower.Upgrade path2Upgrade = activeTower.GetNextUpgrade(false);
        path2Upgrade ??= maxedUpgrade;

        path1Name.text = string.Format("${0} : {1}", path1Upgrade.price, path1Upgrade.upgradeName);
        path1Description.text = path1Upgrade.upgradeDescription;

        path2Name.text = string.Format("${0} : {1}", path2Upgrade.price, path2Upgrade.upgradeName);
        path2Description.text = path2Upgrade.upgradeDescription;
    }
}
