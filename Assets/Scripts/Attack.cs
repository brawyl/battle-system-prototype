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

    public int damageCalc(int strength, string type)
    {
        float damage = 0f;

        //check menu desc for damage calc
        if (type == "LIGHT")
        {
            damage = strength * Random.Range(attackLightMin, attackLightMax);
        }
        else if (type == "HEAVY")
        {
            damage = strength * Random.Range(attackHeavyMin, attackHeavyMax);
        }

        return (int)damage;
    }
}
