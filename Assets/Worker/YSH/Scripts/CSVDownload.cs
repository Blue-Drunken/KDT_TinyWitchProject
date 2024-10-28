using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class CSVDownload
{
    const string skillDataUrl = "https://docs.google.com/spreadsheets/d/1KKp21OkOGInFUfQvvE-ZlDCtzdnPYfzevhZbfoynPkQ/export?gid=367973711&format=csv";

    public static IEnumerator SkillDataDownloadRoutine()
    {
        // Web�� ��û�� ������ ���� UnityWebRequest ��ü
        // urlPath�� ���� ������Ʈ�� ��û
        UnityWebRequest skillDataRequest = UnityWebRequest.Get(skillDataUrl);

        // ��û�� �Ϸ�� �� ���� ��� (���� �ٿ�ε�)
        yield return skillDataRequest.SendWebRequest();

        // �ٿ�ε尡 �Ϸ�� ��Ȳ
        string skillTableText = skillDataRequest.downloadHandler.text;
        if (skillTableText == null)
        {
            Debug.LogError("Skill Data Download Error!");
            yield break;
        }

        Debug.Log("Skill Data Download OK");
        yield return skillTableText;
    }
}
