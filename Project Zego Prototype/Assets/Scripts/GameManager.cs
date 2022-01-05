using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuTarget;

    public TMP_Text timerText;
    public float timerValue;
    public float timerPreset;

    public TMP_Text menuText;
    private string menuCode;

    private ArrayList turnOrder;
    private GameObject activeObject;

    private float enemyTurnTime = 1f;
    private float inbetweenTurnTime = 0.5f;

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
        timerValue = timerPreset;
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        turnOrder = new ArrayList();

        //build turn order list
        foreach(GameObject hero in heroes)
        {
            string charSpeed = hero.GetComponent<CharController>().charSpeed.ToString();
            //prepend 0 in front of speed stat for sorting
            charSpeed = charSpeed.Length < 2 ? "0" + charSpeed : charSpeed;
            string charName = hero.name;
            turnOrder.Add(charSpeed + "_" + charName);
        }
        foreach (GameObject enemy in enemies)
        {
            string charSpeed = enemy.GetComponent<CharController>().charSpeed.ToString();
            //prepend 0 in front of speed stat for sorting
            charSpeed = charSpeed.Length < 2 ? "0" + charSpeed : charSpeed;
            string charName = enemy.name;
            turnOrder.Add(charSpeed + "_" + charName);
        }

        //sort turn order list according to speed stats
        turnOrder.Sort();
        turnOrder.Reverse();

        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerValue > 0)
        {
            timerValue -= Time.deltaTime;
            timerText.text = ((int)timerValue).ToString();

            //TODO: enemy turn actions
            //if (activeObject != null && activeObject.Tag.Equals("Enemy"))
            //{

            //}
        }
        else
        {
            timerValue = 0;
            timerText.text = ((int)timerValue).ToString();
        }
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
        CharController target = GameObject.Find(button.name).GetComponent<CharController>();
        target.TakeDamage(damageToTake);

        EndTurn();
    }

    public void EnemyTurn()
    {
        //even random number (1-10) for light attack, odd for heavy
        if (Random.Range(1,11) % 2 == 0)
        {
            menuText.text = menuAttackLight;
        }
        else
        {
            menuText.text = menuAttackHeavy;
        }

        //random player target
        string targetName = "Hero " + Random.Range(1,4);
        CharController target = GameObject.Find(targetName).GetComponent<CharController>();

        int damageToTake = damageCalc();
        target.TakeDamage(damageToTake);

        //TODO:add a 2 second delay

        EndTurn();
    }

    public void EndTurn()
    {
        //clear menu description text
        menuText.text = "";

        //end active char turn
        string activeChar = turnOrder[0].ToString();
        string charName = activeChar.Split('_')[1];
        activeObject = GameObject.Find(charName);
        activeObject.GetComponent<CharController>().activeTurn = false;
        activeObject.GetComponent<CharController>().CheckTurn();

        //move active char to back of turn order list
        turnOrder.RemoveAt(0);
        turnOrder.Add(activeChar);
        NextTurn();
    }

    public void NextTurn()
    {
        string charName = turnOrder[0].ToString().Split('_')[1];
        activeObject = GameObject.Find(charName);
        activeObject.GetComponent<CharController>().activeTurn = true;
        activeObject.GetComponent<CharController>().CheckTurn();
    }

    private int damageCalc()
    {
        //get active char stats
        string activeChar = turnOrder[0].ToString();
        string charName = activeChar.Split('_')[1];
        CharController attacker = GameObject.Find(charName).GetComponent<CharController>();
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
}
