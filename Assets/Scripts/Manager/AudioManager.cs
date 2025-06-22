using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinimumDistance;
    public AudioSource[] sfx;
    public AudioSource[] bgm;

    public bool playBGM;
    private int BGMIndex;

    private bool canPlaySFX;

    private void Update()
    {
        if(!playBGM)
        {
            StopAllBGM();
        }
        else
        {
            if(!bgm[BGMIndex].isPlaying)
            {
                PlayBGM(BGMIndex);
            }
        }
    }

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        Invoke(nameof(AllowSFX),1f);
    }

    public void PlaySFX(int _index,Transform _source=null)
    {
        if(!canPlaySFX) return;
        if(sfx[_index].isPlaying) return;
        if(_source!=null&&Vector2.Distance(PlayerManager.instance.player.transform.position,_source.position)>sfxMinimumDistance) return;
        if(_index<sfx.Length)
        {
            sfx[_index].pitch = Random.Range(1.1f,2.0f);
            sfx[_index].Play();
        }
    }
    public void PlayBGM(int _index)
    {
        StopAllBGM();
        if(_index<bgm.Length)
        {
            BGMIndex = _index;
            bgm[_index].Play();
        }
    }

    public void StopSFX(int _index) => sfx[_index].Stop();
    public void StopAllBGM()
    {
        for(int i=0;i<bgm.Length;++i)
        {
            bgm[i].Stop();
        }
    }

    public void StopSFXWithTime(int _index)=>StartCoroutine(DecreaseVolume(sfx[_index]));
    public void StopBGMWithTime(int _index)=>StartCoroutine(DecreaseVolume(bgm[_index]));

    //线性衰减
    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;
        while(_audio.volume>.1f)
        {
            _audio.volume -= _audio.volume*.2f;
            yield return new WaitForSeconds(.25f);

            if(_audio.volume<=.1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }



    public void PlayRandomBGM()
    {
        BGMIndex = Random.Range(0,bgm.Length);
        PlayBGM(BGMIndex);
    }

    private void AllowSFX()=>canPlaySFX = true;

}
