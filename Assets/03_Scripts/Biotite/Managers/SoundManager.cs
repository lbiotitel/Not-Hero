using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    Bgm,
    Sfx
}

//브금, sfx 재생 및 관리
public class SoundManager : MonoBehaviour, IEventListener
{
    [SerializeField] private int sfxSourceCount;
    [SerializeField] private float fadeInOutDelay = 0.5f;

    private float bgmVolume = 100f;
    private float sfxVolume = 100f;

    private AudioSource bgmSource;
    private AudioSource[] sfxSource;

    private Coroutine fadeInOutCoroutine;

    private Dictionary<string, AudioClip> audioResources = new Dictionary<string, AudioClip>();

    public void Init()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();

        sfxSource = new AudioSource[sfxSourceCount];

        for (int i = 0; i < sfxSourceCount; i++)
        {
            sfxSource[i] = gameObject.AddComponent<AudioSource>();
        }

        SetVolume(ESoundType.Bgm, bgmVolume);
        SetVolume(ESoundType.Sfx, sfxVolume);
        BindEvents();
        LoadSoundResources();
    }


    private void BindEvents()
    {
        Managers.Event.AddListener(EEventType.OnEnteredBossNode, this);
        Managers.Event.AddListener(EEventType.OnEnteredShopNode, this);
        Managers.Event.AddListener(EEventType.OnEnteredEventNode, this);
        Managers.Event.AddListener(EEventType.OnNodeCompleted, this);
        Managers.Event.AddListener(EEventType.OnBattleWin, this);
        Managers.Event.AddListener(EEventType.OnGameStart, this);
    }


    private void OnDestroy()
    {
        if (Managers.Instance != null)
        {
            Managers.Event.RemoveListener(EEventType.OnEnteredBossNode, this);
            Managers.Event.RemoveListener(EEventType.OnEnteredShopNode, this);
            Managers.Event.RemoveListener(EEventType.OnEnteredEventNode, this);
            Managers.Event.RemoveListener(EEventType.OnGameStart, this);
            Managers.Event.RemoveListener(EEventType.OnBattleWin, this);
            Managers.Event.RemoveListener(EEventType.OnNodeCompleted, this);
        }
    }


    //오디오 리소스들 캐싱
    private void LoadSoundResources()
    {
        List<AudioClip> resources = Managers.Data.GetDataList<AudioClip>();

        foreach (var r in resources)
        {
            audioResources.Add(r.name, r);
        }
    }


    //사운드 재생
    public void PlaySound(string soundName, bool loop = false, ESoundType soundType = ESoundType.Sfx)
    {
        if (soundName == null || soundName == string.Empty) return;

        if (soundType == ESoundType.Bgm)
        {
            if (!audioResources.ContainsKey(soundName))
            {
                Debug.LogError($"등록되지 않은 BGM: {soundName}");
                return;
            }

            if (bgmSource.clip != null && bgmSource.clip.name == soundName)
                return;

            if (fadeInOutCoroutine != null)
                StopCoroutine(fadeInOutCoroutine);

            fadeInOutCoroutine = StartCoroutine(CoSwitchBGM(soundName, loop, fadeInOutDelay));
        }
        else if (soundType == ESoundType.Sfx)
        {
            if (audioResources.ContainsKey(soundName))
            {
                for (int i = 0; i < sfxSource.Length; i++)
                {
                    if (!sfxSource[i].isPlaying)
                    {
                        sfxSource[i].clip = audioResources[soundName];
                        sfxSource[i].loop = loop;
                        sfxSource[i].Play();
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("등록되지 않는 SFX 입니다");
            }
        }
    }

    //사운드 정지
    public void StopSound(string soundName)
    {
        if (audioResources.ContainsKey(soundName))
        {
            if (bgmSource.clip == audioResources[soundName])
            {
                bgmSource.Stop();
            }
            else
            {
                Debug.LogError("재생되지 않고 있는 BGM 입니다");
            }
        }

        if (audioResources.ContainsKey(soundName))
        {
            AudioClip currentClip = audioResources[soundName];

            for (int i = 0; i < sfxSource.Length; i++)
            {
                if (sfxSource[i].clip == currentClip)
                {
                    sfxSource[i].Stop();
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("재생되지 않고 있는 sfx 입니다");
        }
    }

    //불륨 설정
    public void SetVolume(ESoundType soundType, float volume)
    {
        if (soundType == ESoundType.Bgm)
        {
            bgmVolume = volume;
            bgmSource.volume = bgmVolume * 0.01f;
        }
        else if (soundType == ESoundType.Sfx)
        {
            sfxVolume = volume;

            foreach (var s in sfxSource)
            {
                s.volume = sfxVolume * 0.01f;
            }
        }
    }

    private IEnumerator CoSwitchBGM(string soundName, bool loop, float duration)
    {
        float startVol = bgmSource.volume;
        float targetVol = bgmVolume * 0.01f;

        if (bgmSource.isPlaying)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
                yield return null;
            }
            bgmSource.volume = 0f;
            bgmSource.Stop();
        }

        bgmSource.clip = audioResources[soundName];
        bgmSource.loop = loop;
        bgmSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, targetVol, t / duration);
            yield return null;
        }
        bgmSource.volume = targetVol;

        fadeInOutCoroutine = null;
    }

    //노드의 선택에 따라 bgm 플레이
    public void OnEvent(EEventType eventType, object sender, object param = null)
    {
        switch (eventType)
        {
            case EEventType.OnGameStart:
                PlaySound(Defines.SoundKey.BATTLE_BGM, true, ESoundType.Bgm);
                break;
            case EEventType.OnNodeCompleted:
                PlaySound(Defines.SoundKey.BATTLE_BGM, true, ESoundType.Bgm);
                break;
            case EEventType.OnBattleWin:
                PlaySound(Defines.SoundKey.BATTLE_BGM, true, ESoundType.Bgm);
                break;
            case EEventType.OnEnteredShopNode:
                PlaySound(Defines.SoundKey.SHOP_BGM, true, ESoundType.Bgm);
                break;
            case EEventType.OnEnteredEventNode:
                PlaySound(Defines.SoundKey.EVENT_BGM, true, ESoundType.Bgm);
                break;
            case EEventType.OnEnteredBossNode:
                PlaySound(Defines.SoundKey.BOSS_BGM, true, ESoundType.Bgm);
                break;
        }
    }


    public void Clear()
    {
        audioResources?.Clear();
    }
}
