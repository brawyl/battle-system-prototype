using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private bool gameOver;

    private ArrayList turnOrder;
    private GameObject activeChar;

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
        UIManager.instance.ShowGameOverScreen(gameOver);

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

    public void AttackTarget(Button button)
    {
        //deal damage
        int enemyStrength = ((GameObject)turnOrder[0]).GetComponent<CharController>().charStrength;
        int damageToTake = Attack.instance.damageCalc(enemyStrength);
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

    public void EnemyTurn()
    {
        StartCoroutine(EnemyActions());
    }

    private IEnumerator EnemyActions()
    {
        //even random number (1-10) for light attack, odd for heavy
        if (Random.Range(1, 11) % 2 == 0)
        {
            UIManager.instance.menuText.text = UIManager.instance.menuAttackLight;
        }
        else
        {
            UIManager.instance.menuText.text = UIManager.instance.menuAttackHeavy;
        }

        //2 sec delay so player can see what happened
        yield return new WaitForSeconds(2);

        //random player target
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        GameObject targetObject = heroes[Random.Range(0, heroes.Length)];
        CharController target = targetObject.GetComponent<CharController>();

        int enemyStrength = ((GameObject)turnOrder[0]).GetComponent<CharController>().charStrength;
        int damageToTake = Attack.instance.damageCalc(enemyStrength);
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
        UIManager.instance.menuText.text = "";
        UIManager.instance.timerText.text = "";

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

    private void CheckGameOver()
    {
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (heroes.Length == 0)
        {
            gameOver = true;
            UIManager.instance.gameOverText.text = "YOU LOSE";
        }
        else if (enemies.Length == 0)
        {
            gameOver = true;
            UIManager.instance.gameOverText.text = "YOU WIN";
        }
        UIManager.instance.ShowGameOverScreen(gameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
