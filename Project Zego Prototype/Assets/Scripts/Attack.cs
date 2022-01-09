using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public static Attack instance;

    //attack ranges
    [SerializeField]
    private float attackLightMin = 0.5f;
    [SerializeField]
    private float attackLightMax = 0.6f;
    [SerializeField]
    private float attackHeavyMin = 0.9f;
    [SerializeField]
    private float attackHeavyMax = 1.0f;

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

    public int damageCalc(int strength)
    {
        float damage = 0f;

        //check menu desc for damage calc
        if (UIManager.instance.menuText.text == UIManager.instance.menuAttackLight)
        {
            damage = strength * Random.Range(attackLightMin, attackLightMax);
        }
        else if (UIManager.instance.menuText.text == UIManager.instance.menuAttackHeavy)
        {
            damage = strength * Random.Range(attackHeavyMin, attackHeavyMax);
        }

        return (int)damage;
    }
}
