using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerStats
{
    public float attackPower = 10f; 
    public float defense = 5f; 
    public float maxHealth = 100f;
    public float currentHealth;

    public float invincibleDuration = 1.5f; // ���� �ð� (��)
    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    //ü�� �ʱ�ȭ
    public PlayerStats()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        //// ���� �ð� ����
        //if (isInvincible)
        //{
        //    invincibleTimer -= Time.deltaTime;
        //    if (invincibleTimer <= 0)
        //    {
        //        isInvincible = false;
        //    }
        //}
    }

    public UnityAction OnChangedHP;

    //������ ���
    public void TakeDamage(float damage)
    {
        /*
        if (isInvincible)
        {
            Debug.Log("���������Դϴ�.");
            return; // ���� ������ ���� ���� ����
        }
        */

        float actualDamage = damage - defense;
        actualDamage = Mathf.Clamp(actualDamage, 0, actualDamage);
        currentHealth -= actualDamage;

        OnChangedHP?.Invoke();

        Debug.Log($"�ǰ� ����! : ���� ü�� = {currentHealth}");

        invincibleTimer = invincibleDuration;
        isInvincible = true;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //���
    private void Die()
    {
        Debug.Log("Player has died.");  
    }
}

