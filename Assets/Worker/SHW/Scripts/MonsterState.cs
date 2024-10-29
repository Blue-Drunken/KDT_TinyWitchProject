using System.Collections;
using UnityEngine;

public class MonsterState : MonoBehaviour
{
    // ���, ����, ����, ���, �ǵ��ư���, ��ų, �̵�, �ǰ�
    public enum State { Idle, Running, Attack, Return, Skill, Walking, Dead, IsHit }

    // ������
    [SerializeField] State curState;
    // ������ �÷��̾�(�ӽ�)
    [SerializeField] GameObject player;
    // �⺻ ��ġ(�ӽ�)
    public Vector3 spawnPoint;
    // ���Ÿ� ���ݽ� �߻��� ������
    [SerializeField] GameObject bulletPrefab;

    // ����� ���ϸ�����
    [SerializeField] Animator animator;

    // ���ϴ� �ӽ������� �ۼ��� ���ݵ���
    [Header("State")]
    [SerializeField] int id;
    [SerializeField] float attack;  // ���ݷ�
    [SerializeField] float def; // ����
    [SerializeField] float hp;  // ü��
    public float curHp;
    [SerializeField] float walkSpeed;   // �ȱ��̼�
    [SerializeField] float runSpeed;   // �ٱ��̼�
    [SerializeField] float attackSpeed; // �̼�
    [SerializeField] float rage;    // �����Ÿ�
    [SerializeField] float attackRage;   // ���� ��Ÿ�
    [SerializeField] bool canSkill; // ��ų����
    [SerializeField] bool attackType;  // ���� Ÿ�� true�� ��� ���Ÿ�

    protected MonsterData _monsterData;
    public MonsterData MonsterData { get { return _monsterData; } }

    private void Awake()
    {
        // �ִϸ����� �������
        animator = GetComponent<Animator>();

        // ���� ����Ʈ ����
        spawnPoint = transform.position;

        // �÷��̾� Ȯ��(�ϴ� �̸����� �÷��̾� ������Ʈ ã�´�)
        player = GameObject.Find("testPlayer");

        // ���� ü�� = ����ü������ ����
        curHp = hp;
    }
    private void Start()
    {
        LoadMonsterData(id);
    }

    public void LoadMonsterData(int id)
    {
        // id ����
        // id�� �����͸������� ��ųʸ� ���� value���� �����´�

        // ���� Ȯ�ο�
        Debug.Log($"MonsterDict�� ����� ������ ����: {_monsterData.ID}");
        Debug.Log($"��û�� ���� ID: {id}");

        // id�� �ش��ϴ� �����Ͱ� �����ϴ��� Ȯ���ϰ�, �������� ���� ��� ���� ���
        if (DataManager.Instance.MonsterDict.TryGetValue(id, out MonsterData data) == false)
        {
            Debug.LogError($"MonsterData�� ã�� �� �����ϴ�. ID: {id}");
            return;
        }

        _monsterData = data;

        // ������ ���� ������ ���� �����Ϳ� �Ҵ��Ѵ�.
        attack = _monsterData.Attack;
        def = _monsterData.Defense;
        hp = _monsterData.Hp;
        walkSpeed = _monsterData.WalkSpeed;
        runSpeed = _monsterData.RunSpeed;
        attackSpeed = _monsterData.AttackSpeed;
        rage = _monsterData.Rage;
        attackRage = _monsterData.AttackRage;
        canSkill = _monsterData.CanSkill;
        attackType = _monsterData.AttackType;
    }

    private void Update()
    {
        // ���� Ȯ�ο� ����� �α�
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
            /*
        case State.IsHit:   // �ǰ�
            IsHit();
            return;*/
            case State.Dead:  // (�ӽ�) ���
                Dead();
                return;
        }

        // ���������� ������ ��� 
        // ���������� �������� ���


