using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound: MonoBehaviour {

    static AudioSource source;

    public Sounds[] sounds;
    static Dictionary<string, Sounds> dict;

    void Awake() {
        source = GetComponent<AudioSource>();

        dict = new Dictionary<string, Sounds>();

        foreach(Sounds s in sounds) {
            dict.Add(s.name, s);
        }
    }

    public static void Play(string sound, float multiplier = 1) {
        if(dict.ContainsKey(sound)) {
            Sounds s = dict[sound];
            AudioClip clip = s.clips[Random.Range(0, s.clips.Length)];

            source.PlayOneShot(clip, s.volume * multiplier);
        }
        else {
            Debug.LogWarning("Could not play sound with the name of '" + sound + "'");
        }
    }

    [System.Serializable]
    public struct Sounds {
        public string name;
        [Range(0, 1)]
        public float volume;
        public AudioClip[] clips;
    }
}