using UnityEngine;

public class Scale : MonoBehaviour
{
    private AudioSource[] _auds;
    private AudioSource[] auds {
        get {
            if(_auds == null) { _auds = GetComponentsInChildren<AudioSource>(); } return _auds;
        }
    }

    [SerializeField]
    private float startOffset = 0f;
    [SerializeField, Header("EndTime: defaults to clip length if zero")]
    private float endTime;
    [SerializeField]
    private int noteCount = 13;

    private float noteTime {
        get { return (endTime - startOffset) / noteCount; }
    }

    private AudioSource nextAudioSource() {
        foreach(AudioSource aud in auds) {
            if(!aud.isPlaying) { return aud; }
        }
        auds[0].Stop();
        return auds[0];
    }

    public void play(int note) {
        while(note < 0) { note += noteCount; }
        note = note % noteCount;
        AudioSource next = nextAudioSource();
        if(next == null) { return; }
        next.time = note * noteTime + noteTime * .05f;
        next.Play();
        next.SetScheduledEndTime(AudioSettings.dspTime + noteTime * .65f);
    }

    private void Start() {
        if(endTime < .01f) {
            endTime = auds[0].clip.length;
        }
    }

}
