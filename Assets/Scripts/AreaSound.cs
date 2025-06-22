using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;
    private float defaultVolume;

    private void Start()
    {
        defaultVolume = AudioManager.instance.bgm[areaSoundIndex].volume;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        AudioManager.instance.bgm[areaSoundIndex].volume = defaultVolume;
        if(_collision.GetComponent<Player>()!=null)
        {
            AudioManager.instance.PlayBGM(areaSoundIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if(_collision.GetComponent<Player>()!=null)
        {
            AudioManager.instance.StopBGMWithTime(areaSoundIndex);
        }
    }

}
