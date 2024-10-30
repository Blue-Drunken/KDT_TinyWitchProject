using System.Collections;
using UnityEngine;

public class MonsterState : MonoBehaviour
{
    // ���, ����, ����, ���, �ǵ��ư���, ��ų, �̵�, �ǰ�
    public enum State { Idle, Running, Attack, Return, Skill, Walking, Dead, IsHit }

    // ������
    [SerializeField] State curState;
    // ������ �÷��̾�(�ӽ�)
    [SerializeField] Player_Controller player;
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

    bool canAttack = true;
    float attackTimer;

    // ���Ȯ�ο�
    bool isdead = false;

    [SerializeField] AttackTrigger trigger;

    [SerializeField] float bulletSpeed;

    protected MonsterData _monsterData;
    public MonsterData MonsterData { get { return _monsterData; } }

    private void Awake()
    {
        // �ִϸ����� �������
        animator = GetComponent<Animator>();

        // ���� ����Ʈ ����
        spawnPoint = transform.position;

        // �÷��̾� Ȯ��(�ϴ� �̸����� �÷��̾� ������Ʈ ã�´�)
        //  player = GameObject.Find("testPlayer");

        // ���� ü�� = ����ü������ ����
        curHp = hp;


    }
    private void Start()
    {
        // LoadMonsterData(id);
        DataManager.Instance.OnLoadCompleted += Test;

        player = GameManager.Instance.player;
    }

    private void OnDisable()
    {
        DataManager.Instance.OnLoadCompleted -= Test;
    }

    public void Test()
    {
        LoadMonsterData(id);
    }
    public void LoadMonsterData(int id)
    {
        // id ����
        // id�� �����͸������� ��ųʸ� ���� value���� �����´�

        // ���� Ȯ�ο�
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

    // ���Ÿ� ���ݿ� �ڷ�ƾ �ۼ�?
    // n�� �ڿ� ������ �ѹ� ����?

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
            animator.SetBool("isDead", false);
            Destroy(gameObject,3f);
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

        if (transform.rotation.eulerAngles.y >= -80)
        {
            transform.position += Vector3.left * walkSpeed * Time.deltaTime;
        }
        else if (transform.rotation.eulerAngles.y <= 80)
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

        // ���� ������ �¾��� ���
        // �����Լ� ����

    }

    // �浹 ����
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"���� �浹 : {collision.gameObject.name}");
        if (collision.gameObject == GameManager.Instance.player.gameObject)
        {
            GameManager.Instance.player.stats.TakeDamage(attack);
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
        // ���� ������µ� ���ϸ��̼� ����
        AllAnimationOff();

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

