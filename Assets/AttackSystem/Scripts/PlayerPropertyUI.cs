using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
{
    public Sprite playerHeadIcon;
    public Image healthSlider;
    public Image expSlider;
    public Image magicSlider;
    public CharacterSO playerSO;
    public Player player;

    private Entity playerEntity;
    private CharacterStats playerStats;
    private void Start()
    {
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        playerStats = playerEntity.GetComponent<CharacterStats>();
        playerStats.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }

    private void OnDisable()
    {
        playerStats.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        healthSlider.fillAmount = (float)playerStats.GetCurrentHP() / playerStats.GetMaxHP();
    }



    public void ChangePlayerHeadIcon(Sprite changeIcon)
    {
        playerHeadIcon = changeIcon;
    }
    public void UpdatePlayerPropertyUI()
    {
        healthSlider.fillAmount = playerStats.GetCurrentHP() / playerStats.GetMaxHP();
        expSlider.fillAmount = playerSO.exp.value / 20 + Mathf.Pow(playerSO.level.value, 2.25f + Mathf.Log10(playerSO.level.value));
        //magicSlider.fillAmount = playerSO.currentMagic.value / playerSO.maxMagic.value;
    }
}
