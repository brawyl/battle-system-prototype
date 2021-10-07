using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject menuMain;
    public GameObject menuAttack;
    public GameObject menuTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void hideAllMenus()
    {
        menuMain.gameObject.SetActive(false);
        menuAttack.gameObject.SetActive(false);
        menuTarget.gameObject.SetActive(false);
    }

    public void showMenuMain()
    {
        hideAllMenus();
        menuMain.gameObject.SetActive(true);
    }

    public void showMenuAttack()
    {
        hideAllMenus();
        menuAttack.gameObject.SetActive(true);
    }

    public void showMenuTarget()
    {
        hideAllMenus();
        menuTarget.gameObject.SetActive(true);
    }
}
