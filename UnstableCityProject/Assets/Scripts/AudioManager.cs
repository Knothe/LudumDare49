using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    List<AudioClip> buttonSounds;

    [SerializeField]
    List<AudioClip> sfxList;

    public void PlayButtonSound() {
        audioSource.PlayOneShot(buttonSounds[Random.Range(0, 2)]);
    }

    public void PlaySFX(AudioSFXClip clip) {
        audioSource.PlayOneShot(sfxList[(int)clip]);
    }
}

public enum AudioSFXClip {
    CLOSE_TONE,
    ENGINE,
    PICKAXE,
    POP,
    WATER_DROP,
    WOOD_CUT
}