using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
    public static AudioClip move, click, win;
    static AudioSource audioSrc;

    void Start()
    {
        move = Resources.Load<AudioClip> ("move");
        win = Resources.Load<AudioClip> ("win");
        click = Resources.Load<AudioClip> ("click");

        audioSrc = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip) {
            case "move":
                audioSrc.PlayOneShot(move);
                break;
            case "win":
                audioSrc.PlayOneShot(win);
                break;
            case "click":
                audioSrc.PlayOneShot(click);
                break;
        }
    }
}
