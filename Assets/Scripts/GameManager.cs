using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool gameOver;
    [SerializeField]
    private ArrayList turnOrder;

    public GameObject activeChar;

    public string menuSelection;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] targetButtons = GameObject.FindGameObjectsWithTag("Button_target");
        turnOrder = new ArrayList();

        //build turn order list
        foreach(GameObject hero in heroes)
        {
            turnOrder.Add(hero);
        }
        for (int i=0; i<enemies.Length; i++)
        {
            GameObject enemy = enemies[i];
            turnOrder.Add(enemy);
            //assign target button names
            GameObject targetButton = targetButtons[i];
            targetButton.GetComponentInChildren<TMP_Text>().text = enemy.GetComponent<CharController>().charName;
        }

        menuSelection = "";
    }

    public void AttackTarget(Button button)
    {
        string buttonName = button.name;
        string enemyObjectName = buttonName.Replace("target_", "ENEMY ");
        string enemyObjectIndex = buttonName.Replace("target_", "");
        //deal damage
        int strength = activeChar.GetComponent<CharController>().charStrengthCurrent;
        int attackStrength = gameObject.GetComponent<Attack>().damageCalc(strength, menuSelection);
        GameObject targetObject = GameObject.Find(enemyObjectName);
        CharController target = targetObject.GetComponent<CharController>();
        int damageToTake = attackStrength - target.charDefenseCurrent;
        target.TakeDamage(damageToTake);

        //subract 1 from enemy index since the named index starts at 1
        int enemyIndex = int.Parse(enemyObjectIndex) - 1;
        GameObject damageText = gameObject.GetComponent<UIManager>().enemyDamageText[enemyIndex];
        damageText.GetComponent<TMP_Text>().text = damageToTake.ToString();
        damageText.GetComponent<Animation>().Play();

        //reset current defense to base defense after attacking
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense;

        //change sprite to idle pose
        activeChar.GetComponentInChildren<SpriteRenderer>().sprite = activeChar.GetComponent<CharController>().idlePose;

        if (!target.charAlive)
        {
            target.gameObject.SetActive(false);
            turnOrder.Remove(targetObject);

            //deactivate enemy target button if KO'd
            button.gameObject.SetActive(false);

            //check remaining characters for win/lose state
            CheckGameOver();
        }

        EndTurn();
    }

    public void Defend()
    {
        //modify def stat
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense * 2;

        //change sprite to def pose
        activeChar.GetComponentInChildren<SpriteRenderer>().sprite = activeChar.GetComponent<CharController>().defendPose;

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
            menuSelection = gameObject.GetComponent<UIManager>().menuText.text = gameObject.GetComponent<UIManager>().menuAttackItems[0];
             
        }
        else
        {
            menuSelection = gameObject.GetComponent<UIManager>().menuText.text = gameObject.GetComponent<UIManager>().menuAttackItems[1];
        }

        //2 sec delay so player can see what happened
        yield return new WaitForSeconds(2);

        //random player target
        GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
        int heroIndex = Random.Range(0, heroes.Length);
        GameObject targetObject = heroes[heroIndex];
        CharController target = targetObject.GetComponent<CharController>();

        int enemyStrength = ((GameObject)turnOrder[0]).GetComponent<CharController>().charStrengthCurrent;
        int attackStrength = gameObject.GetComponent<Attack>().damageCalc(enemyStrength, menuSelection);
        int damageToTake = attackStrength - target.charDefenseCurrent;
        target.TakeDamage(damageToTake);

        GameObject damageText = gameObject.GetComponent<UIManager>().heroDamageText[heroIndex];
        damageText.GetComponent<TMP_Text>().text = damageToTake.ToString();
        damageText.GetComponent<Animation>().Play();

        //change sprite to idle pose
        activeChar.GetComponentInChildren<SpriteRenderer>().sprite = activeChar.GetComponent<CharController>().idlePose;

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
        menuSelection = "";

        if (gameOver) { return; }

        //clear menu description text
        gameObject.GetComponent<UIManager>().menuText.text = "";
        gameObject.GetComponent<UIManager>().timerText.text = "";

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
            gameObject.GetComponent<UIManager>().gameOverText.text = "YOU LOSE";
        }
        else if (enemies.Length == 0)
        {
            gameOver = true;
            gameObject.GetComponent<UIManager>().gameOverText.text = "YOU WIN";
        }
        gameObject.GetComponent<UIManager>().ShowGameOverScreen(gameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
