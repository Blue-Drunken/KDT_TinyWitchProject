using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class LobbyUI : MonoBehaviour
{
    public GameObject magicShop;
    public GameObject statusUpgradeShop;

    public Button StartButton;

    private void OnEnable()
    {
        LinkButton();
    }

    public void LinkButton()
    {
        if(StartButton != null)
        {
            StartButton.onClick.AddListener(GameManager.Instance.StartGame);
        }
    }

    // ���� ���� ����
    public void EnterMagicShop()
    {
        gameObject?.SetActive(false);
        magicShop?.SetActive(true);
    }

    // ���� ���� �ݱ�
    public void ExitMagicShop()
    {
        gameObject?.SetActive(true);
        magicShop?.SetActive(false);
    }

    // �������ͽ� ���� ����
    public void EnterStatusShop()
    {
        gameObject?.SetActive(false);
        statusUpgradeShop?.SetActive(true);
        
    }

    // �������ͽ� ���� �ݱ�
    public void ExitStatusShop()
    {
        gameObject?.SetActive(true);
        statusUpgradeShop?.SetActive(false);
    }

    private void OpenOptionWindow()
    {
        
    }

    private void CloseOptionWindow()
    {

    }
}
