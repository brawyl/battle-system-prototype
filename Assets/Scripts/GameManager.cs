using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private List<string> prevCommands;

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
        turnOrder = new ArrayList();
        prevCommands = new List<string>();
        enemyTarget = 0;

        //build turn order list
        foreach (GameObject hero in heroes)
        {
            turnOrder.Add(hero);
        }
        foreach (GameObject enemy in enemies)
        {
            turnOrder.Add(enemy);
        }
    }

    public void PoseCharacter(string pose)
    {
        prevCommands.Add(pose);
        if (prevCommands.Count == 3)
        {
            string specialCheck = "";
            foreach(string command in prevCommands)
            {
                specialCheck += command;
            }
            prevCommands.Clear();

            if (specialCheck == "crouchdashlight" || specialCheck == "crouchdashheavy") //fireball command
            {
                pose = "special" + specialCheck.Replace("crouchdash", "_");
            }
        }

        string currentPose = activeChar.GetComponent<CharController>().charPose;
        //reset pose to neutral when same direction is pressed
        if (currentPose == pose)
        {
            activeChar.GetComponent<CharController>().charPose = "neutral";
            prevCommands.Clear();
        }
        else if (pose == "light" || pose == "heavy")
        {
            string[] poseParts = currentPose.Split("_");
            string attackPose = poseParts[0] + "_" + pose;
            //make it so spamming attacks actually resets to the pose before attacking again
            if (currentPose == attackPose)
            {
                activeChar.GetComponent<CharController>().charPose = poseParts[0];
            }
            else
            {
                activeChar.GetComponent<CharController>().charPose = attackPose;
                if (gameObject.GetComponent<UIManager>().playerTurn)
                {
                    AttackTarget(enemies[enemyTarget]);
                }
            }
        }
        else if (pose.Contains("special"))
        {
            activeChar.GetComponent<CharController>().charPose = pose;
            if (gameObject.GetComponent<UIManager>().playerTurn)
            {
                SkillTarget(enemies[enemyTarget]);
            }
        }
        else if (pose != "wait")
        {
            activeChar.GetComponent<CharController>().charPose = pose;
        }

        activeChar.GetComponent<CharController>().UpdatePose();

        if (pose == "wait")
        {
            //ensure waiting on an attack pose resets to the movement pose instead
            if (currentPose.Contains("_"))
            {
                string[] poseParts = currentPose.Split("_");
                string movementPose = poseParts[0];
                activeChar.GetComponent<CharController>().charPose = movementPose;
                activeChar.GetComponent<CharController>().UpdatePose();
            }

            EndTurn();
        }
    }

    public void StartPlayerTurn()
    {
        HideAllTargetSelections();
        if (enemyTarget >= enemies.Count) enemyTarget = 0;
        gameObject.GetComponent<UIManager>().playerTurn = true;
        gameObject.GetComponent<UIManager>().ToggleContolButtonDisplay();
        enemies[enemyTarget].GetComponent<CharController>().targetSelect.SetActive(true);

        CheckStatBonus();
        activeChar.GetComponent<CharController>().charPose = "neutral";
        activeChar.GetComponent<CharController>().UpdatePose();
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

    public void AttackTarget(GameObject targetObject)
    {
        //reset current defense to base defense before attacking
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense;

        //check attack strength and speed values
        int strength = activeChar.GetComponent<CharController>().charStrengthCurrent;
        string pose = activeChar.GetComponent<CharController>().charPose;
        int attackStrength = gameObject.GetComponent<Attack>().damageCalc(strength, pose);
        int attackCost = gameObject.GetComponent<Attack>().selectedAttackCost;

        //reduce timer value
        activeChar.GetComponent<CharController>().charSpeedCurrent -= attackCost;
        int newCurrentSpeed = activeChar.GetComponent<CharController>().charSpeedCurrent;
        gameObject.GetComponent<UIManager>().UpdateTimerText(newCurrentSpeed);

        //reduce defense if more time was used than available
        if (newCurrentSpeed < 0)
        {
            activeChar.GetComponent<CharController>().charDefenseCurrent /= 2;

            if (activeChar.GetComponent<CharController>().charDefenseCurrent < 0)
            {
                activeChar.GetComponent<CharController>().charDefenseCurrent = 0;
            }
        }

        DealDamageToEnemy(targetObject, attackStrength);

        PrepNextTurn();

        CheckRemainingSpeed(newCurrentSpeed);
    }

    public void SkillTarget(GameObject targetObject)
    {
        //reset current defense to base defense before attacking
        int baseDefense = activeChar.GetComponent<CharController>().charDefenseBase;
        activeChar.GetComponent<CharController>().charDefenseCurrent = baseDefense;

        //check attack strength and speed values
        int strength = activeChar.GetComponent<CharController>().charStrengthCurrent;
        string pose = activeChar.GetComponent<CharController>().charPose;
        int skillStrength = gameObject.GetComponent<Skill>().damageCalc(strength, pose);
        int skillCost = gameObject.GetComponent<Skill>().selectedSkillCost;

        //reduce timer value
        activeChar.GetComponent<CharController>().charSpeedCurrent -= skillCost;
        int newCurrentSpeed = activeChar.GetComponent<CharController>().charSpeedCurrent;
        gameObject.GetComponent<UIManager>().UpdateTimerText(newCurrentSpeed);

        //reduce defense if more time was used than available
        if (newCurrentSpeed < 0)
        {
            activeChar.GetComponent<CharController>().charDefenseCurrent /= 2;

            if (activeChar.GetComponent<CharController>().charDefenseCurrent < 0)
            {
                activeChar.GetComponent<CharController>().charDefenseCurrent = 0;
            }
        }

        if (pose.Contains("heavy")) //heavy skill is multi target
        {
            //iterate thru a new find of the enemy game objects since removing enemies while iterating thru the enemy list causes issues
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (enemies.Contains(enemy))
                {
                    DealDamageToEnemy(enemy, skillStrength);
                }
            }
        }
        else
        {
            DealDamageToEnemy(targetObject, skillStrength);
        }

        PrepNextTurn();

        CheckRemainingSpeed(newCurrentSpeed);
    }

    private void DealDamageToEnemy(GameObject targetObject, int strength)
    {
        CharController attacker = activeChar.GetComponent<CharController>();
        CharController target = targetObject.GetComponent<CharController>();

        int damageToTake = CalculateDamageAndPose(target, attacker, strength);

        if (damageToTake > 0)
        {
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
        }

        //subract 1 from enemy index since the named index starts at 1
        string enemyObjectIndex = targetObject.name.Replace("ENEMY ", "");
        int enemyIndex = int.Parse(enemyObjectIndex) - 1;
        GameObject damageText = gameObject.GetComponent<UIManager>().enemyDamageText[enemyIndex];
        damageText.GetComponent<TMP_Text>().text = damageToTake > 0 ? damageToTake.ToString() : "MISS";
        damageText.GetComponent<Animation>().Play();

        if (!target.charAlive)
        {
            target.charPose = "ko";
            target.UpdatePose();
            target.targetSelect.SetActive(false);
            turnOrder.Remove(targetObject);
            enemies.Remove(targetObject);

            //check remaining characters for win/lose state
            CheckGameOver();
        }
    }

    private int CalculateDamageAndPose(CharController target, CharController attacker, int strength)
    {
        int damageToTake;

        //check if attack will miss the target that's jumping or crouching
        if ((target.charPose.Contains("jump") && !attacker.charPose.Contains("jump")) ||
            (target.charPose.Contains("crouch") && !attacker.charPose.Contains("crouch")))
        {
            damageToTake = 0;
        }
        else
        {
            damageToTake = strength - target.charDefenseCurrent;

            //take double damage if hit mid-jump or mid-crouch
            if ((target.charPose.Contains("jump") && attacker.charPose.Contains("jump")) ||
                    (target.charPose.Contains("crouch") && attacker.charPose.Contains("crouch")))
            {
                damageToTake *= 2;
            }
            //take half damage when blocking
            else if (target.charPose.Contains("block"))
            {
                damageToTake /= 2;
            }

            //do not allow zero damage attacks
            if (damageToTake < 1) { damageToTake = 1; }
        }

        target.TakeDamage(damageToTake);

        //reset a blocking pose to neutral when hit with a dash attack
        if (target.charPose.Contains("block") && attacker.charPose.Contains("dash"))
        {
            target.charPose = "neutral";
            target.UpdatePose();
        }

        return damageToTake;
    }

    private IEnumerator PlayerLastAttack()
    {
        gameObject.GetComponent<UIManager>().playerTurn = false;
        gameObject.GetComponent<UIManager>().ToggleContolButtonDisplay();
        if (activeChar.GetComponent<CharController>().charSpeedCurrent < 0)
        {
            gameObject.GetComponent<UIManager>().menuText.text = "VULNERABLE!";
        }

        //delay so player can see their last attack
        yield return new WaitForSeconds(1);

        activeChar.GetComponent<CharController>().charPose = "neutral";
        activeChar.GetComponent<CharController>().UpdatePose();

        //reset speed stat
        activeChar.GetComponent<CharController>().charSpeedCurrent = activeChar.GetComponent<CharController>().charSpeedBase;
        EndTurn();
    }

    public void StartEnemyTurn()
    {
        gameObject.GetComponent<UIManager>().playerTurn = false;
        gameObject.GetComponent<UIManager>().ToggleContolButtonDisplay();
        HideAllTargetSelections();

        CheckStatBonus();
        activeChar.GetComponent<CharController>().charPose = "neutral";
        activeChar.GetComponent<CharController>().UpdatePose();
        StartCoroutine(EnemyActions());
    }

    private IEnumerator EnemyActions()
    {
        //enemies do one movement and one action
        int randomNumberMovement = Random.Range(1, 9); //50% chance neutral, 50% chance other pose
        int randomNumberAction = Random.Range(1, 4); //33% chance for light, heavy or wait

        string movementString;
        switch (randomNumberMovement)
        {
            case 1:
                movementString = "block";
                break;
            case 2:
                movementString = "jump";
                break;
            case 3:
                movementString = "dash";
                break;
            case 4:
                movementString = "crouch";
                break;
            default:
                movementString = "neutral";
                break;
        }
        PoseCharacter(movementString);
        //delay so player can see what happened
        yield return new WaitForSeconds(1);

        string actionString;
        switch (randomNumberAction)
        {
            case 1:
                actionString = "wait";
                break;
            case 2:
                actionString = "heavy";
                break;
            default:
                actionString = "light";
                break;
        }
        if (movementString == "block") actionString = "wait"; //make blocking dependent on movement chance only
        else if (movementString == "neutral" && actionString == "wait") actionString = "light"; //prevent enemy wasting turn
        PoseCharacter(actionString);
        //delay so player can see what happened
        yield return new WaitForSeconds(1);

        if (actionString != "wait")
        {
            //random player target
            int heroIndex = Random.Range(0, heroes.Count);
            GameObject targetObject = heroes[heroIndex];
            CharController target = targetObject.GetComponent<CharController>();
            CharController attacker = activeChar.GetComponent<CharController>();

            int strength = attacker.charStrengthCurrent;
            string pose = attacker.charPose;

            int attackStrength = gameObject.GetComponent<Attack>().damageCalc(strength, pose);

            int damageToTake = CalculateDamageAndPose(target, attacker, attackStrength);

            //subract 1 from enemy index since the named index starts at 1
            string heroObjectNameIndex = targetObject.name.Replace("HERO ", "");
            int heroDamageTextIndex = int.Parse(heroObjectNameIndex) - 1;

            GameObject damageText = gameObject.GetComponent<UIManager>().heroDamageText[heroDamageTextIndex];
            damageText.GetComponent<TMP_Text>().text = damageToTake > 0 ? damageToTake.ToString() : "MISS";
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

            PrepNextTurn();
            EndTurn();
        }
    }

    private void CheckStatBonus()
    {
        if (activeChar.GetComponent<CharController>().charPose == "dash")
        {
            gameObject.GetComponent<UIManager>().menuText.text = "ATTACK BONUS!";
            activeChar.GetComponent<CharController>().charStrengthCurrent *= 2;
        }

        if (activeChar.GetComponent<CharController>().charDefenseCurrent != activeChar.GetComponent<CharController>().charDefenseBase)
        {
            activeChar.GetComponent<CharController>().charDefenseCurrent = activeChar.GetComponent<CharController>().charDefenseBase;
        }

        activeChar.GetComponent<CharController>().UpdateStatus();
    }

    private void CheckRemainingSpeed(int newCurrentSpeed)
    {
        if (newCurrentSpeed <= 0)
        {
            //change sprite to idle pose if they were attacking, denoted by an underscore in the pose name
            if (activeChar.GetComponent<CharController>().charPose.Contains("_"))
            {
                StartCoroutine(PlayerLastAttack());
            }
            else
            {
                //reset speed stat
                activeChar.GetComponent<CharController>().charSpeedCurrent = activeChar.GetComponent<CharController>().charSpeedBase;
                EndTurn();
            }
        }
    }

    private void PrepNextTurn()
    {
        //reset current strength to base after attacking
        int baseStrength = activeChar.GetComponent<CharController>().charStrengthBase;
        activeChar.GetComponent<CharController>().charStrengthCurrent = baseStrength;

        //update char status display
        activeChar.GetComponent<CharController>().UpdateStatus();

        gameObject.GetComponent<UIManager>().ToggleContolButtonDisplay();
    }

    public void EndTurn()
    {
        comboCount = 0;

        if (gameOver) { return; }

        gameObject.GetComponent<UIManager>().menuText.text = "";
        gameObject.GetComponent<UIManager>().timerText.text = "";

        //end active char turn
        if (activeChar != null && activeChar.GetComponent<CharController>().charAlive)
        {
            activeChar.GetComponent<CharController>().activeTurn = false;
            activeChar.GetComponent<CharController>().CheckTurn();
        }

        //move active char to back of turn order
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
        if (enemies.Count > 0)
        {
            enemyTarget = 0;
            enemies[enemyTarget].GetComponent<CharController>().targetSelect.SetActive(true);
        }
        gameObject.GetComponent<UIManager>().ShowGameOverScreen(gameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
