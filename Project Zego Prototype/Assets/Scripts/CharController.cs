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
    private Slider hpSlider;

    public string charName;
    public int charHPMax;
    public int charHPCurrent;
    public int charStrength;
    public int charDefense;
    public int charSpeed;
    public bool charAlive;

    // Start is called before the first frame update
    void Start()
    {
        charAlive = true;
        charHPCurrent = charHPMax;
        hpSlider.value = 1.0f;
        charNameText.text = charName;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void damageTarget(int damage, GameObject target)
    {
        target.GetComponent<CharController>().TakeDamage(damage);
    }
}
