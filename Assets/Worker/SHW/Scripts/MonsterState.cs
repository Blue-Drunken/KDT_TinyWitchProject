using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonsterState : MonoBehaviour
{
    // ���, ����, ����, ���, �ǵ��ư���, ��ų, �̵�, �ǰ�
    public enum State { Idle, Running, Attack, Return, Skill, Walking, Dead, IsHit }

    [Header("Setting")]
    [SerializeField] Player_Controller player;    // ������ �÷��̾�
    [SerializeField] GameObject bulletPrefab;     // ���Ÿ� ���ݽ� �߻��� ������
    [SerializeField] Transform shootPoint;        // �߻� ����Ʈ
    [SerializeField] Animator animator;           // ����� ���ϸ�����
    [SerializeField] AttackTrigger trigger;       // ���� ���� Ȯ�� Ʈ����


    [Header("State")]
    [SerializeField] State curState;         // ������
    public Vector3 spawnPoint;               // �⺻ ��ġ(�ӽ�)
    public Vector3 WalkRangePoint;  // �̵� ��ġ
    public Vector3 destination;

    [SerializeField] int id;
    [SerializeField] float attack;           // ���ݷ�
    [SerializeField] float def;              // ����
    [SerializeField] float hp;               // ü��
    public float curHp;                      // ���� ���� ü��
    [SerializeField] float walkSpeed;        // �ȱ��̼�
    [SerializeField] float runSpeed;         // �ٱ��̼�
    [SerializeField] float attackSpeed;      // �̼�
    [SerializeField] float range;             // �����Ÿ�
    [SerializeField] float attackRage;       // ���� ��Ÿ�
    [SerializeField] bool canSkill;          // ��ų����
    [SerializeField] bool attackType;        // ���� Ÿ�� true�� ��� ���Ÿ�
    [SerializeField] float bulletSpeed;      // ����ü �߻� �ӵ�

    bool canAttack = true;      // ���� Ȯ��
    float attackTimer;          // ���� Ÿ�̸�



    // ���Ȯ�ο�
    bool isdead = false;

    protected MonsterData _monsterData;
    public MonsterData MonsterData { get { return _monsterData; } }

    public UnityAction<MonsterState> OnDead;

    private void Awake()
    {
        // �ִϸ����� �������
        animator = GetComponent<Animator>();

        LoadMonsterData(id);

        // ���� ����Ʈ ����
        spawnPoint = transform.position;
        // ��ȸ�� �Ÿ� ������ġ +5
        WalkRangePoint = new Vector3(spawnPoint.x - 5, spawnPoint.y, spawnPoint.z);

        GameManager.Instance.SetMonster(this);
    }
    private void Start()
    {
        // LoadMonsterData(id);
        player = GameManager.Instance.player;
    }

    private void OnDisable()
    {

    }

    public void LoadMonsterData(int id)
    {
        // ���� Ȯ�ο�
        // Debug.Log($"��û�� ���� ID: {id}");

        // id�� �ش��ϴ� �����Ͱ� �����ϴ��� Ȯ���ϰ�, �������� ���� ��� ���� ���
        if (DataManager.Instance.MonsterDict.TryGetValue(id, out MonsterData data) == false)
        {
            Debug.LogError($"MonsterData�� ã�� �� �����ϴ�. ID: {id}");
            return;
        }

        // ������ ���� ������ ���� �����Ϳ� �Ҵ��Ѵ�.
        _monsterData = data;

        attack = _monsterData.Attack;
        def = _monsterData.Defense;
        hp = _monsterData.Hp;
        curHp = hp;  // ���� ü�� = ����ü������ ����
        walkSpeed = _monsterData.WalkSpeed;
        runSpeed = _monsterData.RunSpeed;
        attackSpeed = _monsterData.AttackSpeed;
        range = _monsterData.Rage;
        attackRage = _monsterData.AttackRage;
        canSkill = _monsterData.CanSkill;
        attackType = _monsterData.AttackType;

        if (trigger != null)
        {
            trigger.SetDamage(attack);
        }
    }

    private void Update()
    {
        // ���� Ȯ�ο� ����� �α�
        // Debug.Log(curState);

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
                break;
            case State.Attack: // ����
                Attack();
                break;
            case State.Skill: // ��ų
                Skill();
                break;
            case State.Walking: // �̵� = ��ȸ
                Walking();
                break;
            /*
        case State.IsHit:   // �ǰ�
            IsHit();
            return;*/
            case State.Dead:  // (�ӽ�) ���
                Dead();
                break;
        }

        if (canAttack == false)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                canAttack = true;
            }
        }
    }

    public void Idle()
    {

        // ��� �ִϸ��̼� ��� ���
        animator.SetBool("isIdle", true);

        StartCoroutine(WalkCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            StopAllCoroutines();
            AllAnimationOff(); // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }
    }

    // ����ǰ� ��ȸ����� �ڷ�ƾ
    IEnumerator WalkCoroutine()
    {
        yield return new WaitForSeconds(3f);
        curState = State.Walking;
        animator.SetBool("isIdle", false);

        // �̰� ������..?
        // ���� ���·� �Ѿ�� �ٸ� �ڷ�ƾ ���� ����..?
        StopAllCoroutines();
    }

    public void Running()
    {
        // ���� �ִϸ��̼� ����
        animator.SetBool("isRunning", true);

        Flip(player.transform.position);
        Vector3 towardVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        // Ÿ��(�÷��̾�)�� ���ؼ� �̵�
        // �÷��̾��� x�� �� �޴� ���͸� �����
        transform.position = Vector3.MoveTowards(transform.position, towardVector, runSpeed * Time.deltaTime);

        Debug.Log($"{Vector3.Distance(transform.position, player.transform.position) }");

        // ���ݹ��� ���� ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < attackRage)
        {
            AllAnimationOff();
            curState = State.Attack;
        }

        // �������� ������ Ȥ�� else if ���� �����ΰ�? �; �ϴ� if������ ��ȯ
        // ���� ���� ���� �÷��̾ �Ѿ ���
        if (Vector3.Distance(transform.position, player.transform.position) > range)
        {
            AllAnimationOff();
            curState = State.Return;   // ������������ ���ư���
        }
    }

    // �̵��� ������������ ���ư��� Return
    public void Return()
    {
        // �ǵ��ư��� ���� = �ȴ� ���
        animator.SetBool("isWalking", true);

        // ������������ �ٽ� ���ư�
        transform.position = Vector3.MoveTowards(transform.position, spawnPoint, walkSpeed * Time.deltaTime);

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            animator.SetBool("isWalking", false);  // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }

        // ��������Ʈ�� �������� ���
        else if (transform.position.x == spawnPoint.x)
        {
            animator.SetBool("isWalking", false);
            curState = State.Idle;
        }
    }

    public void Attack()
    {
        if (canAttack == true)
        {
            // Debug.Log("���ݼ���");
            // ���� �ִϸ��̼�
            animator.SetBool("isAttacking", true);

            if (attackType == true)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                Bullet instance = bullet.GetComponent<Bullet>();
                instance.SetSpeed(bulletSpeed);
                instance.SetDamage(attack);
            }
            else
            {
                // �÷��̾� ����
                trigger.TirggerOnOff();
            }

            attackTimer = attackSpeed;

            canAttack = false;
        }


        // ���ݹ��� ���� ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) > attackRage)
        {
            animator.SetBool("isAttacking", false);
            curState = State.Running;
        }
    }

    IEnumerator ShootCoroutine()
    {
        Debug.Log("�ڷ�ƾ ����");

        yield return new WaitForSeconds(2f);



    }

    public void Dead()
    {
        // ���� ������µ� ���ϸ��̼� ����
        AllAnimationOff();

        // ��� �ִϸ��̼�
        if (isdead == false)
        {
            animator.SetBool("isDead", true);
            isdead = true;
        }
        else
        {
            OnDead?.Invoke(this);
            // animator.SetBool("isDead", false);
            Destroy(gameObject, 3f);
        }
    }

    public void Skill()
    {
        // ��ų�� �ִ� ������ ���
    }

    public void Walking()
    {
        // �ȱ� �ִϸ��̼� 
        animator.SetBool("isWalking", true);

        if (transform.position.x >= spawnPoint.x)
        {
            destination = WalkRangePoint;
            Flip(destination);
        }
        else if (transform.position.x <= WalkRangePoint.x)
        {
            destination = spawnPoint;
            Flip(destination);

        }

        transform.position = Vector3.MoveTowards(transform.position, destination, walkSpeed * Time.deltaTime);

        StartCoroutine(IdleCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            animator.SetBool("isWalking", false);  // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }
    }

    // ����ǰ� ��ȸ����� �ڷ�ƾ
    IEnumerator IdleCoroutine()
    {
        yield return new WaitForSeconds(5f);
        curState = State.Idle;
        animator.SetBool("isWalking", false);

        StopAllCoroutines();
    }

    // �ǰݽ� ����� �Լ�
    public void IsHit(float damage)
    {
        // ���� ������µ� ���ϸ��̼� ����
        AllAnimationOff();

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

    // �浹 ���� = �÷��̾�� ������ �ִ� �κ�
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"���� �浹 : {collision.gameObject.name}");
        if (collision.gameObject == GameManager.Instance.player.gameObject)
        {
            GameManager.Instance.player.stats.TakeDamage(attack);
        }
    }

    // ȸ��
    public void Flip(Vector3 lookingPos)
    {
        // ������ y���� ���������ǰ� ������ �ٶ󺸴� �ڵ�
        // �÷��̾� �������� �� ������ ���ؼ� �ణ ���� ����?
        if (transform.position.y == lookingPos.y)
        {
            transform.LookAt(lookingPos);
        }
    }

    public void Stun()
    {
        // ���� ������µ� ���ϸ��̼� ����
        AllAnimationOff();

        animator.SetBool("isStun", true);
        animator.SetBool("isStun", false);
    }

    // ��ȭ(�ӽ��ۼ�) 
    public void Slow(float ice)
    {
        // ��ȭ ��ų�� �ɷ��� ��� �̼� ����?
        // ������� ���� ��� �ʿ�
        walkSpeed -= ice;
        runSpeed -= ice;
    }

    // �ִϸ��̼� ���� ���� �Լ�
    public void AllAnimationOff()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isHit", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isStun", false);
    }
}

