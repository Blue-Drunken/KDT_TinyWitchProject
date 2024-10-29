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
    // �߻� ����Ʈ
    [SerializeField] Transform shootPoint;

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

        if (curState == State.Attack && attackType == true)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    public void Idle()
    {
        // ��� �ִϸ��̼� ��� ���
        animator.SetBool("isIdle", true);

        StartCoroutine(WalkCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < rage)
        {
            StopAllCoroutines();
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

        StopCoroutine(WalkCoroutine());

        // Ÿ��(�÷��̾�)�� ���ؼ� �̵�
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);

        // ���ݹ��� ���� ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < attackRage)
        {
            animator.SetBool("isRunning", false);
            curState = State.Attack;
        }

        // ���� ���� ���� �÷��̾ �Ѿ ���
        else if (Vector3.Distance(transform.position, player.transform.position) > rage)
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
            Flip();
            animator.SetBool("isWalking", false);
            curState = State.Idle;
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


        // ���ݹ��� ���� ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) > attackRage)
        {
            animator.SetBool("isAttacking", false);
            curState = State.Running;
        }
    }

    // ���Ÿ� ���ݿ� �ڷ�ƾ �ۼ�?
    // n�� �ڿ� ������ �ѹ� ����?

    IEnumerator ShootCoroutine()
    {
        Debug.Log("�ڷ�ƾ ����");

        yield return new WaitForSeconds(2f);

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
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

        if (transform.rotation.eulerAngles.y <= -80)
        {
            transform.position += Vector3.left * walkSpeed * Time.deltaTime;
        }
        else if (transform.rotation.eulerAngles.y >= 80)
        {
            transform.position += Vector3.right * walkSpeed * Time.deltaTime;
        }

        // �ȱ� �ڷ�ƾ ���� & �ǵ��ư��� �ڷ�ƾ ����
        StopCoroutine(WalkCoroutine());
        StartCoroutine(returnCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < rage)
        {

            animator.SetBool("isWalking", false);  // �ִϸ��̼� ���
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

        // ���� ������ �¾��� ���
        // �����Լ� ����

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

    public void Stun()
    {
        animator.SetBool("isStun", true);
        animator.SetBool("isStun", false);
    }

    // ��ȭ 
    public void Delay(float ice)
    {
        // ��ȭ ��ų�� �ɷ��� ��� �̼� ����?
        // ������� ���� ��� �ʿ�
        walkSpeed -= ice;
        runSpeed -= ice;
    }

}

