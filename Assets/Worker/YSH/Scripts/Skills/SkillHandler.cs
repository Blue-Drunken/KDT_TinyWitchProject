using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    SkillBase[] _playerSkillSlot = new SkillBase[(int)Enums.PlayerSkillSlot.Length];

    Coroutine _castRoutine;

    public void EquipSkill(int skillID, Enums.PlayerSkillSlot slot)
    {
        // ID �˻�
        if (DataManager.Instance.SkillDict.TryGetValue(skillID, out SkillData data) == false)
        {
            Debug.LogError($"SkillHandler EquipSkill failed... / ID : {skillID}");
            Debug.LogError("Please Check data");
            return;
        }

        SkillBase prefab = Resources.Load<SkillBase>($"Prefabs/Skills/{data.ClassName}");
        if (prefab == null)
        {
            Debug.LogError($"Can't find SkillBase Component! / ID : {skillID}");
            return;
        }

        SkillBase skill = Instantiate(prefab);
        skill.SetData(data.ID);
        skill.transform.SetParent(gameObject.transform);

        // �ش� ���Կ� ��ų�� �����ϸ� �����Ѵ�.
        if (_playerSkillSlot[(int)slot] != null)
            UnEquipSkill(slot);

        _playerSkillSlot[(int)slot] = skill;
    }

    public void UnEquipSkill(Enums.PlayerSkillSlot slot)
    {
        if (_playerSkillSlot[(int)slot] = null)
            return;

        // slot�� �ִ� ��ų�� Ƣ����� �ؾ���

        // slot���� ����
        _playerSkillSlot[(int)slot] = null;
    }

    public void DoSkill(Enums.PlayerSkillSlot slot, Vector3 startPos)
    {
        // ���Կ� ��ų�� �����ϴ��� ��
        if (_playerSkillSlot[(int)slot] == null)
            return;

        // ��Ÿ�� üũ
        if (_playerSkillSlot[(int)slot].CurrentCoolTime > 0)
            return;

        if (_castRoutine != null)
            return;

        Cast(slot, startPos);
    }

    public void Cast(Enums.PlayerSkillSlot slot, Vector3 startPos)
    {
        _playerSkillSlot[(int)slot].StartPos = startPos;
        _playerSkillSlot[(int)slot].User = gameObject;
        // ���� ���� ���� �ʿ�
        _castRoutine = StartCoroutine(CastRoutine(slot));
    }

    IEnumerator CastRoutine(Enums.PlayerSkillSlot slot)
    {
        WaitForSeconds castTime = new WaitForSeconds(_playerSkillSlot[(int)slot].SkillData.CastTime);

        Debug.Log($"Start Cast : {_playerSkillSlot[(int)slot].SkillData.Name}");
        yield return castTime;
        _playerSkillSlot[(int)slot].DoSkill();
        _castRoutine = null;
    }

    public void StopSkill(Enums.PlayerSkillSlot slot)
    {
        // ���Կ� ��ų�� �����ϴ��� ��
        if (_playerSkillSlot[(int)slot] == null)
            return;

        if (_castRoutine != null)
        {
            StopCoroutine(_castRoutine);
            _castRoutine = null;
            return;
        }

        _playerSkillSlot[(int)slot].StopSkill();
    }
}
