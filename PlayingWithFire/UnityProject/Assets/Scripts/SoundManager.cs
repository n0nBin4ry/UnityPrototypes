/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundType
    {
        PlayerMovementSound,
        ShootFireSound,
        OnFireSound,
        ExplosionSound,
        BossDeathSound, 
        PlayerDeathSound,
        WinSound,
        LoseSound,
        BumpSound, 
        ErrorSound, 
        MysterySound
    }

    private static List<AudioSource> s_AllAudioSources = new List<AudioSource>();

    private void Start()
    {
        for(int x = 0; x < transform.childCount; x++) {
            s_AllAudioSources.Add(transform.GetChild(x).GetComponent<AudioSource>());
        }
    }

    public static void PlaySound(SoundType soundType) {
        AudioSource soundAudioSource = s_AllAudioSources[(int) soundType];
		if (soundAudioSource != null)
			soundAudioSource.PlayOneShot(soundAudioSource.clip);
    } 
}
*/