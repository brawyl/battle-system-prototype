using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuTarget;

    public TMP_Text timerText;
    public TMP_Text menuText;

    [SerializeField]
    private bool gameOver;

    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    private string menuCode;

    private ArrayList turnOrder;
    private GameObject activeChar;

    //menu description strings
    [SerializeField]
    private string menuAttackLight = "ATTACK LIGHT";
    [SerializeField]
    private string menuAttackHeavy = "ATTACK HEAVY";

    //attack ranges
    [SerializeField]
    private float attackLightMin = 0.5f;
    [SerializeField]
    private float attackLightMax = 0.6f;
    [SerializeField]
    private float attackHeavyMin = 0.9f;
    [SerializeField]
    private float attackHeavyMax = 1.0f;

    void Awake()
    {
        //check if instance exists
        if (instance == null)
        {
            //set instance to this
            instance = this;
        }
        //exists but is another instance
        else if (instance != this)
        {
            //destroy it
            Destroy(gameObject);
        }
        //set this to not be destroyable
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        gameOverScreen.SetActive(false);

        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        turnOrder = new ArrayList();

        //build turn order list
        foreach(GameObject hero in heroes)
        {
            turnOrder.Add(hero);
        }
        foreach (GameObject enemy in enemies)
        {
            turnOrder.Add(enemy);
        }

        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        UpdateTimer();
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

    public void AttackTarget(Button button)
    {
        //deal damage
        int damageToTake = damageCalc();
        GameObject targetObject = GameObject.Find(button.name);
        CharController target = targetObject.GetComponent<CharController>();
        target.TakeDamage(damageToTake);

        if (!target.charAlive)
        {
            target.gameObject.SetActive(false);
            turnOrder.Remove(targetObject);

            //check remaining characters for win/lose state
            CheckGameOver();
        }

        EndTurn();
    }

    private void UpdateTimer()
    {
        //show char speed on timer text
        activeChar = (GameObject)turnOrder[0];
        int speed = activeChar.GetComponent<CharController>().charSpeed;

        timerText.text = speed.ToString();
    }

    public void EnemyTurn()
    {
        UpdateTimer();
        StartCoroutine(EnemyActions());
    }

    private IEnumerator EnemyActions()
    {
        //even random number (1-10) for light attack, odd for heavy
        if (Random.Range(1, 11) % 2 == 0)
        {
            menuText.text = menuAttackLight;
        }
        else
        {
            menuText.text = menuAttackHeavy;
        }

        //2 sec delay so player can see what happened
        yield return new WaitForSeconds(2);

        //random player target
        string targetName = "Hero " + Random.Range(1, 4);
        GameObject targetObject = GameObject.Find(targetName);
        CharController target = targetObject.GetComponent<CharController>();

        int damageToTake = damageCalc();
        target.TakeDamage(damageToTake);

        if (!target.charAlive)
        {
            target.gameObject.SetActive(false);
            turnOrder.Remove(targetObject);

            //check remaining characters for win/lose state
            CheckGameOver();
        }

        EndTurn();
    }

    public void EndTurn()
    {
        if (gameOver) { return; }

        //clear menu description text
        menuText.text = "";
        timerText.text = "";

        //end active char turn
        activeChar = (GameObject)turnOrder[0];
        if (activeChar != null && activeChar.GetComponent<CharController>().charAlive)
        {
            activeChar.GetComponent<CharController>().activeTurn = false;
            activeChar.GetComponent<CharController>().CheckTurn();
        }

        //move active char to back of turn order list
        turnOrder.RemoveAt(0);
        turnOrder.Add(activeChar);
        NextTurn();
    }

    public void NextTurn()
    {
        activeChar = (GameObject)turnOrder[0];
        if (activeChar != null && activeChar.GetComponent<CharController>().charAlive)
        {
            activeChar.GetComponent<CharController>().activeTurn = true;
            activeChar.GetComponent<CharController>().CheckTurn();
        }
        else
        {
            turnOrder.RemoveAt(0);
            EndTurn();
        }
    }

    private int damageCalc()
    {
        //get active char stats
        activeChar = (GameObject)turnOrder[0];
        CharController attacker = activeChar.GetComponent<CharController>();
        int attackStrength = attacker.charStrength;
        float damage = 0f;

        //check menu desc for damage calc
        if (menuText.text == menuAttackLight)
        {
            damage = attackStrength * Random.Range(attackLightMin, attackLightMax);
        }
        else if (menuText.text == menuAttackHeavy)
        {
            damage = attackStrength * Random.Range(attackHeavyMin, attackHeavyMax);
        }

        return (int)damage;
    }

    private void CheckGameOver()
    {
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (heroes.Length == 0)
        {
            gameOver = true;
            gameOverScreen.SetActive(true);
            gameOverText.text = "YOU LOSE";
        }
        else if (enemies.Length == 0)
        {
            gameOver = true;
            gameOverScreen.SetActive(true);
            gameOverText.text = "YOU WIN";
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
