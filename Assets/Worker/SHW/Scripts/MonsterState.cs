using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MonsterState : MonoBehaviour
{
    // ���, ����, ����, ���, �ǵ��ư���, ��ų, �̵�, �ǰ�
    public enum State { Idle, Running, Attack, Return, Skill, Walking, Dead, Stun }

    [Header("Setting")]
    [SerializeField] Player_Controller player;    // ������ �÷��̾�
    [SerializeField] GameObject bulletPrefab;     // ���Ÿ� ���ݽ� �߻��� ������
    [SerializeField] Transform shootPoint;        // �߻� ����Ʈ
    [SerializeField] Animator animator;           // ����� ���ϸ�����
    [SerializeField] AttackTrigger trigger;       // ���� ���� Ȯ�� Ʈ����
    [SerializeField] Collider Collider;           // ������ �ǰ� ������ �ݶ��̴�
    [SerializeField] GameObject hpBarPrefab;
    Slider hpBar;
    Transform hpBarTransform;
    [SerializeField] private GameObject[] monsterPrefabs;    // ��ȯ�� ���� ������

    [Header("Boss1")]
    [SerializeField] float healAmount;      // ȸ����
    [SerializeField] float healRange;       // ȸ�� ����                                 

    [Header("Boss2")]
    [SerializeField] float buffDuration; // ���� ���� �ð�
    [SerializeField] float attackBuffMultiplier; // ���ݷ� 20% ����
    [SerializeField] float defenseBuffMultiplier; // ���� 20% ����

    [Header("Boss3")]
    [SerializeField] float absorbRadius = 10f; // ��� ���� (��: �ݰ� 10m)
    [SerializeField] float absorbAmount = 20f; // ���� �� ������ ����ϴ� ü�·�


    [Header("State")]
    [SerializeField] State curState;         // ������
    public Vector3 spawnPoint;               // �⺻ ��ġ(�ӽ�)
    public Vector3 WalkRangePoint;  // �̵� ��ġ
    public Vector3 destination;

    public int id;
    public float attack;           // ���ݷ�
    public float def;              // ����
    public float hp;               // ü��
    public float curHp;           // ���� ���� ü��
    public float walkSpeed;        // �ȱ��̼�
    public float runSpeed;         // �ٱ��̼�
    public float attackSpeed;      // �̼�
    public float range;             // �����Ÿ�
    public float attackRage;       // ���� ��Ÿ�
    public bool canSkill;          // ��ų����
    public bool attackType;        // ���� Ÿ�� true�� ��� ���Ÿ�
    public float bulletSpeed;      // ����ü �߻� �ӵ�
    public float skillCoolTime;    // ��ų ��Ÿ��

    bool canAttack = true;      // ���� Ȯ��
    float attackTimer;          // ���� Ÿ�̸�

    public bool isDeathWorm; // ������ Ȯ��
    public bool isBoss;  // ���� Ȯ��

    public bool isStun = false;
    float stunTimer = 0;

    public bool skillCoolDown = true;



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

        if (id == 14)
        {
            isDeathWorm = true;
        }
        if (id >= 15)
        {
            isBoss = true;
        }

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

        GameObject hpBarInstance = Instantiate(hpBarPrefab, transform.position, Quaternion.identity);
        hpBarInstance.transform.SetParent(GameObject.Find("WorldCanvas").transform);
        hpBar = hpBarInstance.GetComponent<Slider>();

        hpBarTransform = hpBarInstance.transform;
        UpdateHPBar();
    }

    private void OnDisable()
    {

    }

    // ������ �ҷ����� �Լ�
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
        skillCoolTime = _monsterData.SkillCool;

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
            case State.Dead:  // (�ӽ�) ���
                Dead();
                break;
            case State.Stun:
                Stun();
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

        hpBarTransform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * -1.5f);
    }

    // �⺻ ������
    public void Idle()
    {
        AllAnimationOff();

        // ��� �ִϸ��̼� ��� ���
        animator.SetBool("isIdle", true);

        // ������ ���� �ȱ� ���� ����
        if (isDeathWorm == false)
        {
            StartCoroutine(WalkCoroutine());
        }

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            StopAllCoroutines();
            curState = State.Running;   // �������·� ��ȯ
        }
    }

    // ����ǰ� ��ȸ����� �ڷ�ƾ
    IEnumerator WalkCoroutine()
    {
        yield return new WaitForSeconds(3f);
        curState = State.Walking;
        AllAnimationOff();

        // �̰� ������..?
        // ���� ���·� �Ѿ�� �ٸ� �ڷ�ƾ ���� ����..?
        StopAllCoroutines();
    }

    // ����
    public void Running()
    {
        AllAnimationOff();

        Flip(player.transform.position);
        Vector3 towardVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        // ������
        if (isDeathWorm == true)
        {
            animator.SetBool("isDisappear", true);
            // ���� �ִϸ��̼� ����
            animator.SetBool("isRunning", true);

            // ���ִ� ���� �ǰ� ���� ������
            Collider.enabled = false;
        }
        // �Ϲ� ��
        else
        {
            // ���� �ִϸ��̼� ����
            animator.SetBool("isRunning", true);

            // Ÿ��(�÷��̾�)�� ���ؼ� �̵�
            // �÷��̾��� x�� �� �޴� ���͸� �����
            transform.position = Vector3.MoveTowards(transform.position, towardVector, runSpeed * Time.deltaTime);
        }

        // Debug.Log($"{Vector3.Distance(transform.position, player.transform.position)}");

        // ���ݹ��� ���� ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < attackRage)
        {

            curState = State.Attack;
        }

        // �������� ������ Ȥ�� else if ���� �����ΰ�? �; �ϴ� if������ ��ȯ
        // ���� ���� ���� �÷��̾ �Ѿ ���
        if (Vector3.Distance(transform.position, player.transform.position) > range)
        {
            curState = State.Idle;   // ������������ ���ư���

            if (isDeathWorm == true)
            {
                curState = State.Idle;
            }
        }
    }

    // �̵��� ������������ ���ư��� Return
    // �����ʿ� =  ���� ��ȭ�� ���� 
    public void Return()
    {
        AllAnimationOff();

        Flip(destination);

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            curState = State.Running;   // �������·� ��ȯ
        }

        // ��������Ʈ�� �������� ���
        else if (transform.position.x == spawnPoint.x)
        {
            curState = State.Idle;
        }
    }

    // ����
    public void Attack()
    {
        AllAnimationOff();

        if (canAttack == true)
        {
            // ��ų �ߵ� ���� Ȯ��
            if (canSkill == true)
            {
                Skill();
                return;
            }

            if (isDeathWorm == true)
            {
                animator.SetBool("isAppear", true);
                // animator.SetBool("isIdle", true);

                Collider.enabled = true;

                AllAnimationOff();
            }

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
            curState = State.Running;
        }
    }

    // ���
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
            Destroy(hpBar.gameObject);
            Destroy(gameObject, 3f);
        }
    }

    // ��ų
    public void Skill()
    {
        AllAnimationOff();

        animator.SetBool("SkillReady", true);

        if (id == 1)      // �����
        {
            RushSkill();
        }
        if(id==9)
        {
            Harden();
        }
        if (id >= 15)        // ���� ������ ��ȯ ��ų
        {
            SummonMonster();
        }
        if(id == 15)
        {
            MonsterHill();
        }
        if (id == 16)
        {
            MonsterBurserKer();
        }
        if (id == 17)
        {
            MonsterAbsorb();
        }
    }

    // ��ȸ
    public void Walking()
    {
        AllAnimationOff();

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

        UpdateHPBar();

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

    // ���� ����
    public void Stun()
    {

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }
        else
        {
            curState = State.Idle;
        }
    }

    // ���� �����Լ�
    public void Stunned(float second)
    {
        // ���� ������µ� ���ϸ��̼� ����
        AllAnimationOff();

        stunTimer = second;
        curState = State.Stun;

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

    private void UpdateHPBar()
    {
        if (hpBar != null)
        {
            hpBar.value = (float)curHp / hp;
        }
    }

    #region ��ų ����

    public void RushSkill()
    {
        StartCoroutine(RushCoroutine());
    }

    private IEnumerator RushCoroutine()
    {
        AllAnimationOff();

        Vector3 rushDirection = (player.transform.position - transform.position).normalized;
        float rushDistance = 5f;  // ���� �Ÿ� (5m)
        float rushSpeed = runSpeed * 2.5f;  // ���� �ӵ� (���� �ӵ��� 2.5��)
        Vector3 rushStartPos = transform.position;

        animator.SetBool("isUsingSkill", true);

        while (Vector3.Distance(rushStartPos, transform.position) < rushDistance)
        {
            transform.position += rushDirection * rushSpeed * Time.deltaTime;
            yield return null;
        }

        // ���� ���� �� �÷��̾� ����
        trigger.TirggerOnOff();

        animator.SetBool("isUsingSkill", false);

        StartCoroutine(SkillCoolDown());

        // ���� �� ���¸� �⺻ ���·� ����
        curState = State.Idle;

    }

    // ��ũ���� 
    public void IceBall()
    {
        // �������� ���̽��� 
    }

    // �� ��ų = �ܴ�������
    public void Harden()
    {
        if (skillCoolDown == false) { return; }

        int hardenStack = 0;
        int maxStack = 5;
        float StackDuration = 35;
        float amountIncrease = 0.1f;

        if (hardenStack < maxStack)
        {
            // �����ϴ��� ������? ���� ���� ��Ȳ
            float defIncrease = def * amountIncrease;
            def += defIncrease;
            hardenStack++;
        }

        StartCoroutine(SkillCoolDown());
    }

    // ���̷��� �ּ���
    public void PlayerSlow()
    {
        // �÷��̾� ���ο�
    }

    // ����1 ���� ��
    public void MonsterHill()
    {
        if (skillCoolDown == false) { return; }

        // ���� ���� ���� ã��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, healRange);

        foreach (var hitCollider in hitColliders)
        {
            MonsterState monster = hitCollider.GetComponent<MonsterState>();

            // ������ �ƴ� ���͵鸸 ȸ�� (�ڱ� �ڽ� ����)
            if (monster != null && monster != this)
            {
                monster.curHp = Mathf.Min(monster.curHp + healAmount, monster.hp); // �ִ� ü���� ���� �ʵ��� ȸ��
                monster.UpdateHPBar();  // ü�¹� ������Ʈ
            }
        }

        StartCoroutine(SkillCoolDown());
    }

    // ����2 ���� ����ȭ
    public void MonsterBurserKer()
    {
        if (skillCoolDown == false) { return; }

        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, 10f); // �ݰ� 10m ���� ���� Ž��

        foreach (Collider collider in nearbyMonsters)
        {
            MonsterState monster = collider.GetComponent<MonsterState>();

            if (monster != null && monster != this) // �ڽ��� ������ ���͸� ����
            {
                StartCoroutine(ApplyBuff(monster, attackBuffMultiplier, defenseBuffMultiplier, buffDuration));
            }
        }

        StartCoroutine(SkillCoolDown());
    }

    IEnumerator ApplyBuff(MonsterState monster, float attackMultiplier, float defenseMultiplier, float duration)
    {
        float originalAttack = monster.attack;
        float originalDefense = monster.def;

        // ���ݷ°� ���� ����
        monster.attack *= attackMultiplier;
        monster.def *= defenseMultiplier;

        yield return new WaitForSeconds(duration);

        // ���� ���·� ����
        monster.attack = originalAttack;
        monster.def = originalDefense;
    }

    // ����3 ���� ����
    public void MonsterAbsorb()
    {
        // ��ų ��Ÿ�� ��
        if (skillCoolDown == false) { return; }

        float maxHealth = hp; // ���� ������ �ִ� ü��

        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, absorbRadius);

        foreach (Collider collider in nearbyMonsters)
        {
            MonsterState monster = collider.GetComponent<MonsterState>();

            if (monster != null && monster != this && monster.curHp > 0) // �ڽ��� �����ϰ� ü���� �ִ� ���͸�
            {
                float actualAbsorb = Mathf.Min(absorbAmount, monster.curHp); // ������ ���� ü���� �ʰ����� �ʰ� ���

                // ������ ü���� ���ҽ�Ű��, ������ ü���� ȸ��
                monster.curHp -= actualAbsorb;
                curHp = Mathf.Min(curHp + actualAbsorb, maxHealth); // ������ ü���� �ִ� ü���� ���� �ʵ��� ����

                // ü�� UI ������Ʈ
                monster.UpdateHPBar();
                UpdateHPBar();
            }
        }

        StartCoroutine(SkillCoolDown());
    }

    // �������� ���� ��ȯ
    public void SummonMonster()
    {
        // ��ų ��Ÿ�� ��
        if (skillCoolDown == false) { return; }

        int monstersToSummon = 3;   // ��ȯ�� ���� ��
        float spawnOffset = 2f;  // ���� ������ ��ȯ �Ÿ�

        // 3���� ���� ��ȯ
        for (int i = 0; i < monstersToSummon; i++)
        {
            // ������ ���� ��ȯ ��ġ ���
            Vector3 spawnPosition = transform.position + transform.forward * spawnOffset
                                  + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            // ���� ���� Ÿ�� ����
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            GameObject monsterPrefab = monsterPrefabs[randomIndex];

            // ���� ��ȯ
            Quaternion spawnRotation = Quaternion.Euler(0, -90, 0); // y�� �������� 90�� ȸ�� // ��ȯ ���� 
            Instantiate(monsterPrefab, spawnPosition, spawnRotation);
        }

        StartCoroutine(SkillCoolDown());
    }

    IEnumerator SkillCoolDown()
    {
        skillCoolDown = false;
        yield return new WaitForSeconds(skillCoolTime);
        skillCoolDown = true;
    }

    #endregion
}

