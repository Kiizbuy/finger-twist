using GameFramework.Events;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace GameFramework.Components
{
    ///TODO: Refactor this Shitty-Code
    public class PlaySound : MonoBehaviour
    {
        public event Action<EventParameter> OnCompleteClip;

        [EventName(nameof(OnCompleteClip))]
        public EventToMethodSubscribeСontainer OnCompleteClipSubscriber = new EventToMethodSubscribeСontainer();
        [MethodName(nameof(Play))]
        public MethodToEventSubscribeContainer PlaySubscriber = new MethodToEventSubscribeContainer();
        [MethodName(nameof(Stop))]
        public MethodToEventSubscribeContainer StopSubscriber = new MethodToEventSubscribeContainer();
        [MethodName(nameof(PlayOneShot))]
        public MethodToEventSubscribeContainer PlayOneShotSubscriber = new MethodToEventSubscribeContainer();

        [SerializeField] private AudioClip _sound;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField, Range(0f, 1f)]  private float _volume = 1f;
        [Header("Fade"), SerializeField] private bool _smoothFade = false;
        [SerializeField] private float _fadeTime = 1;
        [SerializeField] private bool _multiplePlayback = true;

        protected static bool AudioMixerInitialized;
        protected static AudioMixerGroup MusicAudioMixerGroup;
        protected static AudioMixerGroup SFXAudioMixerGroup;
        protected static AudioMixerGroup VoicesAudioMixerGroup;


        private void Awake()
        {
            //if (!AudioMixerInitialized)
            //    InitializeAudioMixer();

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null)
                    Debug.LogError($"in {gameObject.name} doesn't have an audio source", gameObject);
            }
        }

        private void Start()
        {
            EventSubscriber.Subscribe(this);
        }
        ///Very bad method - Refactor this
        private void InitializeAudioMixer()
        {
            var mixer = Resources.Load<AudioMixer>("GameMasterSound");

            if (mixer == null)
            {
                Debug.LogError("Can't load AudioMixer in resources by name: MasterAudioMixer");
                return;
            }

            var musicGroups = mixer.FindMatchingGroups("Music");

            if (musicGroups.Length == 1)
                MusicAudioMixerGroup = musicGroups[0];
            else if (musicGroups.Length == 0)
                Debug.LogError("Can't find AudioMixerGroup with name: Music.");
            else if (musicGroups.Length == 1)
                Debug.LogError("Can't find AudioMixerGroup with name: Music. There are more than 1 group matching that name");

            var soundGroups = mixer.FindMatchingGroups("SFX");

            if (soundGroups.Length == 1)
                SFXAudioMixerGroup = soundGroups[0];
            else if (soundGroups.Length == 0)
                Debug.LogError("Can't find AudioMixerGroup with name: SFX.");
            else if (soundGroups.Length == 1)
                Debug.LogError("Can't find AudioMixerGroup with name: SFX. There are more than 1 group matching that name");

            var voicesGroups = mixer.FindMatchingGroups("Voices");

            if (voicesGroups.Length == 1)
                VoicesAudioMixerGroup = voicesGroups[0];
            else if (soundGroups.Length == 0)
                Debug.LogError("Can't find AudioMixerGroup with name: Voices.");
            else if (soundGroups.Length == 1)
                Debug.LogError("Can't find AudioMixerGroup with name: Voices. There are more than 1 group matching that name");

            AudioMixerInitialized = true;
        }

        public void PlayOneShot(EventParameter parameter)
        {
            if (_audioSource == null || this == null)
                return;

            if (_audioSource.isPlaying && !_multiplePlayback)
                return;

            _audioSource.Stop();
            _audioSource.PlayOneShot(_sound);
        }

        public void Play(EventParameter parameter)
        {
            if (_audioSource == null || this == null)
                return;

            if (_audioSource.isPlaying && !_multiplePlayback)
                return;

            _audioSource.clip = _sound;
            _audioSource.volume = _volume;

            StopAllCoroutines();

            if (_smoothFade)
                StartCoroutine(PlayCoroutine());
            else
                _audioSource.Play();

            StartCoroutine(WaitCompleteCoroutine());
        }

        public void Stop(EventParameter parameter)
        {
            Debug.Log("Stop");

            if (_audioSource == null)
                return;

            StopAllCoroutines();

            if (_smoothFade)
                StartCoroutine(StopCoroutine());
            else
                _audioSource.Stop();
        }

        private IEnumerator PlayCoroutine()
        {
            var timer = 0f;
            _audioSource.Play();
            _audioSource.volume = 0;

            while (timer < _fadeTime)
            {
                timer += Time.deltaTime;
                _audioSource.volume = Map(timer / _fadeTime, 0, 1, 0, _volume);
                yield return null;
            }
        }

        private IEnumerator StopCoroutine()
        {
            var timer = _fadeTime;

            _audioSource.volume = _volume;

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                _audioSource.volume = Map(timer / _fadeTime, 0, 1, 0, _volume);
                yield return null;
            }
            _audioSource.Stop();
        }

        private IEnumerator WaitCompleteCoroutine()
        {
            while (_audioSource.isPlaying)
                yield return null;
            OnCompleteClip?.Invoke(null);
        }

        private float Map(float s, float a1, float a2, float b1, float b2)
            => b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}

