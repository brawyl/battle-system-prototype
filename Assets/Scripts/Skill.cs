using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    //skill damage multiplier ranges
    [SerializeField]
    private float skillSingleMin = 1.6f;
    [SerializeField]
    private float skillSingleMax = 2.0f;
    [SerializeField]
    private float skillMultiMin = 0.6f;
    [SerializeField]
    private float skillMultiMax = 1.0f;

    [SerializeField]
    private int skillSingleCost = 10;
    [SerializeField]
    private int skillMultiCost = 10;

    public int selectedSkillCost;

    public string skillName;
    public int skillCost;

    public int damageCalc(int strength, string menuDesc)
    {
        float damage = 0f;
        selectedSkillCost = 0;

        //check menu desc for damage calc and set attack cost
        string type = menuDesc.Contains("SINGLE") ? "SINGLE" : "MULTI";

        if (type == "SINGLE")
        {
            damage = strength * Random.Range(skillSingleMin, skillSingleMax);
            selectedSkillCost = skillSingleCost;
        }
        else if (type == "MULTI")
        {
            damage = strength * Random.Range(skillMultiMin, skillMultiMax);
            selectedSkillCost = skillMultiCost;
        }

        return (int)damage;
    }

    public int getSkillCost(string type)
    {
        if (type == "SINGLE")
        {
            return skillSingleCost;
        }
        else if (type == "MULTI")
        {
            return skillMultiCost;
        }

        //default to 0 cost
        return 0;
    }
}
