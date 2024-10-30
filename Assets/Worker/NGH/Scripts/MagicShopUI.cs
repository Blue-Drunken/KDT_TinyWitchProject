using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicShopUI : MonoBehaviour
{
    // ���� ���� UI ���
    [Header("MagicShop UI")]
    public Button magicShopExit;
    public Button[] skillButtons; // ���� UI�� �ִ� �� ��ų ��ư �迭
    public int[] skillCosts;      // �� ��ų�� �ر� ���

    private void Start()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int skillID = i;
            skillButtons[i].onClick.AddListener(() => UnlockSkillInShop(skillID));
        }
    }

    private void UnlockSkillInShop(int skillID)
    {
        int cost = skillCosts[skillID];

        if (GameManager.Instance.HasEnoughGold(cost))
        {
            GameManager.Instance.SpendGold(cost);
            SkillUnlockManager.Instance.UnlockSkill(skillID);
        }
        else
        {
            Debug.Log("��尡 �����մϴ�.");
        }
    }
}
