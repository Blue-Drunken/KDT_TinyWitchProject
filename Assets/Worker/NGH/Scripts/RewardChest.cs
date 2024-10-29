using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    public List<DropTable> dropTableList; // ���̺� ����Ʈ
    public int tableIndex; // ����� ���̺� �ε���
    public GameObject hpItemPrefab, skillPrefab, goldPrefab, rareGoldPrefab;

    private void Awake()
    {
        
    }

    public void OpenChest()
    {
        DropTable selectedTable = dropTableList[tableIndex];
        GenerateRewards(selectedTable);
    }

    private void GenerateRewards(DropTable table)
    {
        // ��� ����
        int goldAmount = Random.Range(table.minGold, table.maxGold);
        GameObject goldDrop = Instantiate(goldPrefab, transform.position, Quaternion.identity);
        goldDrop.GetComponent<DropItem>().Initialize(DropItem.ItemType.Gold, goldAmount);

        // HP�� ��� ���� Ȯ��
        if (table.hasHPItem)
        {
            Instantiate(hpItemPrefab, transform.position, Quaternion.identity);
        }

        // ��ų ID ����Ʈ (0~8����)���� QWER�� ��ϵ��� ���� ��ų�� �����ϱ� ���� ����� �����մϴ�.
        List<int> availableLowSkills = new List<int> { 0, 1, 2 };
        List<int> availableMidSkills = new List<int> { 3, 4, 5 };
        List<int> availableHighSkills = new List<int> { 6, 7, 8 };

        // QWER�� ��ϵ� ��ų ID�� �����ͼ� ���� ��Ͽ��� �����մϴ�.
        List<int> registeredSkills = UIManager.Instance.GetRegisteredSkills(); // QWER ��ų ����Ʈ �Լ�

        availableLowSkills.RemoveAll(skill => registeredSkills.Contains(skill));
        availableMidSkills.RemoveAll(skill => registeredSkills.Contains(skill));
        availableHighSkills.RemoveAll(skill => registeredSkills.Contains(skill));

        // ��ų ��� Ȯ�� üũ
        float skillRoll = Random.value;
        if (skillRoll <= table.lowSkillChance && availableLowSkills.Count > 0)
        {
            // lowSkill �߿��� ���� ����
            int skillID = availableLowSkills[Random.Range(0, availableLowSkills.Count)];
            CreateSkill(DropItem.ItemType.Skill, skillID);
        }
        else if (skillRoll <= table.lowSkillChance + table.midSkillChance && availableMidSkills.Count > 0)
        {
            // midSkill �߿��� ���� ����
            int skillID = availableMidSkills[Random.Range(0, availableMidSkills.Count)];
            CreateSkill(DropItem.ItemType.Skill, skillID);
        }
        else if (skillRoll <= table.lowSkillChance + table.midSkillChance + table.highSkillChance && availableHighSkills.Count > 0)
        {
            // highSkill �߿��� ���� ����
            int skillID = availableHighSkills[Random.Range(0, availableHighSkills.Count)];
            CreateSkill(DropItem.ItemType.Skill, skillID);
        }

        // �͸� ��� ��� üũ
        if (Random.value <= table.rareGoldChance)
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