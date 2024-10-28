using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoom : SkillBase
{
    public override void SetData(int id)
    {
        base.SetData(id);
    }

    public override void DoSkill(float attackPoint)
    {
        //AreaProjectile projectile = Instantiate(projectilePrefab, _user.transform.position + transform.right * _skillData.Range, Quaternion.identity) as AreaProjectile;

        // test ��ġ
        // �� ��ġ ��� ã������ Ȯ���ʿ�
        Vector3 dist = new Vector3(_skillData.Range, -1f, 0);
        AreaProjectile projectile = Instantiate(projectilePrefab, _user.transform.position + dist, Quaternion.identity) as AreaProjectile;
        projectile.SetDamage(_skillData.Damage * attackPoint);
        projectile.EnableTrigger();
        base.DoSkill(attackPoint);
    }
}
