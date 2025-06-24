using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour,ISaveManager
{
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [Space]
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;
    
    [Header("弹出式UI")]
    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;
    public UI_AssignSkillSlot assignSkillSlot; // 技能分配UI

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private bool inventoryIsOpen = false;

    private void Awake()
    {
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        SwitchTo(inGameUI);
        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
        
        // 确保技能分配UI初始时隐藏
        if(assignSkillSlot != null)
        {
            assignSkillSlot.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            if(inventoryIsOpen)
            {
                SwitchWithKeyTo(inGameUI);
                inventoryIsOpen = false;
            }
            else
            {
                SwitchWithKeyTo(characterUI);
                inventoryIsOpen = true;
            }
        }
    }

    /// <summary>
    /// 显示技能分配界面
    /// </summary>
    /// <param name="skill">要分配的技能</param>
    /// <param name="position">显示位置</param>
    public void ShowAssignSkillSlot(BaseSkill skill, Vector2 position)
    {
        if(assignSkillSlot != null)
        {
            assignSkillSlot.ShowSlotSelection(skill,position);
        }
        else
        {
            Debug.LogError("UI_AssignSkillSlot 组件未设置！");
        }
    }

    /// <summary>
    /// 显示技能分配界面（重载方法，使用默认位置）
    /// </summary>
    /// <param name="skill">要分配的技能</param>
    public void ShowAssignSkillSlot(BaseSkill skill)
    {
        Vector2 centerPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        ShowAssignSkillSlot(skill, centerPosition);
    }

    /// <summary>
    /// 隐藏技能分配界面
    /// </summary>
    public void HideAssignSkillSlot()
    {
        if(assignSkillSlot != null)
        {
            assignSkillSlot.HideSlotSelection();
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;
            if(!isFadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }
        if(_menu != null)
        {
            _menu.SetActive(true);
        }
        if(GameManager.Instance!=null)
        {
            if(_menu == inGameUI)
            {
                GameManager.Instance.ResumeGame();
            }
            else
            {
                GameManager.Instance.PauseGame();
            }
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
        }
        else
        {
            AudioManager.instance.PlaySFX(4);
            SwitchTo(_menu);
        }
    }

    private void CheckForInGameUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf&&transform.GetChild(i).GetComponent<UI_FadeScreen>()==null)
            {
                return;
            }
        }
        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeIn();
        StartCoroutine(EndScreenCoroutine());
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1f);
        endText.SetActive(true);
    }

    //死亡后转换到realworld
    

    public void RestartGameButton()
    {
        // 播放重启游戏按钮音效
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(4, null);
            
        GameManager.Instance.RestartScene();
    }

    public void LoadData(GameData _data)
    {
        foreach(var pair in _data.volumeSettings)
        {
            foreach(var volume in volumeSettings)
            {
                if(volume.parameter == pair.Key)
                {
                    volume.LoadSlider(pair.Value);
                    break;
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();
        foreach(var volume in volumeSettings)
        {
            _data.volumeSettings[volume.parameter] = volume.slider.value;
        }
    }
}