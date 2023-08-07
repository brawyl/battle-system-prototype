using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text charNameText;
    [SerializeField]
    private TMP_Text charStatusText;
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
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
    public bool isEvading;

    public string charPose;

    public Sprite poseNeutral, poseNeutralLight, poseNeutralHeavy;
    public Sprite poseJump, poseJumpLight, poseJumpHeavy;
    public Sprite poseCrouch, poseCrouchLight, poseCrouchHeavy;
    public Sprite poseDash, poseDashLight, poseDashHeavy;
    public Sprite poseBlock, poseSpecial, poseKO;

    public string charStatus;

    // Start is called before the first frame update
    void Start()
    {
        charAlive = true;
        charHPCurrent = charHPMax;
        hpSlider.value = 1.0f;
        charNameText.text = charName;

        UpdateStatus();
    }

    public void CheckTurn()
    {
        if (activeTurn)
        {
            isEvading = false;
            gameObject.transform.position = Vector3.zero;
            if (gameObject.tag.Equals("Hero"))
            {
                GameManager.instance.GetComponent<UIManager>().ShowMenuMain();

                //reset to idle pose on turn start
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutral;
            }
            else
            {
                //set to atk pose on turn start for enemy
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutralLight;

                //enemy targets random player with random action
                GameManager.instance.EnemyTurn();
            }
            GameManager.instance.GetComponent<UIManager>().UpdateTimerText(charSpeedCurrent.ToString());
        }
        else
        {
            gameObject.transform.position = startPosition;
            GameManager.instance.GetComponent<UIManager>().HideAllMenus();
        }

        UpdateStatus();
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

        UpdateStatus();
    }

    public void UpdateStatus()
    {
        charStatus = "";

        if (charAlive)
        {
            if (charStrengthCurrent != charStrengthBase)
            {
                charStatus += "ATK ";
                charStatus += charStrengthCurrent > charStrengthBase ? "+" : "-";
                charStatus += Mathf.Abs(charStrengthCurrent - charStrengthBase).ToString();
            }

            if (charDefenseCurrent != charDefenseBase)
            {
                charStatus += " DEF ";
                charStatus += charDefenseCurrent > charDefenseBase ? "+" : "-";
                charStatus += Mathf.Abs(charDefenseCurrent - charDefenseBase).ToString();
            }

            if (isEvading)
            {
                charStatus += " EVADE";
            }
        }
        else
        {
            charStatus = "KO";
        }

        charStatusText.text = charStatus;
    }

    public void UpdatePose()
    {
        switch (charPose)
        {
            case "neutral":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutral;
                break;
            case "neutral_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutralLight;
                break;
            case "neutral_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutralHeavy;
                break;
            case "jump":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseJump;
                break;
            case "jump_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseJumpLight;
                break;
            case "jump_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseJumpHeavy;
                break;
            case "crouch":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseCrouch;
                break;
            case "crouch_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseCrouchLight;
                break;
            case "crouch_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseCrouchHeavy;
                break;
            case "dash":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseDash;
                break;
            case "dash_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseDashLight;
                break;
            case "dash_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseDashHeavy;
                break;
            case "block":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseBlock;
                break;
            case "block_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutralLight;
                break;
            case "block_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutralHeavy;
                break;
            case "special":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseSpecial;
                break;
            case "ko":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseKO;
                break;
            default:
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poseNeutral;
                break;
        }
    }
}
