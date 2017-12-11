using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : PhoenixLikeSingleton<AudioManager> {


    Dictionary<string, AudioSource> audios = new Dictionary<string, AudioSource>();

    private Watchable<bool> _muted;
    private Watchable<bool> muted {
        get {
            if(_muted == null) {
                _muted = new Watchable<bool>(false);
            }
            return _muted;
        }
    }

    private void setMuted(bool shouldMute) {
        muted._value = shouldMute;
    }

    public void toggleMute() {
        setMuted(!muted._value);
    }

    private Scale _scale;
    private Scale scale {
        get { if(!_scale) { _scale = GetComponentInChildren<Scale>(); } return _scale; }
    }

    public static string Dink = "Dink";

    public void Awake() {
        setupAudios();
    }

    protected override void onSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        setupAudios();
    }

    private void setupAudios() {
        bool needSetup = audios.Count == 0;
        foreach(string k in audios.Keys) {
            if(!audios[k]) {
                needSetup = true;
                break;
            }
        }

        if(!needSetup) { return; }
        audios.Clear();

        foreach(AudioSource au in GetComponentsInChildren<AudioSource>()) {
            audios.Add(au.gameObject.name, au);
        }
        
    }

    public void playDink() { play(Dink); }

    public void play(string name, Action callback = null) {
        AudioSource aud = getSource(name);

        if(aud && !muted._value) { aud.Play(); }

        if(callback != null) {
            CallbackHelper.Instance.waitThenInvoke(aud ? aud.clip.length : .1f, callback);
        }
    }

    public void note(int i) {
        scale.play(i);
    }

    private AudioSource getSource(string resourcesAudioRelativePath) {
        //lazily load audio clip
        if(audios.ContainsKey(resourcesAudioRelativePath)) { return audios[resourcesAudioRelativePath]; }
        AudioClip clip = Resources.Load<AudioClip>(string.Format("Audio/Clip/{0}", resourcesAudioRelativePath));
        Assert.IsTrue(clip, "null audio clip? " + resourcesAudioRelativePath);
        GameObject go = new GameObject(resourcesAudioRelativePath);
        AudioSource aud = go.AddComponent<AudioSource>();
        aud.clip = clip;
        audios.Add(go.name, aud);
        return aud;
    }

}
