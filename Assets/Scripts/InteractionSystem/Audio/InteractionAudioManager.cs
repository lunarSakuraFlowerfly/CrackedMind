using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem.Audio
{
    /// <summary>
    /// 交互音效管理器，管理交互系统的所有音效
    /// </summary>
    public class InteractionAudioManager : MonoBehaviour
    {
        // 单例实例
        public static InteractionAudioManager Instance { get; private set; }
        
        [Header("音频源")]
        [SerializeField] private AudioSource sfxSource; // 音效音频源
        [SerializeField] private AudioSource typingSource; // 打字机效果专用音频源
        
        [Header("交互音效")]
        [SerializeField] private AudioClip interactionStartClip; // 交互开始音效
        [SerializeField] private AudioClip interactionCompleteClip; // 交互完成音效
        [SerializeField] private AudioClip itemCollectClip; // 物品收集音效
        
        [Header("打字机音效")]
        [SerializeField] private AudioClip[] typingSounds; // 打字机音效数组
        [SerializeField] private AudioClip typingPunctuationSound; // 标点符号音效
        [SerializeField] [Range(0.1f, 1f)] private float typingVolume = 0.5f; // 打字机音效音量
        
        [Header("设置")]
        [SerializeField] private bool enableSFX = true; // 是否启用音效
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f; // 音效音量
        
        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // 如果没有音频源，添加音频源
                if (sfxSource == null)
                {
                    sfxSource = gameObject.AddComponent<AudioSource>();
                    sfxSource.playOnAwake = false;
                }
                
                if (typingSource == null)
                {
                    typingSource = gameObject.AddComponent<AudioSource>();
                    typingSource.playOnAwake = false;
                    typingSource.volume = typingVolume;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 播放交互开始音效
        /// </summary>
        public void PlayInteractionStartSound()
        {
            if (!enableSFX || interactionStartClip == null) return;
            
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(interactionStartClip);
        }
        
        /// <summary>
        /// 播放交互完成音效
        /// </summary>
        public void PlayInteractionCompleteSound()
        {
            if (!enableSFX || interactionCompleteClip == null) return;
            
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(interactionCompleteClip);
        }
        
        /// <summary>
        /// 播放物品收集音效
        /// </summary>
        public void PlayItemCollectSound()
        {
            if (!enableSFX || itemCollectClip == null) return;
            
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(itemCollectClip);
        }
        
        /// <summary>
        /// 播放打字机音效
        /// </summary>
        public void PlayTypingSound(char character)
        {
            if (!enableSFX || typingSounds == null || typingSounds.Length == 0) return;
            
            // 如果是标点符号，播放标点符号音效
            if (char.IsPunctuation(character) && typingPunctuationSound != null)
            {
                typingSource.PlayOneShot(typingPunctuationSound);
            }
            // 否则随机播放一个打字机音效
            else if (typingSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, typingSounds.Length);
                typingSource.PlayOneShot(typingSounds[randomIndex]);
            }
        }
        
        /// <summary>
        /// 设置音效启用状态
        /// </summary>
        public void SetSFXEnabled(bool enabled)
        {
            enableSFX = enabled;
        }
        
        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
        
        /// <summary>
        /// 设置打字机音效音量
        /// </summary>
        public void SetTypingVolume(float volume)
        {
            typingVolume = Mathf.Clamp01(volume);
            if (typingSource != null)
            {
                typingSource.volume = typingVolume;
            }
        }
    }
} 