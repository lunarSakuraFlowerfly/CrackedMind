using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_StatToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    public void ShowStatToolTip(string _text)
    {
        gameObject.SetActive(true);
        description.text = _text;
    }
    public void HideToolTip()=>gameObject.SetActive(false);
}
