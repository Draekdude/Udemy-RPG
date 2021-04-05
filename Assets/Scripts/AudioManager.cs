using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] sfx;
    public AudioSource[] bgm;

    public static AudioManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            PlayMusic(3);
            PlaySoundEffect(0);
        }
    }

    public void PlaySoundEffect(int soundIndex)
    {
        if (soundIndex < sfx.Length)
        {
            sfx[soundIndex].Play();
        }
    }

    public void PlayMusic(int songIndex)
    {
        if (!bgm[songIndex].isPlaying)
        {
            bgm.ToList().ForEach(s => s.Stop());
            if (songIndex < bgm.Length)
            {
                bgm[songIndex].Play();
            }
        }
    }
}
