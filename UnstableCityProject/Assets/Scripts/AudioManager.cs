using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    [SerializeField]
    AudioSource[] musicTracks;

    [SerializeField]
    AudioSource sfxSource;

    [SerializeField]
    List<AudioClip> buttonSounds;

    [SerializeField]
    List<AudioClip> sfxList;

    [SerializeField]
    Toggle audioToggle;

    [SerializeField]
    Image on, off;

    int currentClip = 0;

    private void Start() {
        if (AudioListener.volume == 0) {
            audioToggle.isOn = false;
            on.gameObject.SetActive(false);
            off.gameObject.SetActive(true);
            audioToggle.targetGraphic = off;
        }
        musicTracks[0].volume = 1;
        musicTracks[1].volume = 0;
    }

    public void SwapTrack(int id) {
        if (id == currentClip)
            return;
        Debug.Log("Changed");
        StopAllCoroutines();
        StartCoroutine(FadeTrack(id));
        currentClip = id;
    }

    public void ToggleMute() {
        bool b = audioToggle.isOn;
        AudioListener.volume = (b) ? 1 : 0;
        on.gameObject.SetActive(b);
        off.gameObject.SetActive(!b);
        audioToggle.targetGraphic = b ? on : off;
    }

    public void PlayButtonSound() {
        sfxSource.PlayOneShot(buttonSounds[Random.Range(0, 2)]);
    }

    public void PlaySFX(AudioSFXClip clip) {
        sfxSource.PlayOneShot(sfxList[(int)clip]);
    }

    IEnumerator FadeTrack(int clipId) {
        float timeToFade = .25f;
        float timeElapsed = 0;

        int otherClilp = clipId == 0 ? 1 : 0;
        while(timeElapsed < timeToFade) {
            musicTracks[clipId].volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
            musicTracks[otherClilp].volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        musicTracks[clipId].volume = 1;
        musicTracks[otherClilp].volume = 0;
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