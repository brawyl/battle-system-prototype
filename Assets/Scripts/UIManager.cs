using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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

    public GameObject mainFirstButton, attackFirstButton;
    public GameObject[] targetButtons;
    public GameObject restartButton;

    public List<GameObject> heroDamageText, enemyDamageText;

    public GameObject currentMenu;

    public TMP_Text nextTurnText;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        menuObject.SetActive(true);
        gameOverObject.SetActive(false);

        BuildMenuItems();

        gameManager.NextTurn();
    }

    private void Update()
    {
        //manu navigation with arrow keys
        if (currentMenu != null)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.Escape))
            {
                if (currentMenu == menuAttack)
                {
                    ShowMenuMain();
                }
                else if (currentMenu == menuTarget)
                {
                    ShowMenuAttack();
                }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                //click the selected button right right arrow to go to next menu
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    public void HideAllMenus()
    {
        menuMain.gameObject.SetActive(false);
        menuAttack.gameObject.SetActive(false);
        menuTarget.gameObject.SetActive(false);
        currentMenu = null;
    }

    public void ShowMenuMain()
    {
        HideAllMenus();
        SetMenuDesc("ACTION");
        menuMain.gameObject.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(mainFirstButton);

        //change to atk pose
        GameObject activeChar = gameManager.activeChar;
        activeChar.GetComponentInChildren<SpriteRenderer>().sprite = activeChar.GetComponent<CharController>().idlePose;

        currentMenu = menuMain;
    }

    public void ShowMenuAttack()
    {
        HideAllMenus();
        SetMenuDesc("ATTACK");
        menuAttack.gameObject.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(attackFirstButton);

        //change to atk pose
        GameObject activeChar = gameManager.activeChar;
        activeChar.GetComponentInChildren<SpriteRenderer>().sprite = activeChar.GetComponent<CharController>().attackPose;

        currentMenu = menuAttack;
    }

    public void ShowMenuTarget(Button button)
    {
        gameManager.menuSelection = button.GetComponentInChildren<TMP_Text>().text;
        HideAllMenus();
        SetMenuDesc("TARGET");
        menuTarget.gameObject.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //loop thru targets and get first active button
        foreach(GameObject targetButton in targetButtons)
        {
            if (targetButton.activeSelf)
            {
                //set a new selected object
                EventSystem.current.SetSelectedGameObject(targetButton);
                break;
            }
        }

        currentMenu = menuTarget;
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

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(restartButton);

        currentMenu = null;
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
