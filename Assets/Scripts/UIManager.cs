using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject menuObject;
    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuSkill;
    public GameObject menuDefend;
    public GameObject menuTarget;
    public TMP_Text menuText;
    public TMP_Text heroComboText;
    public TMP_Text enemyComboText;

    //menu description strings
    public string[] menuMainItems;
    public string[] menuAttackItems;
    public string[] menuSkillItems;
    public string[] menuDefendItems;
    public string[] menuTargetItems;

    public TMP_Text timerText;

    public GameObject gameOverObject;
    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    public GameObject mainFirstButton, attackFirstButton, skillFirstButton, defendFirstButton;
    public GameObject[] targetButtons;
    public GameObject restartButton;

    public List<GameObject> heroDamageText, enemyDamageText;

    public List<string> currentMenu;

    public TMP_Text nextTurnText;

    public string actionString = "";

    public TMP_Text debugText;

    private void Start()
    {
        menuObject.SetActive(true);
        gameOverObject.SetActive(false);

        BuildMenuItems();

        GameManager.instance.NextTurn();
    }

    private void Update()
    {
        //MOVEMENT KEYS
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameManager.instance.PoseCharacter("jump");
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            GameManager.instance.PoseCharacter("block");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.instance.PoseCharacter("crouch");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.instance.PoseCharacter("dash");
        }

        //ACTION KEYS
        else if (Input.GetKeyDown(KeyCode.J))
        {
            GameManager.instance.PoseCharacter("wait");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            GameManager.instance.PoseCharacter("light");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.instance.PoseCharacter("heavy");
        }

        //TARGET SELECT KEYS
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.instance.ChangeTargetSelection("left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.instance.ChangeTargetSelection("right");
        }
    }

        public void HideAllMenus()
    {
        menuMain.gameObject.SetActive(false);
        menuAttack.gameObject.SetActive(false);
        menuSkill.gameObject.SetActive(false);
        menuDefend.gameObject.SetActive(false);
        menuTarget.gameObject.SetActive(false);
    }

    public void ShowMenuMain()
    {
        HideAllMenus();
        SetMenuDesc("ACTION");
        menuMain.gameObject.SetActive(true);

        //clear selected object
        //EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(mainFirstButton);

        //change to neutral pose
        GameManager.instance.PoseCharacter("neutral");


        if (currentMenu != null)
        {
            currentMenu.Clear();
            currentMenu.Insert(0, "main");
        }
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
        GameManager.instance.PoseCharacter("neutral_light");

        if (currentMenu.Count > 0 && currentMenu[0] != "attack")
        {
            currentMenu.Insert(0, "attack");
        }

        actionString = "ATTACK";
    }

    public void ShowMenuSkill()
    {
        HideAllMenus();
        SetMenuDesc("SKILL");
        menuSkill.gameObject.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(skillFirstButton);

        //change to skill pose
        GameManager.instance.PoseCharacter("special");

        if (currentMenu.Count > 0 && currentMenu[0] != "skill")
        {
            currentMenu.Insert(0, "skill");
        }

        actionString = "SKILL";
    }

    public void ShowMenuDefend()
    {
        HideAllMenus();
        SetMenuDesc("DEFEND");
        menuDefend.gameObject.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        //set a new selected object
        EventSystem.current.SetSelectedGameObject(defendFirstButton);

        //change to def pose
        GameManager.instance.PoseCharacter("block");

        if (currentMenu.Count > 0 && currentMenu[0] != "defend")
        {
            currentMenu.Insert(0, "defend");
        }

        actionString = "DEFEND";
    }

    public void ShowMenuTarget(Button button)
    {
        GameManager.instance.menuSelection = button.GetComponentInChildren<TMP_Text>().text;
        HideAllMenus();
        SetMenuDesc("TARGET");
        //add the action name from the button to the action string which is assumed to be at index 0
        actionString += " > " + button.GetComponentsInChildren<TMP_Text>()[0].text;
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

        if (currentMenu.Count > 0 && currentMenu[0] != "target")
        {
            currentMenu.Insert(0, "target");
        }
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

        if (gameOver)
        {
            currentMenu = null;

            nextTurnText.text = "";

            //clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            //set a new selected object
            EventSystem.current.SetSelectedGameObject(restartButton);
        }
    }

    private void BuildMenuItems()
    {
        menuMainItems = new string[] { "ATTACK", "DEFEND" };
        menuAttackItems = new string[] { "LIGHT", "HEAVY" };
        menuSkillItems = new string[] { "SINGLE", "MULTI" };
        menuDefendItems = new string[] { "BLOCK", "EVADE" };
        menuTargetItems = new string[] { "ENEMY 1", "ENEMY 2", "ENEMY 3" };

        //set attack costs
        TMP_Text[] attackItems = menuAttack.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text attackItemText in attackItems)
        {
            if (attackItemText.gameObject.name.Contains("cost"))
            {
                //assume gameObject name is formatted like atk_cost_1 so index 2 of a split string will return the atk number
                string attackNumber = attackItemText.gameObject.name.Split("_")[2];
                //subtract 1 from attack number since the object names are numbered starting at 1
                int attackIndex = int.Parse(attackNumber) - 1;
                string attackName = menuAttackItems[attackIndex];
                int attackCost = GameManager.instance.GetComponent<Attack>().getAttackCost(attackName);
                attackItemText.text = attackCost.ToString();
            }
        }

        //set skill costs
        TMP_Text[] skillItems = menuSkill.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text skillItemText in skillItems)
        {
            if (skillItemText.gameObject.name.Contains("cost"))
            {
                //assume gameObject name is formatted like skill_cost_1 so index 2 of a split string will return the atk number
                string skillNumber = skillItemText.gameObject.name.Split("_")[2];
                //subtract 1 from skill number since the object names are numbered starting at 1
                int skillIndex = int.Parse(skillNumber) - 1;
                string skillName = menuSkillItems[skillIndex];
                int skillCost = GameManager.instance.GetComponent<Skill>().getSkillCost(skillName);
                skillItemText.text = skillCost.ToString();
            }
        }
    }
}
