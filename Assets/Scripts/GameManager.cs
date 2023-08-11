using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool gameOver;
    [SerializeField]
    private ArrayList turnOrder;
    private int comboCount = 0;
    private List<GameObject> enemies;
    private List<GameObject> heroes;

    public static GameManager instance;

    public GameObject activeChar;

    public int enemyTarget;

    public string menuSelection;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;

        heroes = GameObject.FindGameObjectsWithTag("Hero").OrderBy(p => p.name).ToArray().ToList();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").OrderBy(p => p.name).ToArray().ToList();
        GameObject[] targetButtons = gameObject.GetComponent<UIManager>().targetButtons;
        turnOrder = new ArrayList();

        //build turn order list
        foreach(GameObject hero in heroes)
        {
            turnOrder.Add(hero);
        }
        for (int i=0; i<enemies.Count; i++)
        {
            GameObject enemy = enemies[i];
            turnOrder.Add(enemy);
            //assign target button names
            GameObject targetButton = targetButtons[i];
            targetButton.GetComponentInChildren<TMP_Text>().text = enemy.GetComponent<CharController>().charName;
        }

        menuSelection = "";
    }

    public void PoseCharacter(string pose)
    {
        string currentPose = activeChar.GetComponent<CharController>().charPose;
        //cancelling out poses if same direction given
        if (currentPose == pose)
        {
            activeChar.GetComponent<CharController>().charPose = "neutral";
        }
        else if (pose == "light" || pose == "heavy")
        {
            string attackPose = currentPose.Split("_")[0] + "_" + pose;
            activeChar.GetComponent<CharController>().charPose = attackPose;
        }
        else if (pose != "wait")
        {
            activeChar.GetComponent<CharController>().charPose = pose;
        }
        activeChar.GetComponent<CharController>().UpdatePose();
    }

    public void ShowTargetSelection()
    {
        enemies[enemyTarget].GetComponent<CharController>().targetSelect.SetActive(true);
    }

    public void ChangeTargetSelection(string direction)
    {
        enemies[enemyTarget].GetComponent<CharController>().targetSelect.SetActive(false);

        enemyTarget = (direction == "left") ? enemyTarget - 1 : enemyTarget + 1;

        //wrap around target selection
        if (enemyTarget >= enemies.Count)
        {
            enemyTarget = 0;
        }
        else if (enemyTarget < 0)
        {
            enemyTarget = enemies.Count - 1;
        }

        enemies[enemyTarget].GetComponent<CharController>().targetSelect.SetActive(true);
    }

    public void HideAllTargetSelections()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<CharController>().targetSelect.SetActive(false);
        }
    }

    public void SelectTarget(Button button)
    {
        string actionString = gameObject.GetComponent<UIManager>().actionString;

        if (actionString.Contains("ATTACK"))
        {
            AttackTarget(button);
        }
        else if (actionString.Contains("SKILL"))
        {
            List<Button> buttons = new();

            //skill button is passed in as the multi target skill instead of a target button
            if (button.name.Contains("skill"))
            {
                //built the array of targets for multi target skill
                GameObject[] buttonObjects = gameObject.GetComponent<UIManager>().targetButtons;
                for (int i=0; i< buttonObjects.Length; i++)
                {
                    if(buttonObjects[i].activeSelf) //inactive targets throw a NullReferenceException
                    {
                        buttons.Add(buttonObjects[i].GetComponent<Button>());
                    }
                }
            }
            else
            {
                //single target skill is a single element array
                buttons.Add(button);
            }

            SkillTarget(buttons);
        }
    }

    public void AttackTarget(Button button)
    {
        string buttonName = button.name;
        string enemyObjectName = buttonName.Replace("target_", "ENEMY ");
        string enemyObjectIndex = buttonName.Replace("target_", "");

        //reset current defense to base defense before attacking
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense;

        //check attack strength and speed values
        int strength = activeChar.GetComponent<CharController>().charStrengthCurrent;
        int attackStrength = gameObject.GetComponent<Attack>().damageCalc(strength, menuSelection);
        int attackCost = gameObject.GetComponent<Attack>().selectedAttackCost;

        //reduce timer value
        activeChar.GetComponent<CharController>().charSpeedCurrent -= attackCost;
        int newCurrentSpeed = activeChar.GetComponent<CharController>().charSpeedCurrent;
        gameObject.GetComponent<UIManager>().timerText.text = newCurrentSpeed.ToString();

        //update combo text
        comboCount++;
        gameObject.GetComponent<UIManager>().heroComboText.text = comboCount + " HITS";
        if (comboCount > 1)
        {
            gameObject.GetComponent<UIManager>().heroComboText.GetComponent<Animation>().Stop();
            gameObject.GetComponent<UIManager>().heroComboText.GetComponent<Animation>().Play();
        }
        else
        {
            gameObject.GetComponent<UIManager>().heroComboText.text = "";
        }

        //reduce defense if more time was used than available
        if (newCurrentSpeed < 0)
        {
            activeChar.GetComponent<CharController>().charDefenseCurrent = activeChar.GetComponent<CharController>().charDefenseCurrent  + newCurrentSpeed;

            if (activeChar.GetComponent<CharController>().charDefenseCurrent < 0)
            {
                activeChar.GetComponent<CharController>().charDefenseCurrent = 0;
            }
        }

        //deal damage
        GameObject targetObject = GameObject.Find(enemyObjectName);
        CharController target = targetObject.GetComponent<CharController>();
        int damageToTake = attackStrength - target.charDefenseCurrent;
        if (damageToTake < 1) { damageToTake = 1; }
        target.TakeDamage(damageToTake);

        //subract 1 from enemy index since the named index starts at 1
        int enemyIndex = int.Parse(enemyObjectIndex) - 1;
        GameObject damageText = gameObject.GetComponent<UIManager>().enemyDamageText[enemyIndex];
        damageText.GetComponent<TMP_Text>().text = damageToTake.ToString();
        damageText.GetComponent<Animation>().Play();

        PrepNextTurn();

        if (!target.charAlive)
        {
            target.charPose = "ko";
            target.UpdatePose();
            turnOrder.Remove(targetObject);
            enemies.Remove(targetObject);

            //deactivate enemy target button if KO'd
            button.gameObject.SetActive(false);

            //check remaining characters for win/lose state
            CheckGameOver();
        }

        CheckRemainingSpeed(newCurrentSpeed);
    }

    public void SkillTarget(List<Button> buttons)
    {
        //reset current defense to base defense before attacking
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense;

        //check attack strength and speed values
        int strength = activeChar.GetComponent<CharController>().charStrengthCurrent;
        int skillStrength = gameObject.GetComponent<Skill>().damageCalc(strength, menuSelection);
        int skillCost = gameObject.GetComponent<Skill>().selectedSkillCost;

        //reduce timer value
        activeChar.GetComponent<CharController>().charSpeedCurrent -= skillCost;
        int newCurrentSpeed = activeChar.GetComponent<CharController>().charSpeedCurrent;
        gameObject.GetComponent<UIManager>().timerText.text = newCurrentSpeed.ToString();

        //reduce defense if more time was used than available
        if (newCurrentSpeed < 0)
        {
            activeChar.GetComponent<CharController>().charDefenseCurrent = activeChar.GetComponent<CharController>().charDefenseCurrent + newCurrentSpeed;

            if (activeChar.GetComponent<CharController>().charDefenseCurrent < 0)
            {
                activeChar.GetComponent<CharController>().charDefenseCurrent = 0;
            }
        }

        //loop to deal with multi target skills if needed
        foreach (Button button in buttons)
        {
            string buttonName = button.name;
            string enemyObjectName = buttonName.Replace("target_", "ENEMY ");
            string enemyObjectIndex = buttonName.Replace("target_", "");

            //deal damage
            GameObject targetObject = GameObject.Find(enemyObjectName);
            CharController target = targetObject.GetComponent<CharController>();
            int damageToTake = skillStrength - target.charDefenseCurrent;
            if (damageToTake < 1) { damageToTake = 1; }
            target.TakeDamage(damageToTake);

            //subract 1 from enemy index since the named index starts at 1
            int enemyIndex = int.Parse(enemyObjectIndex) - 1;
            GameObject damageText = gameObject.GetComponent<UIManager>().enemyDamageText[enemyIndex];
            damageText.GetComponent<TMP_Text>().text = damageToTake.ToString();
            damageText.GetComponent<Animation>().Play();

            if (!target.charAlive)
            {
                target.charPose = "ko";
                target.UpdatePose();
                turnOrder.Remove(targetObject);
                enemies.Remove(targetObject);

                //deactivate enemy target button if KO'd
                button.gameObject.SetActive(false);

                //check remaining characters for win/lose state
                CheckGameOver();
            }
        }

        PrepNextTurn();

        CheckRemainingSpeed(newCurrentSpeed);
    }

    public void Defend()
    {
        //modify def stat
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense * 2;

        //reset speed stat
        activeChar.GetComponent<CharController>().charSpeedCurrent = activeChar.GetComponent<CharController>().charSpeedBase;

        EndTurn();
    }

    public void Evade()
    {
        //modify def stat
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense / 2;

        //reset speed stat
        activeChar.GetComponent<CharController>().charSpeedCurrent = activeChar.GetComponent<CharController>().charSpeedBase;

        //set evade flag
        activeChar.GetComponent<CharController>().isEvading = true;

        EndTurn();
    }

    private void PrepNextTurn()
    {
        //change sprite to idle pose
        activeChar.GetComponent<CharController>().charPose = "neutral";
        activeChar.GetComponent<CharController>().UpdatePose();

        //update char status display
        activeChar.GetComponent<CharController>().UpdateStatus();
    }

    private void CheckRemainingSpeed(int newCurrentSpeed)
    {
        if (newCurrentSpeed <= 0)
        {
            //reset speed stat
            activeChar.GetComponent<CharController>().charSpeedCurrent = activeChar.GetComponent<CharController>().charSpeedBase;
            EndTurn();
        }
        else
        {
            gameObject.GetComponent<UIManager>().ShowMenuMain();
        }
    }

    public void EnemyTurn()
    {
        gameObject.GetComponent<UIManager>().currentMenu.Clear();
        HideAllTargetSelections();
        StartCoroutine(EnemyActions());
    }

    private IEnumerator EnemyActions()
    {
        int randomNumber = Random.Range(1, 11);
        //even random number (1-10) for light attack, odd for heavy
        if (randomNumber % 2 == 0)
        {
            menuSelection = gameObject.GetComponent<UIManager>().menuText.text = gameObject.GetComponent<UIManager>().menuAttackItems[0];
             
        }
        else
        {
            menuSelection = gameObject.GetComponent<UIManager>().menuText.text = gameObject.GetComponent<UIManager>().menuAttackItems[1];
        }

        //delay so player can see what happened
        yield return new WaitForSeconds(2);

        //random player target
        int heroIndex = Random.Range(0, heroes.Count);
        GameObject targetObject = heroes[heroIndex];
        CharController target = targetObject.GetComponent<CharController>();

        int enemyStrength = ((GameObject)turnOrder[0]).GetComponent<CharController>().charStrengthCurrent;
        int attackStrength = gameObject.GetComponent<Attack>().damageCalc(enemyStrength, menuSelection);
        int damageToTake = attackStrength - target.charDefenseCurrent;
        if (damageToTake < 1) { damageToTake = 1; }

        //check if evading target can negate attack with a 50% chance of the random number (1-10) being 5 or less
        if (target.isEvading && randomNumber < 6)
        {
            damageToTake = 0;
        }

        target.TakeDamage(damageToTake);

        GameObject damageText = gameObject.GetComponent<UIManager>().heroDamageText[heroIndex];
        damageText.GetComponent<TMP_Text>().text = damageToTake.ToString();
        damageText.GetComponent<Animation>().Play();

        //change sprite to idle pose
        activeChar.GetComponent<CharController>().charPose = "neutral";
        activeChar.GetComponent<CharController>().UpdatePose();

        if (!target.charAlive)
        {
            target.charPose = "ko";
            target.UpdatePose();
            turnOrder.Remove(targetObject);
            heroes.Remove(targetObject);

            //check remaining characters for win/lose state
            CheckGameOver();
        }

        EndTurn();
    }

    public void EndTurn()
    {
        menuSelection = "";
        comboCount = 0;

        //update char status display
        activeChar.GetComponent<CharController>().UpdateStatus();

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

            if (turnOrder.Count > 1)
            {
                GameObject nextCharObject = (GameObject)turnOrder[1];

                string nextChar = nextCharObject.GetComponent<CharController>().charName;
                gameObject.GetComponent<UIManager>().nextTurnText.text = nextChar;

                //turn next char text red for enemies as an extra visual indicator
                if (nextCharObject.tag == "Enemy")
                {
                    gameObject.GetComponent<UIManager>().nextTurnText.color = Color.red;
                }
                else
                {
                    gameObject.GetComponent<UIManager>().nextTurnText.color = Color.black;
                }
            }
            else
            {
                gameObject.GetComponent<UIManager>().nextTurnText.text = "";
            }
        }
        else
        {
            turnOrder.RemoveAt(0);
            EndTurn();
        }
    }

    private void CheckGameOver()
    {
        if (heroes.Count == 0)
        {
            gameOver = true;
            gameObject.GetComponent<UIManager>().gameOverText.text = "YOU LOSE";
        }
        else if (enemies.Count == 0)
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
