using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MonsterState : MonoBehaviour
{
    // 대기, 추적, 공격, 사망, 되돌아가기, 스킬, 이동, 피격
    public enum State { Idle, Running, Attack, Return, Skill, Walking, Dead, Stun }

    [Header("Setting")]
    [SerializeField] Player_Controller player;    // 추적할 플레이어
    [SerializeField] GameObject bulletPrefab;     // 원거리 공격시 발사할 프리팹
    [SerializeField] Transform shootPoint;        // 발사 포인트
    [SerializeField] Animator animator;           // 재생할 에니메이터
    [SerializeField] AttackTrigger trigger;       // 공격 범위 확인 트리거
    [SerializeField] Collider Collider;           // 데스웜 피격 판정용 콜라이더
    [SerializeField] GameObject hpBarPrefab;
    Slider hpBar;
    Transform hpBarTransform;



    [Header("State")]
    [SerializeField] State curState;         // 현상태
    public Vector3 spawnPoint;               // 기본 위치(임시)
    public Vector3 WalkRangePoint;  // 이동 위치
    public Vector3 destination;

    [SerializeField] int id;
    [SerializeField] float attack;           // 공격력
    [SerializeField] float def;              // 방어력
    [SerializeField] float hp;               // 체력
    public float curHp;                      // 실제 현재 체력
    [SerializeField] float walkSpeed;        // 걷기이속
    [SerializeField] float runSpeed;         // 뛰기이속
    [SerializeField] float attackSpeed;      // 이속
    [SerializeField] float range;             // 추적거리
    [SerializeField] float attackRage;       // 공격 사거리
    [SerializeField] bool canSkill;          // 스킬여부
    [SerializeField] bool attackType;        // 공격 타입 true일 경우 원거리
    [SerializeField] float bulletSpeed;      // 투사체 발사 속도

    bool canAttack = true;      // 공격 확인
    float attackTimer;          // 공격 타이머

    public bool isDeathWorm; // 데스웜 확인
    public bool isBoss;  // 보스 확인

    public bool isStun = false;
    float stunTimer = 0;

    // 사망확인용
    bool isdead = false;

    protected MonsterData _monsterData;
    public MonsterData MonsterData { get { return _monsterData; } }

    public UnityAction<MonsterState> OnDead;

    private void Awake()
    {
        // 애니메이터 갖고오기
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

        // 스폰 포인트 저장
        spawnPoint = transform.position;
        // 배회시 거리 기존위치 +5
        WalkRangePoint = new Vector3(spawnPoint.x - 5, spawnPoint.y, spawnPoint.z);

        GameManager.Instance.SetMonster(this);
    }
    private void Start()
    {
        // LoadMonsterData(id);
        player = GameManager.Instance.player;

        //남궁하
        Vector3 hpBarPosition = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z - 1.0f);
        GameObject hpBarInstance = Instantiate(hpBarPrefab, hpBarPosition, Quaternion.identity);
        hpBar = hpBarInstance.GetComponentInChildren<Slider>();
        hpBarInstance.transform.SetParent(gameObject.transform);

        hpBar.minValue = 0;
        hpBar.maxValue = hp;

        hpBarTransform = hpBarInstance.transform;
        UpdateHPBar(); // 남궁하
    }

    private void OnDisable()
    {

    }

    // 데이터 불러오는 함수
    public void LoadMonsterData(int id)
    {
        // 오류 확인용
        // Debug.Log($"요청된 몬스터 ID: {id}");

        // id에 해당하는 데이터가 존재하는지 확인하고, 존재하지 않을 경우 오류 출력
        if (DataManager.Instance.MonsterDict.TryGetValue(id, out MonsterData data) == false)
        {
            Debug.LogError($"MonsterData를 찾을 수 없습니다. ID: {id}");
            return;
        }

        // 가져온 값은 선언한 몬스터 데이터에 할당한다.
        _monsterData = data;

        attack = _monsterData.Attack;
        def = _monsterData.Defense;
        hp = _monsterData.Hp;
        curHp = hp;  // 현재 체력 = 설정체력으로 설정
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
        // 상태 확인용 디버그 로그
        // Debug.Log(curState);

        // 상태패턴
        switch (curState)
        {
            case State.Idle:    // 대기 = 숨쉬기
                Idle();
                break;
            case State.Running:     // 뛰기 = 추적
                Running();
                break;
            case State.Return:  // 리턴 = 되돌아가기
                Return();
                break;
            case State.Attack: // 공격
                Attack();
                break;
            case State.Skill: // 스킬
                Skill();
                break;
            case State.Walking: // 이동 = 배회
                Walking();
                break;
            case State.Dead:  // (임시) 사망
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

        hpBarTransform.position = transform.position + new Vector3(0, -1, -2); // 남궁하
    }

    // 기본 대기상태
    public void Idle()
    {
        AllAnimationOff();

        // 대기 애니메이션 모션 출력
        animator.SetBool("isIdle", true);

        // 데스웜 제외 걷기 상태 변경
        if (isDeathWorm == false)
        {
            StartCoroutine(WalkCoroutine());
        }

        // 일정 범위 내에 플레이어가 들어왔을 경우
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            StopAllCoroutines();
            curState = State.Running;   // 추적상태로 변환
        }
    }

    // 대기모션과 배회모션의 코루틴
    IEnumerator WalkCoroutine()
    {
        yield return new WaitForSeconds(3f);
        curState = State.Walking;
        AllAnimationOff();

        // 이거 괜찮나..?
        // 다음 상태로 넘어갈때 다른 코루틴 전부 종료..?
        StopAllCoroutines();
    }

    // 추적
    public void Running()
    {
        AllAnimationOff();

        Flip(player.transform.position);
        Vector3 towardVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        // 데스웜
        if (isDeathWorm == true)
        {
            animator.SetBool("isDisappear", true);
            
            // 추적 애니메이션 실행
            animator.SetBool("isRunning", true);

            // 들어가있는 동안 피격 판정 없도록
            Collider.enabled = false;
        }
        // 일반 몹
        else
        {
            // 추적 애니메이션 실행
            animator.SetBool("isRunning", true);

            // 타겟(플레이어)를 향해서 이동
            // 플레이어의 x축 만 받는 벡터를 만들것
            transform.position = Vector3.MoveTowards(transform.position, towardVector, runSpeed * Time.deltaTime);
        }

        // 공격범위 내로 들어왔을 경우
        if (Vector3.Distance(transform.position, player.transform.position) < attackRage)
        {

            curState = State.Attack;
        }

        // 추적에서 문제가 혹시 else if 여서 문제인가? 싶어서 일단 if문으로 전환
        // 일정 범위 내에 플레이어가 넘어갈 경우
        if (Vector3.Distance(transform.position, player.transform.position) > range)
        {
            curState = State.Return;   // 스폰지점으로 돌아간다
        }
    }

    // 이동시 스폰지점으로 돌아가는 Return
    public void Return()
    {
        AllAnimationOff();

        // 일정 범위 내에 플레이어가 들어왔을 경우
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            curState = State.Running;   // 추적상태로 변환
        }

        // 스폰포인트에 도착했을 경우
        else if (transform.position.x == spawnPoint.x)
        {
            curState = State.Idle;
        }
    }

    // 공격
    public void Attack()
    {
        AllAnimationOff();
        if (canAttack == true)
        {

            // 스킬 발동 여부 확인
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
            
            // 공격 애니메이션
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
                // 플레이어 공격
                trigger.TirggerOnOff();
            }

            attackTimer = attackSpeed;

            canAttack = false;
        }


        // 공격범위 내로 들어왔을 경우
        if (Vector3.Distance(transform.position, player.transform.position) > attackRage)
        {
            curState = State.Running;
        }
    }

    // 사망
    public void Dead()
    {
        // 이전 어느상태든 에니메이션 끄기
        AllAnimationOff();

        // 사망 애니메이션
        if (isdead == false)
        {
            animator.SetBool("isDead", true);
            isdead = true;
        }
        else
        {
            OnDead?.Invoke(this);
            // animator.SetBool("isDead", false);
            // 남궁하
            if (hpBar != null)
            {
                Destroy(hpBar.gameObject);
            }
            Destroy(gameObject, 3f);
        }
    }

    // 스킬
    public void Skill()
    {
        AllAnimationOff();

        animator.SetBool("SkillReady", true);

        if (id == 1)      // 멧돼지
        {
            RushSkill();
        }
    }

    // 배회
    public void Walking()
    {
        AllAnimationOff();

        // 걷기 애니메이션 
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

        // 일정 범위 내에 플레이어가 들어왔을 경우
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            curState = State.Running;   // 추적상태로 변환
        }
    }

    // 대기모션과 배회모션의 코루틴
    IEnumerator IdleCoroutine()
    {
        yield return new WaitForSeconds(5f);
        curState = State.Idle;
        animator.SetBool("isWalking", false);

        StopAllCoroutines();
    }

    // 피격시 출력할 함수
    public void IsHit(float damage)
    {
        // 이전 어느상태든 에니메이션 끄기
        AllAnimationOff();

        // 피격 애니메이션 출력
        animator.SetBool("isHit", true);
        animator.SetBool("isHit", false);

        // Hp 감소
        curHp -= damage;

        UpdateHPBar();

        // 죽었을 경우
        if (curHp <= 0)
        {
            curState = State.Dead;
        }

    }

    // 충돌 감지 = 플레이어에게 데미지 주는 부분
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"몬스터 충돌 : {collision.gameObject.name}");
        if (collision.gameObject == GameManager.Instance.player.gameObject)
        {
            GameManager.Instance.player.stats.TakeDamage(attack);
        }
    }

    // 회전
    public void Flip(Vector3 lookingPos)
    {
        // 원래는 y값이 기존포지션과 같을때 바라보는 코드
        // 플레이어 점프했을 때 감지를 위해서 약간 범위 수정?
        if (transform.position.y == lookingPos.y)
        {
            transform.LookAt(lookingPos);
        }
    }

    // 스턴 임시작성
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

    public void Stunned(float second)
    {
        // 이전 어느상태든 에니메이션 끄기
        AllAnimationOff();

        stunTimer = second;
        curState = State.Stun;

        animator.SetBool("isStun", true);
        animator.SetBool("isStun", false);


    }

    // 둔화(임시작성) 
    public void Slow(float ice)
    {
        // 둔화 스킬에 걸렸을 경우 이속 감소?
        // 원래대로 돌릴 방법 필요
        walkSpeed -= ice;
        runSpeed -= ice;
    }

    // 애니메이션 전부 끄는 함수
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
    //남궁하
    private void UpdateHPBar()
    {
        if (hpBar != null)
        {
            hpBar.value = curHp;
        }
    }

    #region 스킬 모음

    public void RushSkill()
    {
        // 플레이어 방향으로 돌진
        Vector3 rushDirection = (player.transform.position - transform.position).normalized;

        float rushDistance = 5f;  // 돌진 거리 (5m)
        float rushSpeed = runSpeed * 2.5f;  // 돌진 속도 (기존 속도의 2.5배)

        Vector3 rushStartPos = transform.position;

        transform.position += rushDirection * rushSpeed * Time.deltaTime;

        // 플레이어 공격
        trigger.TirggerOnOff();

        // 공격 후 기본상태로 변경
        curState = State.Idle;
    }

    #endregion
}

