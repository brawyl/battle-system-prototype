using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuTarget;

    public TMP_Text timerText;
    public TMP_Text menuText;

    public GameObject gameOverScreen;
    public TMP_Text gameOverText;

    private string menuCode;

    //menu description strings
    [SerializeField]
    private string menuAttackLight = "ATTACK LIGHT";
    [SerializeField]
    private string menuAttackHeavy = "ATTACK HEAVY";

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void UpdateTimerText(string speed)
    {
        timerText.text = speed.ToString();
    }

    public void ShowGameOverScreen(bool gameOver)
    {
        gameOverScreen.SetActive(gameOver);
    }
}
