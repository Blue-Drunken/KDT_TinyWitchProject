using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class UIManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static UIManager Instance;

    // �κ� UI ���
    [Header("Lobby UI")]
    public GameObject lobbyScreen;
    public Button magicShopButton;
    public Button statusUpgradeShopButton;
    public Button gameStartButton;

    // ���� ���� UI ���
    [Header("MagicShop UI")]
    public GameObject magicShopScreen;
    public Button magicShopExit;
    public Button[] skillButtons; // ���� UI�� �ִ� �� ��ų ��ư �迭
    public int[] skillCosts;      // �� ��ų�� �ر� ���

    // �������ͽ� ���׷��̵� ���� UI ���
    [Header("StatusUpgradeShop UI")]
    public GameObject statusUpgradeShopScreen;
    public Button statusUpgradeShopExit;


    private void Awake()
    {
        //�̱��� ���� ����
        if(Instance == null)
        {
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int skillID = i;
            skillButtons[i].onClick.AddListener(() => UnlockSkillInShop(skillID));
        }
    }

    private void UnlockSkillInShop(int skillID)
    {
        int cost = skillCosts[skillID];

        if (GameManager.Instance.HasEnoughGold(cost))
        {
            GameManager.Instance.SpendGold(cost);
            SkillUnlockManager.Instance.UnlockSkill(skillID);
        }
        else
        {
            Debug.Log("��尡 �����մϴ�.");
        }
    }

    public void LinkButton()
    {
        // ��ư�� �޼��� ����
        if (magicShopButton != null)
            magicShopButton.onClick.AddListener(EnterMagicShop);

        if (statusUpgradeShopButton != null)
            statusUpgradeShopButton.onClick.AddListener(EnterStatusShop);

        if (magicShopExit != null)
            magicShopExit.onClick.AddListener(ExitMagicShop);

        if (statusUpgradeShopExit != null)
            statusUpgradeShopExit.onClick.AddListener(ExitStatusShop);

        if (gameStartButton != null)
            gameStartButton.onClick.AddListener(GameManager.Instance.StartGame);
    }

    // ���� ���� ����
    public void EnterMagicShop()
    {
        lobbyScreen?.SetActive(false);
        magicShopScreen?.SetActive(true);
    }

    // ���� ���� �ݱ�
    public void ExitMagicShop()
    {
        lobbyScreen?.SetActive(true);
        magicShopScreen?.SetActive(false);
    }

    // �������ͽ� ���� ����
    public void EnterStatusShop()
    {
        lobbyScreen?.SetActive(false);
        statusUpgradeShopScreen?.SetActive(true);
        
    }

    // �������ͽ� ���� �ݱ�
    public void ExitStatusShop()
    {
        lobbyScreen?.SetActive(true);
        statusUpgradeShopScreen?.SetActive(false);
    }

    private void OpenOptionWindow()
    {
        
    }

    private void CloseOptionWindow()
    {

    }
}
