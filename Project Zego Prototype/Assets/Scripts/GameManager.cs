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

    private ArrayList turnOrder;

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
        if(timerValue > 0)
        {
            timerValue -= Time.deltaTime;
            timerText.text = ((int)timerValue).ToString();
        }
        else
        {
            EndTurn();
            timerValue = timerPreset;
            NextTurn();
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
        menuMain.gameObject.SetActive(true);
    }

    public void ShowMenuAttack()
    {
        HideAllMenus();
        menuAttack.gameObject.SetActive(true);
    }

    public void ShowMenuTarget()
    {
        HideAllMenus();
        menuTarget.gameObject.SetActive(true);
    }

    public void AttackTarget(Button button)
    {
        CharController target = GameObject.Find(button.name).GetComponent<CharController>();
        //TODO: factor stats into damage calc
        target.TakeDamage(10);
        ShowMenuMain();
    }

    public void EndTurn()
    {
        string activeChar = turnOrder[0].ToString();

        //end active char turn
        string charName = activeChar.Split('_')[1];
        GameObject activeObject = GameObject.Find(charName);
        activeObject.GetComponent<CharController>().activeTurn = false;
        activeObject.GetComponent<CharController>().CheckTurn();

        //move active char to back of turn order list
        turnOrder.RemoveAt(0);
        turnOrder.Add(activeChar);
    }

    public void NextTurn()
    {
        string charName = turnOrder[0].ToString().Split('_')[1];
        GameObject activeObject = GameObject.Find(charName);
        activeObject.GetComponent<CharController>().activeTurn = true;
        activeObject.GetComponent<CharController>().CheckTurn();
    }
}
