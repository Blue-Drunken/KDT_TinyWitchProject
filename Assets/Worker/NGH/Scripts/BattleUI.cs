using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    // ��Ʋ�� UI ���
    [Header("Skill Icon")]
    public Image[] skillimage = new Image[(int)Enums.PlayerSkillSlot.Length];
    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI goldText;
    public SkillSlotUI skillSlotUI;

    private void Awake()
    {
        skillSlotUI.gameObject.SetActive(false);
        GameManager.Instance.SetBattleUI(this);
    }

    private void Start()
    {
        // hpBar�ʱ�ȭ
        hpBar.minValue = 0;
        hpBar.maxValue = GameManager.Instance.player.stats.maxHealth;

        // �̺�Ʈ ���
        GameManager.Instance.player.handler.OnChangedSkillSlot += UpdateSkill;
        GameManager.Instance.player.stats.OnChangedHP += UpdateHp;
        GameManager.Instance.OnGoldChanged += UpdateGold;

        UpdateGold();
        UpdateHp();
        UpdateSkill();
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        // gold UI TEST Code
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameManager.Instance.AddGold(100);
        }

        // hp UI TEST Code
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameManager.Instance.player.stats.TakeDamage(10);
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"BattleUI �ı��� {gameObject.name}");
        // �̺�Ʈ ����
        GameManager.Instance.player.handler.OnChangedSkillSlot -= UpdateSkill;
        GameManager.Instance.player.stats.OnChangedHP -= UpdateHp;
        GameManager.Instance.OnGoldChanged -= UpdateGold;
    }

    public void ShowSkillSlotUI(int skillID)
    {
        skillSlotUI.SetInfo(skillID);
        skillSlotUI.gameObject.SetActive(true);
    }

    // ������ QWER��ų�� �ϳ��� �ٲ������ �۵��Ǵ� �ڵ�
    public void UpdateSkill()
    {
        for (int i = 0; i < (int)Enums.PlayerSkillSlot.Length; i++)
        {
            // �� ���� ������ null
            if (GameManager.Instance.player.handler.PlayerSkillSlot[i] == null)
            {
                skillimage[i].gameObject.SetActive(false);
            }
            // ��ų�� ���Կ� ������ �� ��ų �����͸� �о ��ų�� �´� ���������� ��ü
            else
            {
                skillimage[i].sprite = GameManager.Instance.player.handler.PlayerSkillSlot[i].SkillData.SkillIcon;
                skillimage[i].gameObject.SetActive(true);
            }
        }
    }

    public void UpdateHp()
    {
        // ���� ü�°� �ִ� ü���� �����ͼ� UI�� ������Ʈ
        float currentHealth = GameManager.Instance.player.stats.currentHealth;
        float maxHealth = GameManager.Instance.player.stats.maxHealth;

        hpBar.value = currentHealth;
        hpText.text = $"{currentHealth} / {maxHealth}";
    }

    public void UpdateGold()
    {
        int gold = GameManager.Instance.GetGold();

        goldText.text = gold.ToString();
    }
}
