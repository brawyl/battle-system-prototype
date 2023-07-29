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
    private int attackLightCost = 6;
    [SerializeField]
    private int attackHeavyCost = 12;

    public int selectedAttackCost;

    public int damageCalc(int strength, string menuDesc)
    {
        float damage = 0f;
        selectedAttackCost = 0;

        //check menu desc for damage calc and set attack cost
        string type = menuDesc.Contains("HEAVY") ? "HEAVY" : "LIGHT";

        if (type == "LIGHT")
        {
            damage = strength * Random.Range(attackLightMin, attackLightMax);
            selectedAttackCost = attackLightCost;
        }
        else if (type == "HEAVY")
        {
            damage = strength * Random.Range(attackHeavyMin, attackHeavyMax);
            selectedAttackCost = attackHeavyCost;
        }

        return (int)damage;
    }

    public int getAttackCost(string type)
    {
        if (type == "LIGHT")
        {
            return attackLightCost;
        }
        else if (type == "HEAVY")
        {
            return attackHeavyCost;
        }

        //default to 0 cost
        return 0;
    }
}
