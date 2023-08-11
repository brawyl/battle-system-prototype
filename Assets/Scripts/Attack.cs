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
    private float attackHeavyMin = 0.8f;
    [SerializeField]
    private float attackHeavyMax = 1.0f;

    public int attackLightCost = 2;
    public int attackHeavyCost = 3;

    public int selectedAttackCost;

    public int damageCalc(int strength, string attackType)
    {
        float damage = 0f;
        selectedAttackCost = 0;

        if (attackType.Contains("light"))
        {
            damage = strength * Random.Range(attackLightMin, attackLightMax);
            selectedAttackCost = attackLightCost;
        }
        else if (attackType.Contains("heavy"))
        {
            damage = strength * Random.Range(attackHeavyMin, attackHeavyMax);
            selectedAttackCost = attackHeavyCost;
        }

        return (int)damage;
    }
}
