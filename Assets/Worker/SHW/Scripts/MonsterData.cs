using System.Xml.Linq;

public class MonsterData : IDataLoader
{
    public int ID;  // ���� NO.
    public string Name;     // ���� �̸�
    public bool AttackType;  // ���� Ÿ�� true�� ��� ���Ÿ�
    public float AttackRage;   // ���� ��Ÿ�
    public int Rage;    // �����Ÿ�
    public bool CanSkill; // ��ų���� : true�� ��� ��ų ����
    public int Attack;  // ���ݷ�
    public int Defense; // ����
    public float Hp;  // ü��
    public float AttackSpeed; // ����
    public float WalkSpeed;   // �ȱ�ӵ�
    public float RunSpeed;   // �ٱ��̼�

    public void Load(string[] fields)
    {
       int id = int.Parse(fields[0]);                  // Parse No.
       string name = fields[1];                           // Parse �̸�
       bool attackType = bool.Parse(fields[2]);         // Parse ���� Ÿ��
       float attackRage = float.Parse(fields[3]);        // ���� ����
       int rage = int.Parse(fields[4]);                // �����Ÿ�
       bool canSkill = bool.Parse(fields[5]);           // ��ų����
       int attack = int.Parse(fields[6]);              // ���ݷ�
       int defense = int.Parse(fields[7]);                 // ����
       float hp = float.Parse(fields[8]);                // ü��
       float attackSpeed = float.Parse(fields[9]);       // ����
       float walkSpeed = float.Parse(fields[10]);        // �ȱ�ӵ�
       float runSpeed = float.Parse(fields[11]);         // �ٱ�ӵ�
    }

    public MonsterData(int id, string name,bool attackType, int attack, int defense, float hp, float walkSpeed, float runSpeed, float attackSpeed, int rage, float attackRage)
    {
        ID = id;
        Name = name;
        AttackType = attackType;    
        Attack = attack;
        Defense = defense;
        Hp = hp;
        WalkSpeed = walkSpeed;
        RunSpeed = runSpeed;
        AttackSpeed = attackSpeed;
        Rage = rage;
        AttackRage = attackRage;
    }
}
