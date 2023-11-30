using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Signalization : MonoBehaviour
{
    [SerializeField] private float _speed;

    private AudioSource _sound;
    private bool _isThiefDiscovered;
    private bool _isVolumeChanging;
    private float _minSoundValue;
    private float _maxSoundValue;
    private WaitForEndOfFrame _waitTime;

    void Awake()
    {
        _sound = GetComponent<AudioSource>();
        _isVolumeChanging = false;
        _minSoundValue = 0f;
        _maxSoundValue = 1f;
        _waitTime = new WaitForEndOfFrame();
    }

    private void Start()
    {
        _sound.volume = 0;
    }

    private void OnDestroy()
    {
        StopCoroutines();
    }

    private void OnDisable()
    {
        StopCoroutines();
    }

    public void ChangeAlarmState(bool isThiefDiscovered)
    {
        _isThiefDiscovered = isThiefDiscovered;

        if (_isVolumeChanging == false)
        {
            _isVolumeChanging = true;
            StartCoroutine(Alarm());
        }
    }

    private IEnumerator Alarm()
    {
        if (_sound.isPlaying == false)
        {
            _sound.Play();
        }

        while (_sound.isPlaying)
        {
            int alarmValue = System.Convert.ToInt32(_isThiefDiscovered);
            _sound.volume = Mathf.MoveTowards(_sound.volume, alarmValue, _speed * Time.deltaTime);

            if (_sound.volume <= _minSoundValue)
            {
                _sound.Stop();
                break;
            }
            else if (_sound.volume >= _maxSoundValue)
            {
                break;
            }

            yield return _waitTime;
        }

        _isVolumeChanging = false;
    }

    private void StopCoroutines()
    {
        StopCoroutine(Alarm());
    }
}
