using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharController : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField]
    private TMP_Text charNameText;
    [SerializeField]
    private Slider hpSlider;

    private Vector3 startPosition;

    public string charName;
    public int charHPMax;
    public int charHPCurrent;
    public int charStrengthBase;
    public int charStrengthCurrent;
    public int charDefenseBase;
    public int charDefenseCurrent;
    public int charSpeedBase;
    public int charSpeedCurrent;
    public bool charAlive;
    public bool activeTurn;

    public Sprite idlePose, attackPose, defendPose;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        charAlive = true;
        charHPCurrent = charHPMax;
        hpSlider.value = 1.0f;
        charNameText.text = charName;

        //get starting position
        startPosition = gameObject.GetComponent<Transform>().position;
    }

    public void CheckTurn()
    {
        if (activeTurn)
        {
            gameObject.transform.position = Vector3.zero;
            if (gameObject.tag.Equals("Hero"))
            {
                gameManager.GetComponent<UIManager>().ShowMenuMain();

                //reset to idle pose on turn start
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = idlePose;
            }
            else
            {
                //set to atk pose on turn start for enemy
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = attackPose;

                //enemy targets random player with random action
                gameManager.EnemyTurn();
            }
            gameManager.GetComponent<UIManager>().UpdateTimerText(charSpeedCurrent.ToString());
        }
        else
        {
            gameObject.transform.position = startPosition;
            gameManager.GetComponent<UIManager>().HideAllMenus();
        }
    }

    public void TakeDamage(int damage)
    {
        charHPCurrent -= damage;
        charHPCurrent = charHPCurrent < 0 ? 0 : charHPCurrent;
        hpSlider.value = (float)charHPCurrent / (float)charHPMax;

        if (charHPCurrent <= 0)
        {
            charAlive = false;
        }
    }
}
