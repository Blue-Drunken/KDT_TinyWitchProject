using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // ����â �г�
    private bool isPaused = false; // ���� ���� ���� Ȯ�ο�

    void Update()
    {
        // ESC Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ���� �簳
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // ����â �����
        Time.timeScale = 1f; // ���� �ð� �簳
        isPaused = false;
    }

    // ���� �Ͻ�����
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // ����â ���̱�
        Time.timeScale = 0f; // ���� �ð� ����
        isPaused = true;
    }

    // ���� ���� �Լ� (�ʿ�� Quit ��ư�� ����)
    public void QuitGame()
    {
        Application.Quit(); // ���� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
