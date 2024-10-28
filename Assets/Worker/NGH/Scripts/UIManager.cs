using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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

    // �������ͽ� ���׷��̵� ���� UI ���
    [Header("StatusUpgradeShop UI")]
    public GameObject statusUpgradeShopScreen;
    public Button statusUpgradeShopExit;

    // ��Ʋ�� UI ���
    [Header("Battle UI")]
    public GameObject battleUI;

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
