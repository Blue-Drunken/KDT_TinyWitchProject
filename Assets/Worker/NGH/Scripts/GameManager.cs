using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager Instance;

    public Player_Controller player;

    public void SetPlayer(Player_Controller player)
    {
        this.player = player;
    }

    [SerializeField] private int gold;

    public UnityAction OnGoldChanged;

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
        // �׽�Ʈ�� �κ� ��ȯ
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ReturnToLobby();
        }
    }

    private void OnEnable()
    {
        // ���� �ε�Ǹ� ȣ��� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // ��Ȱ��ȭ �Ǹ� �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    // ���� �ε������ ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded : " + scene.name);

        // �� �ε��� �ʱ�ȭ
        if(scene.name == "BattleTest")
        {
            InitializeBattleScene();
        }

        if(scene.name == "LobbyTest")
        {
            InitializeLobbyScene();
        }
    }

    // �� ��ȯ�� ���� �ʱ�ȭ
    private void InitializeBattleScene()
    {
        
    }

    private void InitializeLobbyScene()
    {
        
    }

    // ��带 �߰��ϴ� �޼���
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("��� �߰�: " + amount + ", ���� ���: " + gold);

        OnGoldChanged?.Invoke();
    }

    // ��带 �����ϴ� �޼���
    public void SpendGold(int amount)
    {
        if (HasEnoughGold(amount))
        {
            gold -= amount;
            Debug.Log("��� ����: " + amount + ", ���� ���: " + gold);

            OnGoldChanged?.Invoke();
        }
        else
        {
            Debug.Log("��尡 �����մϴ�.");
        }
    }

    // ��尡 ������� Ȯ���ϴ� �޼���
    public bool HasEnoughGold(int amount)
    {
        return gold >= amount;
    }

    // ���� ��� ������ �������� �޼��� (�ʿ� �� �߰�)
    public int GetGold()
    {
        return gold;
    }
}
