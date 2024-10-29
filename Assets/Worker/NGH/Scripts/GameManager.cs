using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        UIManager.Instance.lobbyScreen = null;
        UIManager.Instance.magicShopScreen = null;
        UIManager.Instance.statusUpgradeShopScreen = null;
        UIManager.Instance.battleUI = GameObject.FindWithTag("Battle");
    }

    private void InitializeLobbyScene()
    {
        // �ν����� �� �� ������Ʈ ����
        UIManager.Instance.lobbyScreen = GameObject.FindWithTag("Lobby");
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Magicshop"))
            {
                UIManager.Instance.magicShopScreen = obj;
            }
            if (obj.CompareTag("Statusshop"))
            {
                UIManager.Instance.statusUpgradeShopScreen = obj;
            }
        }
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in allButtons)
        {
            if(button.CompareTag("Gamestart"))
            {
                UIManager.Instance.gameStartButton = button;
            }
            if (button.CompareTag("Magicshopenter"))
            {
                UIManager.Instance.magicShopButton = button;
            }
            if(button.CompareTag("Statusshopenter"))
            {
                UIManager.Instance.statusUpgradeShopButton = button;
            }
            if(button.CompareTag("Magicshopexit"))
            {
                UIManager.Instance.magicShopExit = button;
            }
            if (button.CompareTag("Statusshopexit"))
            {
                UIManager.Instance.statusUpgradeShopExit = button;
            }
        }
        UIManager.Instance.LinkButton();
        UIManager.Instance.battleUI = null;
    }
}
