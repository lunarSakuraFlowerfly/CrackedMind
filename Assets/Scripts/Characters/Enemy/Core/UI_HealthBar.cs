using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private Slider slider;
    private CharacterStats stats;
    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        stats = GetComponentInParent<CharacterStats>();
        slider = GetComponentInChildren<Slider>();
        entity.onFlipped += FlipUI;
        stats.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI();
    }

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        stats.OnHealthChanged -= UpdateHealthUI;
    }


    private void UpdateHealthUI()
    {

        slider.maxValue = stats.GetMaxHP();
        slider.value = stats.GetCurrentHP();
    }

    private void FlipUI()=>slider.transform.Rotate(180, 0, 0);
    
}
