using UnityEngine;

public class DeathWormController : MonoBehaviour
{
    // ���� ���� �������� �ҷ�����
    // = ���� ������ �ҷ����� �Ͱ� ���� �Ѵ�.

    // ���� ������ ��

    //���� ����
    [Header("State")]
    public int id = 14; // ������ id 14��
    public bool attackType;        // ���� Ÿ�� true�� ��� ���Ÿ�
    public float attackRage;       // ���� ��Ÿ�
    public float range;             // �����Ÿ�
    public bool canSkill;          // ��ų����
    public float attack;            // ���ݷ�
    public float def;              // ����
    public float hp;               // ü��
    public float attackSpeed;      // ����

    // ���� ������ �Դ� ���� ü�� �κ�
    public float curHp;                      // ���� ���� ü��

    public float walkSpeed;        // �ȱ��̼�
    public float runSpeed;         // �ٱ��̼�

    public float bulletSpeed;      // ����ü �߻� �ӵ�

}
