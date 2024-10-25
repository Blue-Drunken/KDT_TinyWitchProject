using UnityEngine;

public class MonsterState : MonoBehaviour
{
    // ���, ����, ����, ���, �ǵ��ư���, ��ų, �̵�, �ǰ�
    public enum State { Idle, Running, Attack, Return, Skill, Walking, Dead, ISHit }

    // ������
    [SerializeField] State curState;
    // ������ �÷��̾�(�ӽ�)
    [SerializeField] GameObject player;
    // �⺻ ��ġ(�ӽ�)
    public Vector3 spawnPoint;

    // ����� ���ϸ�����
    [SerializeField] Animator animator;
    // ����Ҷ� �Ʒ�ó�� �ؽ�ȭ �ؼ� ���
    private static int idleHash = Animator.StringToHash("Idle");
    private static int walkHash = Animator.StringToHash("Walk");


    // ������ ���̺�� ���� �����
    // ��Ÿ�, ��ų����, ���ݷ�, ����, ü��, ����, �̼�, ��ų ��

    // ���ϴ� �ӽ������� �ۼ��� ���ݵ���
    [Header("State")]
    [SerializeField] float attack;  // ���ݷ�
    [SerializeField] float def; // ����
    [SerializeField] float hp;  // ü��
    [SerializeField] float speed;   // ����
    [SerializeField] float attackSpeed; // �̼�
    [SerializeField] float rage;    // �����Ÿ�
    [SerializeField] float attackRage;   // ���� ��Ÿ�
    [SerializeField] bool canSkill; // ��ų����
    [SerializeField] bool attackType;  // ���� Ÿ��

    private void Awake()
    {
        // ������ ���̺��� ���� ��������

        // �ʱ� ���� ����
        spawnPoint = transform.position;

        // �ִϸ����� �������
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ������ �÷��̾� ������Ʈ?������Ʈ ã��
        // Transform playerPos = player.GetComponent<Transform>();

        Debug.Log(curState);
   

        // ��������
        switch (curState)
        {
            case State.Idle:    // ��� = ������
                Idle();
                break;
            case State.Running:     // �ٱ� = ����
                Running();
                break;
            case State.Return:  // ���� = �ǵ��ư���
                Return();
                return;
            case State.Attack: // ����
                Attack();
                return;
            case State.Skill: // ��ų
                Skill();
                return;
            case State.Walking: // �̵� = ��ȸ
                Walking();
                return;
            case State.ISHit:   // �ǰ�
                IsHit();
                return;
            case State.Dead:  // (�ӽ�) ���
                Dead();
                return;
        }
    }

    public void Idle()
    {
        // ��� �ִϸ��̼� ��� ���
        animator.SetBool("isIdle", true);

        // �÷��̾ ���� ���� ������ ���
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < rage)
        {
            curState = State.Running;
        }

        // 

        // ���� �ð��� �Ǹ� 
        // �ڷ�ƾ
    }

    public void Running()
    {
        // �ٴ� �ִϸ��̼� ���
        // animator.SetFloat("speed", 1);

        // Ÿ�� ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        // �÷��̾ ������ ����� ���
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) > rage)
        {
            curState = State.Return;
        }

        // �÷��̾ ���ݹ����� ������ ���
        if (Vector3.Distance(player.transform.position, gameObject.transform.position) < attackRage)
        {
            curState = State.Attack;
        }
    }

    public void Return()
    {
        // (TODO)�ȱ� �ִϸ��̼� ���

        // ���� �ڸ��� �̵�
        transform.position = Vector3.MoveTowards(transform.position, spawnPoint, speed * Time.deltaTime);

        // ���� �ڸ��� �̵� �Ϸ�� 
        if (transform.position == spawnPoint)
        {
            curState = State.Idle;
        }
    }

    public void Attack()
    {
        // ���� �ִϸ��̼� ���

        // �ٰŸ� ���Ÿ� Ȯ��

        // �÷��̾� ü�� ����

    }

    public void Dead()
    {
        // ü���� �� ������� ���
        Destroy(gameObject);
    }

    public void Skill()
    {
        // ��ų�ʰ� �����ؼ� �ۼ�
        // (���ϴ� �ӽ�) 
        // ��ų���� �����Ѵٸ�
        // ��ų �� ������Ʈ �� �̸��� ���� �κ��� ������
        // ������Ʈ�� �̸��� ���Ͽ� ��ų�� �������� �������?
    }

    public void Walking()
    {
        // �̵� �ִϸ��̼� ���

        // �̵�
        transform.position += Vector3.left * speed * Time.deltaTime;
        // �� ���ϻ��·� ���ڸ��� ���ư���?
    }

    public void IsHit()
    {
        // �ǰ�
        // ������ ü�� ����
        // �ǰ� ���ϸ��̼� �缺
        // ����
    }

    // IEnumerator 

    // (TODO)���ϻ����� ��� �ҷ��� �Լ� �ۼ�
}