        // (TODO)���ϻ����� ��� �ҷ��� �Լ� �ۼ�
    }

    public void Idle()
    {
        // ��� �ִϸ��̼� ��� ���
        animator.SetBool("isIdle", true);

        StartCoroutine(WalkCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < rage)
        {
            animator.SetBool("isIdle", false);  // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }
    }

    // ����ǰ� ��ȸ����� �ڷ�ƾ
    IEnumerator WalkCoroutine()
    {
        yield return new WaitForSeconds(3f);
        curState = State.Walking;
        animator.SetBool("isIdle", false);
    }

    public void Running()
    {
        // ���� �ִϸ��̼� ����
        animator.SetBool("isRunning", true);


        StopAllCoroutines();

        // Ÿ��(�÷��̾�)�� ���ؼ� �̵�
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);

        // ���ݹ��� ���� ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRage)
        {
            animator.SetBool("isRunning", false);
            curState = State.Attack;
        }

        // ���� ���� ���� �÷��̾ �Ѿ ���
        else if (Vector3.Distance(transform.position, player.transform.position) >= rage)
        {
            animator.SetBool("isRunning", false);  // �ִϸ��̼� ���
            curState = State.Idle;   // �������·� ��ȯ
        }
    }

    // �̵��� ������������ ���ư��� Return
    public void Return()
    {
        // �ǵ��ư��� ���� = �ȴ� ���
        animator.SetBool("isWalking", true);
        // ������������ �ٽ� ���ư�
        transform.position = Vector3.MoveTowards(transform.position, spawnPoint, walkSpeed * Time.deltaTime);

        // �ø� �ݺ��Ǵ� �κ� �ѹ��� �����ϵ��� ����
        StopAllCoroutines();


        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < rage)
        {
            Flip();
            animator.SetBool("isWalking", false);  // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }

        // ��������Ʈ�� �������� ���
        else if (transform.position == spawnPoint)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            curState = State.Idle;
            Flip();
        }
    }

    public void Attack()
    {
        // ���� �ִϸ��̼�
        animator.SetBool("isAttacking", true);

        // ���ݽ� �÷��̾� ���ݿ� ���� ������� ���� ���� �ʿ�
        // (�ӽ�) ������ �÷��̾��� ü���� �����ͼ� ��� ���

        // ���Ÿ� �� ���
        // �ѹ��� ������Ѿ��ϴµ� ���ݻ����϶� ��� �ݺ�(�����ʿ�)
        if (attackType == true)
        {
            StartCoroutine(ShootCoroutine());
        }

        animator.SetBool("isAttacking", false);
        curState = State.Running;
        // ���ݹ��� ����� ���
        if (Vector3.Distance(transform.position, player.transform.position) >= attackRage)
        {
        }
    }

    // ���Ÿ� ���ݿ� �ڷ�ƾ �ۼ�?
    // n�� �ڿ� ������ �ѹ� ����?
    IEnumerator ShootCoroutine()
    {
        Debug.Log("�ڷ�ƾ ����");

        // ���� ���� ���� �ӽ� ����
        yield return new WaitForSeconds(3f);

        GameObject bullet = Instantiate(bulletPrefab, transform.forward, transform.rotation);
        Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
        rigidbody.velocity = bullet.transform.forward * attackSpeed;
    }

    public void Dead()
    {
        // ��� �ִϸ��̼�
        animator.SetBool("isDead", true);
        animator.SetBool("isDead", false);
    }

    public void Skill()
    {
        // ��ų�� �ִ� ������ ���
    }

    public void Walking()
    {
        // �ȱ� �ִϸ��̼� 
        animator.SetBool("isWalking", true);

        // ������ �̵�?
        transform.position += Vector3.left * walkSpeed * Time.deltaTime;

        // �ȱ� �ڷ�ƾ ���� & �ǵ��ư��� �ڷ�ƾ ����
        StopCoroutine(WalkCoroutine());
        StartCoroutine(returnCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < rage)
        {
           
            animator.SetBool("isIdle", false);  // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }
    }

    // ����ǰ� ��ȸ����� �ڷ�ƾ
    IEnumerator returnCoroutine()
    {
        yield return new WaitForSeconds(3f);
        curState = State.Return;
        animator.SetBool("isWalking", false);
        Flip();
    }

    // �ǰݽ� ����� �Լ�
    public void IsHit(float damage)
    {
        // �ǰ� �ִϸ��̼� ���
        animator.SetBool("isHit", true);
        animator.SetBool("isHit", false);

        // Hp ����
        curHp -= damage;

        // �׾��� ���
        if (curHp <= 0)
        {
            curState = State.Dead;
        }

    }

    // �浹 ����
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� �߻�ü�� ���(�÷��̾��� ������ ���)
        if (collision.gameObject.tag == "attack")   // ���Ƿ� ���س��� ����
        {
            curState = State.IsHit;
        }
    }

    // ȸ��
    public void Flip()
    {
        Debug.Log("�ø�");
        // ���� �������� �̵��� ���� ȸ��
        if (transform.rotation.eulerAngles.y <= -80)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
        }
        else if (transform.rotation.eulerAngles.y >= 80)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
        }
    }

}

