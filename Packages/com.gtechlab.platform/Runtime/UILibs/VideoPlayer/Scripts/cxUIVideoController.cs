using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class cxUIVideoController : MonoBehaviour {
    public VideoPlayer _player;
    public RawImage _playerRenderer;
    public AspectRatioFitter aspectRatio;
    public Slider onPlayTimeSlider;
    public cxUITouchDelegator touchDelegator;

    //
    public cxUITweenPanel controlPanel;
    public int hideScreenControlTime = 0;
    public Slider playbackSlider;
    //public Slider speedSlider;
    public Slider volumeSlider;
    public Text currentTime;
    public Text totalTime;
    public Button playButton;
    public Button pauseButton;
    public Button volumeButton;

    //
    public GameObject loading;

    public GameObject errorPanel;
    public Text errorText;

    private bool showPlayerControl = false;
    private bool showingVolume = false;
    private bool seeking = false;

    public UnityEvent OnVideoReady = new UnityEvent ();
    public UnityEvent OnVideoFinished = new UnityEvent ();

    RenderTexture videoRenderTexture;

    private void Awake () {

        _player.loopPointReached += _onVideoEnded;
        _player.prepareCompleted += _onVideoReady;
        _player.seekCompleted += _onVideoSeekCompleted;
        _player.errorReceived += _onVideoError;

        volumeSlider.minValue = 0.0f;
        volumeSlider.maxValue = 1.0f;
        volumeSlider.onValueChanged.AddListener ((value) => {
            ChangeVolume (value);
        });

        playbackSlider.maxValue = 1.0f;
        playbackSlider.minValue = 0.0f;
        playbackSlider.onValueChanged.AddListener ((value) => {
            ChangeVideoTime (value);
        });

        playButton.onClick.AddListener (() => {
            Play ();
        });

        pauseButton.onClick.AddListener (() => {
            Pause ();
        });

        touchDelegator.onPointerClick.AddListener (() => {
            // if (showPlayerControl)
            //     HideControlPanel ();
            // else
            //     ShowControlPanel (true);

            ShowControlPanel (true);
        });

        volumeButton.onClick.AddListener (() => {
            ToggleVolumeSlider ();
        });
    }

    private void OnDisable () {
        _player.Stop ();
    }

    private void OnDestroy () {
        _player.Stop ();
    }
    void _onVideoReady (VideoPlayer vp) {
        var totalTime = _player.length;

        _player.targetTexture = new RenderTexture ((int) _player.width, (int) _player.height, 0);
        _playerRenderer.texture = _player.targetTexture;
        _playerRenderer.color = Color.white;

        var canvas = _playerRenderer.transform.parent as RectTransform;
        var cRatio = canvas.rect.width / canvas.rect.height;
        aspectRatio.aspectRatio = cRatio;

        OnVideoReady.Invoke ();
        _player.Play ();
        loading.SetActive (false);
        ShowControlPanel (true);
    }

    void _onVideoEnded (VideoPlayer vp) {
        _player.Stop ();

        OnVideoFinished.Invoke ();
    }

    void _onVideoSeekCompleted (VideoPlayer vp) {
        seeking = false;
        _player.Play ();
        loading.SetActive (false);
        ShowControlPanel (true);
    }
    void _onVideoError(VideoPlayer vp, string message){
        loading.SetActive (false);
        ShowControlPanel (false);

        errorPanel.SetActive(true);
        errorText.text = message;
    }

    public void InitPlayer () {
        playbackSlider.value = 0;
        currentTime.text = string.Empty;
        totalTime.text = string.Empty;
        _playerRenderer.texture = null;
        _playerRenderer.color = Color.black;

        seeking = false;
        loading.SetActive (true);
        errorPanel.SetActive(false);
        HideControlPanel ();
    }
    
    public void Play (string url) {
        if (cxResourceNaming.IsHttp (url)) {
            _player.url = url;
            _player.Play ();
        } else if (cxResourceNaming.IsStreaming (url, out string path)) {
            _player.url = cxResourceNaming.ToAppStreamingPath (path);
            _player.Play ();
        } else {
            LoadVideoClipPlay (url);
        }
    }

    async void LoadVideoClipPlay (string resourceURL) {

        try {
            var videoClip = await cxUniversalResourceLoader.Instance.LoadAsset<VideoClip> (resourceURL);

            if (videoClip == null)
                throw new Exception ("Video resource not found");

            if (videoClip)
                Play (videoClip);

        } catch (Exception ex) {
            Debug.LogException (ex);
            errorPanel.SetActive(true);
            errorText.text = ex.Message;
        }
    }

    public void Play (VideoClip videoClip) {
        _player.clip = videoClip;
        _player.Play ();
    }

    public void Play () {
        _player.Play ();

        ShowControlPanel (true);
    }

    public void Pause () {
        _player.Pause ();
        ShowControlPanel (false);
    }

    public void ChangeVolume (float volume) {
        switch (_player.audioOutputMode) {
            case VideoAudioOutputMode.Direct:
                _player.SetDirectAudioVolume (0, volume);
                break;
            case VideoAudioOutputMode.AudioSource:
                _player.GetComponent<AudioSource> ().volume = volume;
                _player.SetDirectAudioVolume (0, volume);
                break;
            default:
                _player.GetComponent<AudioSource> ().volume = volume;
                _player.SetDirectAudioVolume (0, volume);
                break;
        }
    }

    public void ChangePlaybackSpeed (float speed) {
        if (!_player.canSetPlaybackSpeed) return;
        if (speed <= 0) {
            _player.playbackSpeed = .5f;
        } else {
            _player.playbackSpeed = speed;
        }
    }

    public void ChangeVideoTime (float value) {
        float time = value * (float) _player.length;

        _player.time = time;
        _player.Pause ();
        loading.SetActive (true);
        seeking = true;
    }

    Coroutine coAutoHideControlPanel;

    public void ShowControlPanel (bool autoHide = false) {
        if (!showPlayerControl) {
            controlPanel.Play ();
            showPlayerControl = true;
        }

        onPlayTimeSlider.gameObject.SetActive (false);

        if (coAutoHideControlPanel != null)
            StopCoroutine (coAutoHideControlPanel);

        if (autoHide)
            coAutoHideControlPanel = StartCoroutine (AutoHideControlPanel ());
    }

    IEnumerator AutoHideControlPanel () {
        yield return new WaitForSeconds (hideScreenControlTime);
        HideControlPanel ();
    }

    public void HideControlPanel () {
        if (coAutoHideControlPanel != null)
            StopCoroutine (coAutoHideControlPanel);

        controlPanel.Hide ();
        volumeSlider.gameObject.SetActive (false);
        onPlayTimeSlider.gameObject.SetActive (true);

        showPlayerControl = false;
        showingVolume = false;
    }

    public void ToggleVolumeSlider () {
        if (showingVolume) {
            showingVolume = false;
            volumeSlider.gameObject.SetActive (false);
        } else {
            showingVolume = true;
            volumeSlider.gameObject.SetActive (true);
        }
    }

    private void Update () {
        if (_player.isPrepared) {

            playButton.gameObject.SetActive (!_player.isPlaying);
            pauseButton.gameObject.SetActive (_player.isPlaying);

            totalTime.text = FormatTime (Mathf.RoundToInt ((float) _player.length));

            if (seeking) {
                float seekTime = playbackSlider.value * (float) _player.length;
                currentTime.text = FormatTime (Mathf.RoundToInt ((float) seekTime));
            } else {

                currentTime.text = FormatTime (Mathf.RoundToInt ((float) _player.time));

                float t = (float) _player.time / (float) _player.length;
                playbackSlider.SetValueWithoutNotify (t);
                onPlayTimeSlider.SetValueWithoutNotify (t);
            }

        } else {
            playButton.gameObject.SetActive (!_player.isPlaying);
            pauseButton.gameObject.SetActive (_player.isPlaying);

            totalTime.text = FormatTime (0);
            currentTime.text = FormatTime (0);
            playbackSlider.SetValueWithoutNotify (0);
            onPlayTimeSlider.SetValueWithoutNotify (0);

            loading.SetActive (true);
        }
    }

    private string FormatTime (int time) {
        int hours = time / 3600;
        int minutes = (time % 3600) / 60;
        int seconds = (time % 3600) % 60;
        if (hours == 0 && minutes != 0) {
            return minutes.ToString ("00") + ":" + seconds.ToString ("00");
        } else if (hours == 0 && minutes == 0) {
            return "00:" + seconds.ToString ("00");
        } else {
            return hours.ToString ("00") + ":" + minutes.ToString ("00") + ":" + seconds.ToString ("00");
        }
    }

}