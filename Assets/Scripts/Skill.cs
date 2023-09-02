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

    public int skillSingleCost = 10;
    public int skillMultiCost = 10;

    public int selectedSkillCost;

    public int damageCalc(int strength, string skillType)
    {
        float damage = 0f;
        selectedSkillCost = 0;

        if (skillType.Contains("heavy"))
        {
            damage = strength * Random.Range(skillSingleMin, skillSingleMax);
            selectedSkillCost = skillSingleCost;
        }
        else if (skillType.Contains("light"))
        {
            damage = strength * Random.Range(skillMultiMin, skillMultiMax);
            selectedSkillCost = skillMultiCost;
        }

        return (int)damage;
    }
}
