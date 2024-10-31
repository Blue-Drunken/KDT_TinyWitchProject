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

        // ���� ���� �ʿ� (�����Ϳ��� �⺻��ų ã����)
        player.handler.SetBasicSkill(9);

        for(int i = 0; i < playerSkillSlotID.Length; i++)
        {
            if (playerSkillSlotID[i] != null)
            {
                player.handler.EquipSkill((int)playerSkillSlotID[i], (Enums.PlayerSkillSlot)i);
            }
        } 
    }

    public BattleUI battleUI;

    public void SetBattleUI(BattleUI battleUI)
    {
        this.battleUI = battleUI;
    }

    [SerializeField] private int gold;

    public UnityAction OnGoldChanged;

    int?[] playerSkillSlotID = new int?[(int)Enums.PlayerSkillSlot.Length];
    public int?[] PlayerSkillSlotID { get { return playerSkillSlotID; } }

    public int monsterCount = 0;

    GameObject rewardChest;

    public void SetRewardChest(GameObject rewardChest)
    {
        this.rewardChest = rewardChest;
    }

    public void SetMonster(MonsterState monster)
    {
        monsterCount++;
        monster.OnDead += DecreaseMonster;
    }

    public void DecreaseMonster(MonsterState monster)
    {
        monsterCount--;
        Debug.Log($"{monster.gameObject.name} ���");
        monster.OnDead -= DecreaseMonster;
        if(monsterCount == 0 && rewardChest != null)
        {
            rewardChest.SetActive(true);
            rewardChest = null;
        }
    }

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
        // �׽�Ʈ�� ��Ʋ �� ��ȯ
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestGame();
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
        AsyncOperation operation = SceneManager.LoadSceneAsync("Stage1-1");

        // �ε� ���� ��Ȳ ������Ʈ
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    // ���� ���� �� Ÿ��Ʋ ȭ�鿡�� ȣ��
    public void TestGame()
    {
        // ���� �ε� ����
        StartCoroutine(LoadTestGame());
    }

    // ���� ���� �񵿱� �ε�
    private IEnumerator LoadTestGame()
    {
        // ���� ���� ���� �񵿱�� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync("BattleTest 2");

        // �ε� ���� ��Ȳ ������Ʈ
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    // ���� ���� �� Ÿ��Ʋ ȭ�鿡�� ȣ��
    public void LoadScene(string sceneName)
    {
        // ���� �ε� ����
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    // ���� ���� �񵿱� �ε�
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // ���� ���� ���� �񵿱�� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

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

        if(scene.name == "LobbyTest")
        {
            InitializeLobbyScene();
            return;
        }

        // �� �ε��� �ʱ�ȭ
        InitializeBattleScene();
        
    }

    // �� ��ȯ�� ���� �ʱ�ȭ
    private void InitializeBattleScene()
    {
        
    }

    private void InitializeLobbyScene()
    {
        monsterCount = 0;
        playerSkillSlotID = new int?[(int)Enums.PlayerSkillSlot.Length];
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
