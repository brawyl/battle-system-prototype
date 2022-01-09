using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuTarget;

    public TMP_Text timerText;
    public TMP_Text menuText;

    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    private string menuCode;

    //menu description strings
    public string menuAttackLight = "ATTACK LIGHT";
    public string menuAttackHeavy = "ATTACK HEAVY";

    public void HideAllMenus()
    {
        menuMain.gameObject.SetActive(false);
        menuAttack.gameObject.SetActive(false);
        menuTarget.gameObject.SetActive(false);
    }

    public void ShowMenuMain()
    {
        HideAllMenus();
        SetMenuDesc("0");
        menuMain.gameObject.SetActive(true);
    }

    public void ShowMenuAttack()
    {
        HideAllMenus();
        SetMenuDesc("1");
        menuAttack.gameObject.SetActive(true);
    }

    public void ShowMenuTarget()
    {
        HideAllMenus();
        menuTarget.gameObject.SetActive(true);
    }

    public void SetMenuDesc(string newCode)
    {
        menuCode = newCode;

        if (menuCode[0] == '0')
        {
            menuText.text = "MAIN MENU";
        }
        else if (menuCode[0] == '1')
        {
            menuText.text = "ATTACK";
            if (menuCode.Length > 1 && menuCode[1] == '0')
            {
                menuText.text = menuAttackLight;
            }
            else if (menuCode.Length > 1 && menuCode[1] == '1')
            {
                menuText.text = menuAttackHeavy;
            }
        }
        else if (menuCode[0] == '2')
        {
            menuText.text = "SKILL";
        }
        else if (menuCode[0] == '3')
        {
            menuText.text = "ITEM";
        }
        else if (menuCode[0] == '4')
        {
            menuText.text = "DEFEND";
        }
        else if (menuCode[0] == '5')
        {
            menuText.text = "RUN";
        }
        else
        {
            Debug.Log("unrecognized menu code");
        }
    }

    public void UpdateTimerText(string speed)
    {
        timerText.text = speed.ToString();
    }

    public void ShowGameOverScreen(bool gameOver)
    {
        gameOverScreen.SetActive(gameOver);
    }
}
