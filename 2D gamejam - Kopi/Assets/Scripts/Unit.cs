using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;
    public int healAmount;

    public int maxHealth;
    public int currentHealth;
    public int defense;
    private int actualDefense;

    public bool TakeDamage(int dmg)
    {
        actualDefense = defense / 100;
        currentHealth -= dmg;

        if(currentHealth <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Heal()
    {
        currentHealth += healAmount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
