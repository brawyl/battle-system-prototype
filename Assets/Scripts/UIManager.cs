using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject menuObject;
    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuTarget;
    public TMP_Text menuText;

    //menu description strings
    public string[] menuMainItems;
    public string[] menuAttackItems;
    public string[] menuTargetItems;

    public TMP_Text timerText;

    public GameObject gameOverObject;
    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        menuObject.SetActive(true);
        gameOverObject.SetActive(false);

        BuildMenuItems();

        gameManager.NextTurn();
    }

    public void HideAllMenus()
    {
        menuMain.gameObject.SetActive(false);
        menuAttack.gameObject.SetActive(false);
        menuTarget.gameObject.SetActive(false);
    }

    public void ShowMenuMain()
    {
        HideAllMenus();
        SetMenuDesc("ACTION");
        menuMain.gameObject.SetActive(true);
    }

    public void ShowMenuAttack()
    {
        HideAllMenus();
        SetMenuDesc("ATTACK");
        menuAttack.gameObject.SetActive(true);
    }

    public void ShowMenuTarget(Button button)
    {
        gameManager.menuSelection = button.GetComponentInChildren<TMP_Text>().text;
        HideAllMenus();
        SetMenuDesc("TARGET");
        menuTarget.gameObject.SetActive(true);
    }

    public void SetMenuDesc(string newDesc)
    {
        if (newDesc.Length > 0)
        {
            menuText.text = newDesc.ToUpper();
        }
        else
        {
            menuText.text = "ERROR";
        }
    }

    public void UpdateTimerText(string speed)
    {
        timerText.text = speed.ToString();
    }

    public void ShowGameOverScreen(bool gameOver)
    {
        gameOverObject.SetActive(gameOver);
    }

    private void BuildMenuItems()
    {
        menuMainItems = new string[] { "ATTACK", "DEFEND" };
        menuAttackItems = new string[] { "LIGHT", "HEAVY" };
        menuTargetItems = new string[] { "ENEMY 1", "ENEMY 2", "ENEMY 3" };

        //todo: add buttons to target menu


        //todo: add attack buttons based on each characters' attacks

    }
}
