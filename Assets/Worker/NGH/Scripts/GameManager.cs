using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager Instance;

    private void Awake()
    {
        //�̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ReturnToLobby();
        }
    }

    // ���� ���� �� Ÿ��Ʋ ȭ�鿡�� ȣ��
    public void StartGame()
    {
        // ���� �ε� ����
        StartCoroutine(LoadMainGame());
    }

    // ���� ���� �񵿱� �ε�
    private IEnumerator LoadMainGame()
    {
        // ���� ���� ���� �񵿱�� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync("BattleTest");

        // �ε� ���� ��Ȳ ������Ʈ
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    // �κ� ȭ������ ��ȯ
    public void ReturnToLobby()
    {
        // �κ� �ε� ����
        StartCoroutine(LoadLobby());
    }

    // �κ� �� �񵿱� �ε�
    private IEnumerator LoadLobby()
    {
        // �κ� ���� �񵿱� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync("LobbyTest");

        // �ε� ���� ��Ȳ ������Ʈ
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
