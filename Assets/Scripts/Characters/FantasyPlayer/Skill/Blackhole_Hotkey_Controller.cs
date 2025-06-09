using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI hotkeyText;

    private Transform enemiesTransform;
    private Blackhole_Skill_Controller myBlackHole;

    public void SetupHotKey(KeyCode _hotkey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        hotkeyText = GetComponentInChildren<TextMeshProUGUI>();
        myHotKey = _hotkey;
        hotkeyText.text = myHotKey.ToString();
        enemiesTransform = _myEnemy;
        myBlackHole = _myBlackHole;
    }

    private void Update()
    {
        if(Input.GetKeyDown(myHotKey))
        {
            myBlackHole.AddEnemyToList(enemiesTransform);
            hotkeyText.color = Color.clear;
            sr.color = Color.clear;
        }
    }    
}

