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


    // ���� ���� UI ���
    [Header("MagicShop UI")]
    public GameObject magicShopScreen;

    // �������ͽ� ���׷��̵� ���� UI ���
    [Header("StatusUpgradeShop UI")]
    public GameObject statusUpgradeShopScreen;

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

    // ���� ���� ����
    public void EnterMagicShop()
    {
        if(lobbyScreen != null)
        {
            lobbyScreen.SetActive(false);
        }

        if(magicShopScreen != null)
        {
            magicShopScreen.SetActive(true);
        }
    }

    // ���� ���� �ݱ�
    public void ExitMagicShop()
    {
        if (lobbyScreen != null)
        {
            lobbyScreen.SetActive(true);
        }

        if (magicShopScreen != null)
        {
            magicShopScreen.SetActive(false);
        }
    }

    // �������ͽ� ���� ����
    public void EnterStatusShop()
    {
        if (lobbyScreen != null)
        {
            lobbyScreen.SetActive(false);
        }

        if (magicShopScreen != null)
        {
            statusUpgradeShopScreen.SetActive(true);
        }
    }

    // �������ͽ� ���� �ݱ�
    public void ExitStatusShop()
    {
        if (lobbyScreen != null)
        {
            lobbyScreen.SetActive(true);
        }

        if (magicShopScreen != null)
        {
            statusUpgradeShopScreen.SetActive(false);
        }
    }

    public void EnterBattleScene()
    {

    }

    private void OpenOptionWindow()
    {

    }

    private void CloseOptionWindow()
    {

    }
}
