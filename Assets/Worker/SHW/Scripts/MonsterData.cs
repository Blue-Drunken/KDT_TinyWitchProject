public class MonsterData : IDataLoader
{
    public int id;  // ���� NO.
    public string name;     // ���� �̸�
    public bool attackType;  // ���� Ÿ�� true�� ��� ���Ÿ�
    public float attackRage;   // ���� ��Ÿ�
    public int rage;    // �����Ÿ�
    public bool canSkill; // ��ų���� : true�� ��� ��ų ����
    public int attack;  // ���ݷ�
    public int def; // ����
    public float hp;  // ü��
    public float attackSpeed; // ����
    public float walkSpeed;   // �ȱ�ӵ�
    public float runSpeed;   // �ٱ��̼�

    public void Load(string[] fields)
    {
        id = int.Parse(fields[0]);                  // Parse No.
        name = fields[1];                           // Parse �̸�
        attackType = bool.Parse(fields[2]);         // Parse ���� Ÿ��
        attackRage = float.Parse(fields[3]);        // ���� ����
        rage = int.Parse(fields[4]);                // �����Ÿ�
        canSkill = bool.Parse(fields[5]);           // ��ų����
        attack = int.Parse(fields[6]);              // ���ݷ�
        def = int.Parse(fields[7]);                 // ����
        hp = float.Parse(fields[8]);                // ü��
        attackSpeed = float.Parse(fields[9]);       // ����
        walkSpeed = float.Parse(fields[10]);        // �ȱ�ӵ�
        runSpeed = float.Parse(fields[11]);         // �ٱ�ӵ�
    }
}
