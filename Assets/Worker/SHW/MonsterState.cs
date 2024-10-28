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
    [SerializeField] bool attackType;  // ���� Ÿ�� true�� ��� ���Ÿ�

    // �ӽ� ����
    [Header("Test")]
    [SerializeField] float damage;  // ü�°��ҿ� ���� ������(�ӽ�)

    private void Awake()
    {
        // �ִϸ����� �������
        animator = GetComponent<Animator>();

        // ���� ����Ʈ ����
        spawnPoint = transform.position;

        // �÷��̾� Ȯ��(�ϴ� �̸����� �÷��̾� ������Ʈ ã�´�)
        player = GameObject.Find("Player");
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
            case State.IsHit:   // �ǰ�
                IsHit();
                return;
            case State.Dead:  // (�ӽ�) ���
                Dead();
                return;
        }

        // ���������� ������ ��� 
        // ���������� �������� ���

        // �׾��� ���
        if (hp < 0)
        {
            curState = State.Dead;
        }

        // (TODO)���ϻ����� ��� �ҷ��� �Լ� �ۼ�
    }

    public void Idle()
    {
        // ��� �ִϸ��̼� ��� ���
        animator.SetBool("isIdle", true);

        StartCoroutine(WalkCoroutine());

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) <= rage)
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
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

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
        transform.position = Vector3.MoveTowards(transform.position, spawnPoint, speed * Time.deltaTime);

        // ���� �������� �̵��� ���� ȸ��
        // ���Ͱ� ������ �ٶ󺸴� ������ �������� ������
        // �������� �ٶ� ��� �����߻�..
        if (transform.rotation.eulerAngles.y >= 260)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
        }

        // ���� ���� ���� �÷��̾ ������ ���
        if (Vector3.Distance(transform.position, player.transform.position) < rage)
        {
            animator.SetBool("isWalking", false);  // �ִϸ��̼� ���
            curState = State.Running;   // �������·� ��ȯ
        }

        // ��������Ʈ�� �������� ���
        else if (transform.position == spawnPoint)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            curState = State.Idle;
            transform.rotation = transform.rotation * Quaternion.Euler(0, -180f, 0);
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
            GameObject bullet = Instantiate(bulletPrefab, transform.forward, transform.rotation);
            Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
            rigidbody.velocity = bullet.transform.forward * attackSpeed;
        }

        // ���ݹ��� ����� ���
        if (Vector3.Distance(transform.position, player.transform.position) >= attackRage)
        {
            animator.SetBool("isAttacking", false);
            curState = State.Running;
        }
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
        transform.position += Vector3.left * speed * Time.deltaTime;

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
    }

    // �ǰݽ� ����� �Լ�
    public void IsHit()
    {
        // �ǰ� �ִϸ��̼� ���
        animator.SetBool("isHit", true);
        animator.SetBool("isHit", false);

        // Hp ����
        hp -= damage;
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

}

