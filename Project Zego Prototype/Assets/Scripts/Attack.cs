using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //attack ranges
    [SerializeField]
    private float attackLightMin = 0.5f;
    [SerializeField]
    private float attackLightMax = 0.6f;
    [SerializeField]
    private float attackHeavyMin = 0.9f;
    [SerializeField]
    private float attackHeavyMax = 1.0f;

    public int damageCalc(int strength)
    {
        float damage = 0f;

        //check menu desc for damage calc
        if (GameManager.instance.GetComponent<UIManager>().menuText.text == GameManager.instance.GetComponent<UIManager>().menuAttackLight)
        {
            damage = strength * Random.Range(attackLightMin, attackLightMax);
        }
        else if (GameManager.instance.GetComponent<UIManager>().menuText.text == GameManager.instance.GetComponent<UIManager>().menuAttackHeavy)
        {
            damage = strength * Random.Range(attackHeavyMin, attackHeavyMax);
        }

        return (int)damage;
    }
}
