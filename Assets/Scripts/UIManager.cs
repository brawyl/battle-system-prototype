using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject menuObject;
    public TMP_Text menuText;
    public TMP_Text heroComboText;
    public TMP_Text enemyComboText;

    public TMP_Text timerText;

    public GameObject gameOverObject;
    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    public GameObject movementButtons;
    public GameObject actionButtons;
    public GameObject targetButtons;

    public GameObject restartButton;

    public List<GameObject> heroDamageText, enemyDamageText;

    public bool playerTurn;

    public TMP_Text nextTurnText;

    public string actionString = "";

    public TMP_Text debugText;

    private void Start()
    {
        menuObject.SetActive(true);
        gameOverObject.SetActive(false);

        GameManager.instance.NextTurn();
    }

    private void Update()
    {
        if (playerTurn)
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
    }

    public void ToggleContolButtonDisplay()
    {
        movementButtons.SetActive(playerTurn);
        actionButtons.SetActive(playerTurn);
        targetButtons.SetActive(playerTurn);

        menuText.text = playerTurn ? "FIGHT" : "";
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
            playerTurn = false;

            nextTurnText.text = "";

            //clear selected object
            EventSystem.current.SetSelectedGameObject(null);

            //set a new selected object
            EventSystem.current.SetSelectedGameObject(restartButton);
        }
    }
}
