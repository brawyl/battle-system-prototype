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
    [SerializeField]
    private Sprite[] poses;
    /*** POSES INDEX REFERENCE
     * 0 - Neutral, 1 - Jump, 2 - Crouch, 3 - Dash, 4 - Block,
     * 5 - Neutral Attack Light, 6 - Jump Attack Light, 7 - Crouch Attack Light, 8 - Dash Attack Light, 9 - Special,
     * 10 - Neutral Attack Heavy, 11 - Jump Attack Heavy, 12 - Crouch Attack Heavy, 13 - Dash Attack Heavy, 14 - KO
    ***/

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

    [SerializeField]
    public string specialty = "";

    public string charPose;

    public GameObject targetSelect;

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
            gameObject.transform.position = Vector3.zero;
            if (gameObject.tag.Equals("Hero"))
            {
                GameManager.instance.StartPlayerTurn();
            }
            else
            {
                GameManager.instance.StartEnemyTurn();
            }
            GameManager.instance.GetComponent<UIManager>().UpdateTimerText(charSpeedCurrent.ToString());
        }
        else
        {
            gameObject.transform.position = startPosition;
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
        else if (damage > 0 && charPose != "block")
        {
            charPose = "neutral";
            UpdatePose();
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
        }
        else
        {
            charStatus = "KO";
        }

        charStatusText.text = charStatus;
    }

    public void UpdatePose()
    {
        if (poses.Length < 15) return;

        switch (charPose)
        {
            case "neutral":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[0];
                break;
            case "block_light": //attacks from block pose just fall thru to neutral attacks
            case "neutral_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[5];
                break;
            case "block_heavy":
            case "neutral_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[10];
                break;
            case "jump":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[1];
                break;
            case "jump_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[6];
                break;
            case "jump_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[11];
                break;
            case "crouch":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[2];
                break;
            case "crouch_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[7];
                break;
            case "crouch_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[12];
                break;
            case "dash":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[3];
                break;
            case "dash_light":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[8];
                break;
            case "dash_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[13];
                break;
            case "block":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[4];
                break;
            case "special_light":
            case "special_heavy":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[9];
                break;
            case "ko":
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[14];
                break;
            default:
                gameObject.GetComponentInChildren<SpriteRenderer>().sprite = poses[0];
                break;
        }
    }
}
