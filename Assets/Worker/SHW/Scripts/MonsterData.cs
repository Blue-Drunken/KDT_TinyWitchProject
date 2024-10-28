public class MonsterData : IDataLoader
{
    public int ID;              // ���� NO.
    public string Name;         // ���� �̸�
    public bool AttackType;     // ���� Ÿ�� true�� ��� ���Ÿ�
    public float AttackRage;    // ���� ��Ÿ�
    public int Rage;            // �����Ÿ�
    public bool CanSkill;       // ��ų���� : true�� ��� ��ų ����
    public int Attack;          // ���ݷ�
    public int Defense;         // ����
    public float Hp;            // ü��
    public float AttackSpeed;   // ����
    public float WalkSpeed;     // �ȱ�ӵ�
    public float RunSpeed;      // �ٱ��̼�

    public void Load(string[] fields)
    {
        ID = int.Parse(fields[0]);                  // Parse No.
        Name = fields[1];                           // Parse �̸�
        AttackType = bool.Parse(fields[2]);         // Parse ���� Ÿ��
        AttackRage = float.Parse(fields[3]);        // ���� ����
        Rage = int.Parse(fields[4]);                // �����Ÿ�
        CanSkill = bool.Parse(fields[5]);           // ��ų����
        Attack = int.Parse(fields[6]);              // ���ݷ�
        Defense = int.Parse(fields[7]);             // ����
        Hp = float.Parse(fields[8]);                // ü��
        AttackSpeed = float.Parse(fields[9]);       // ����
        WalkSpeed = float.Parse(fields[10]);        // �ȱ�ӵ�
        RunSpeed = float.Parse(fields[11]);         // �ٱ�ӵ�
    }
}
