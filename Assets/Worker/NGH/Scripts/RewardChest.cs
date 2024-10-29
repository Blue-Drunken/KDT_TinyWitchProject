using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    public int dataIndex; // ����� ���̺� �ε���
    public GameObject hpItemPrefab, skillPrefab, goldPrefab, rareGoldPrefab;

    public bool testBool;

    private void Awake()
    {
    }

    private void Start()
    {
        DataManager.Instance.OnLoadCompleted += Test;
    }

    private void Test()
    {
        for (int i = 0; DataManager.Instance.SkillDict.Count > i; i++)
        {
            SkillUnlockManager.Instance.UnlockSkill(i);
        }
        OpenChest();
    }

    private void OnDisable()
    {
        DataManager.Instance.OnLoadCompleted -= Test;
    }

    public void OpenChest()
    {
        DropData selectedTable = DataManager.Instance.DropDict[dataIndex];
        GenerateRewards(selectedTable);
    }

    private void GenerateRewards(DropData table)
    {
        // ��� ����
        int goldAmount = Random.Range(table.MinGold, table.MaxGold);
        GameObject goldDrop = Instantiate(goldPrefab, transform.position, Quaternion.identity);
        goldDrop.GetComponent<DropItem>().Initialize(DropItem.ItemType.Gold, goldAmount);

        // HP�� ��� ���� Ȯ��
        if (table.IsDropHP)
        {
            GameObject potionDrop = Instantiate(hpItemPrefab, transform.position, Quaternion.identity);
            potionDrop.GetComponent<DropItem>().Initialize(DropItem.ItemType.Potion,0);
        }

        List<int> availableLowSkills = new List<int>();
        List<int> availableMidSkills = new List<int>();
        List<int> availableHighSkills = new List<int>();

        // �رݵ� ��ų���� ������� �ݺ�
        for (int i= 0; i < SkillUnlockManager.Instance.unlockedSkills.Count; i++)
        {
            Enums.Grade grade = DataManager.Instance.SkillDict[SkillUnlockManager.Instance.unlockedSkills[i]].Grade;
            switch(grade)
            {
                case Enums.Grade.Low :
                    availableLowSkills.Add(SkillUnlockManager.Instance.unlockedSkills[i]);
                        break;
                case Enums.Grade.Mid:
                    availableMidSkills.Add(SkillUnlockManager.Instance.unlockedSkills[i]);
                    break;
                case Enums.Grade.High:
                    availableHighSkills.Add(SkillUnlockManager.Instance.unlockedSkills[i]);
                    break;
            }
        }

        // ��ų ID ����Ʈ (0~8����)���� QWER�� ��ϵ��� ���� ��ų�� �����ϱ� ���� ����� �����մϴ�.

        // TEST
        List<int> equippedSkills = new List<int>();
        for (int i = 0; i < (int)Enums.PlayerSkillSlot.Length; i++)
        {
            if (GameManager.Instance.player.handler.PlayerSkillSlot[i] != null)
            {
                equippedSkills.Add(GameManager.Instance.player.handler.PlayerSkillSlot[i].SkillData.ID);
            }
        }
        availableLowSkills.RemoveAll(skill => equippedSkills.Contains(skill));
        availableMidSkills.RemoveAll(skill => equippedSkills.Contains(skill));
        availableHighSkills.RemoveAll(skill => equippedSkills.Contains(skill));
        //

        // ��ų ��� Ȯ�� üũ
        float skillRoll = Random.value;
        if (skillRoll <= table.LowGradePercent && availableLowSkills.Count > 0)
        {
            // lowSkill �߿��� ���� ����
            int skillID = availableLowSkills[Random.Range(0, availableLowSkills.Count)];
            CreateSkill(DropItem.ItemType.Skill, skillID);
        }
        else if (skillRoll <= table.LowGradePercent + table.MidGradePercent && availableMidSkills.Count > 0)
        {
            // midSkill �߿��� ���� ����
            int skillID = availableMidSkills[Random.Range(0, availableMidSkills.Count)];
            CreateSkill(DropItem.ItemType.Skill, skillID);
        }
        else if (skillRoll <= table.LowGradePercent + table.MidGradePercent + table.HighGradePercent && availableHighSkills.Count > 0)
        {
            // highSkill �߿��� ���� ����
            int skillID = availableHighSkills[Random.Range(0, availableHighSkills.Count)];
            CreateSkill(DropItem.ItemType.Skill, skillID);
        }

        // �͸� ��� ��� üũ
        if (Random.value <= table.BonusGoldPercent)
        {
            Instantiate(rareGoldPrefab, transform.position, Quaternion.identity);
        }
    }

    private void CreateSkill(DropItem.ItemType skillType, int skillID)
    {
        GameObject skillDrop = Instantiate(skillPrefab, transform.position, Quaternion.identity);
        skillDrop.GetComponent<DropItem>().Initialize(skillType, skillID);
    }
}