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

    [SerializeField]
    private int attackLightCost = 7;
    [SerializeField]
    private int attackHeavyCost = 15;

    public int attackCost;

    public int damageCalc(int strength, string menuDesc)
    {
        float damage = 0f;
        attackCost = 0;

        //check menu desc for damage calc
        string type = menuDesc.Contains("HEAVY") ? "HEAVY" : "LIGHT";

        if (type == "LIGHT")
        {
            damage = strength * Random.Range(attackLightMin, attackLightMax);
            attackCost = attackLightCost;
        }
        else if (type == "HEAVY")
        {
            damage = strength * Random.Range(attackHeavyMin, attackHeavyMax);
            attackCost = attackHeavyCost;
        }

        return (int)damage;
    }
}
