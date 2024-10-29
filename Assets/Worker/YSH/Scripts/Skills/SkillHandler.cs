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

        SkillBase prefab = ResourceManager.Instance.Load<SkillBase>($"Prefabs/Skills/{data.ClassName}");
        if (prefab == null)
        {
            Debug.LogError($"Can't find SkillBase Component! / ID : {skillID}");
            return;
        }

        SkillBase skill = Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
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

        // slot�� �ִ� ��ų�� Ƣ����� �ؾ��� (fix ��)


        // slot���� ����
        _playerSkillSlot[(int)slot] = null;
    }

    public void DoSkill(Enums.PlayerSkillSlot slot, Transform startPos, float attackPoint)
    {
        // ���Կ� ��ų�� �����ϴ��� ��
        if (_playerSkillSlot[(int)slot] == null)
            return;

        // ��Ÿ�� üũ
        if (_playerSkillSlot[(int)slot].CurrentCoolTime > 0)
            return;

        if (_castRoutine != null)
            return;

        Cast(slot, startPos, attackPoint);
    }

    public void Cast(Enums.PlayerSkillSlot slot, Transform startPos, float attackPoint)
    {
        _playerSkillSlot[(int)slot].StartPos = startPos;
        _playerSkillSlot[(int)slot].User = gameObject;
        // ���� ���� ���� �ʿ�
        _castRoutine = StartCoroutine(CastRoutine(slot, attackPoint));
    }

    IEnumerator CastRoutine(Enums.PlayerSkillSlot slot, float attackPoint)
    {
        WaitForSeconds castTime = new WaitForSeconds(_playerSkillSlot[(int)slot].SkillData.CastTime);

        Debug.Log($"Start Cast : {_playerSkillSlot[(int)slot].SkillData.Name}");
        _playerSkillSlot[(int)slot].DoCast();

        yield return castTime;
        _playerSkillSlot[(int)slot].StopCast();
        _playerSkillSlot[(int)slot].DoSkill(attackPoint);
        _castRoutine = null;
    }

    public void StopSkill(Enums.PlayerSkillSlot slot)
    {
        // ���Կ� ��ų�� �����ϴ��� ��
        if (_playerSkillSlot[(int)slot] == null)
            return;

        if (_castRoutine != null)
        {
            _playerSkillSlot[(int)slot].StopCast();
            StopCoroutine(_castRoutine);
            _castRoutine = null;
            return;
        }

        _playerSkillSlot[(int)slot].StopSkill();
    }
}
