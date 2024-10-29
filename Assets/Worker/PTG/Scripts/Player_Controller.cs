using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 direction;
    public float speed = 8;
    public float jumpForce = 10;
    public float gravity = -20;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public bool AbleDoubleJump = true;

    public Animator animator;
    public Transform model;

    //�׽�Ʈ�ڵ�
    //public PlayerStats stats = new PlayerStats();

    //private void Start()
    //{
    //    Debug.Log("�ʱ� ü��: " + stats.currentHealth);
    //}
    //�׽�Ʈ�ڵ�

    void Update()
    {
        //�׽�Ʈ�ڵ�
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    stats.TakeDamage(20f);
        //    Debug.Log("���� ü��: " + stats.currentHealth);
        //}
        //�׽�Ʈ�ڵ�

        //�¿� �Է�
        float hInput = Input.GetAxis("Horizontal");
        direction.x = hInput * speed;
        animator.SetFloat("speed", Mathf.Abs(hInput));

        //���� �ִ��� Ȯ��
        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.X))
        {
            Atk();
        }

        if (isGrounded)
        {
            direction.y = -1;
            AbleDoubleJump = true;

            if (Input.GetButton("Jump"))
            {
                Jump();
            }
        }
        else
        {
            direction.y += gravity * Time.deltaTime; //�߷� 
            if (AbleDoubleJump && Input.GetButtonDown("Jump"))
            {
                DoubleJump();
            }
        }

        //ĳ���� �¿����
        if (hInput != 0)
        {
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(hInput, 0, 0));
            model.rotation = newRotation;
        }

        //ĳ���� ������
        controller.Move(direction * Time.deltaTime);
    }

    private void Jump()
    {
        //����
        direction.y = jumpForce;
    }

    private void DoubleJump()
    {
        //���� ����
        Debug.Log("test");
        direction.y = jumpForce;
        AbleDoubleJump = false;
    }

    private void Atk()
    {
        //����
        animator.SetTrigger("Atk");
    }
}
