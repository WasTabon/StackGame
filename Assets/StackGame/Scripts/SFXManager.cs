using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = 0.3f;
        }
    }

    public void PlayRotate()
    {
        PlayGenerated(440f, 0.06f, 0.15f);
    }

    public void PlayConfirm()
    {
        PlayGenerated(880f, 0.08f, 0.2f);
    }

    public void PlayMatch()
    {
        PlayChord(new float[] { 523f, 659f, 784f }, 0.2f, 0.25f);
    }

    public void PlayChain(int step)
    {
        float baseFreq = 523f + (step - 1) * 200f;
        PlayChord(new float[] { baseFreq, baseFreq * 1.25f, baseFreq * 1.5f }, 0.3f, 0.3f);
    }

    public void PlayDrop()
    {
        PlaySweep(300f, 100f, 0.15f, 0.12f);
    }

    public void PlaySpawn()
    {
        PlaySweep(200f, 500f, 0.1f, 0.1f);
    }

    public void PlayGameOver()
    {
        PlayChord(new float[] { 200f, 150f, 100f }, 0.6f, 0.3f);
    }

    public void PlayLevelComplete()
    {
        PlayArpeggio(new float[] { 523f, 659f, 784f, 1047f }, 0.08f, 0.2f);
    }

    public void PlaySelect()
    {
        PlayGenerated(660f, 0.03f, 0.1f);
    }

    public void PlayBonus()
    {
        PlayChord(new float[] { 600f, 750f, 900f }, 0.15f, 0.2f);
    }

    public void PlayCancel()
    {
        PlaySweep(400f, 200f, 0.1f, 0.1f);
    }

    private void PlayGenerated(float freq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("sfx", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration);
            envelope *= envelope;
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * volume;
        }

        clip.SetData(data, 0);
        sfxSource.PlayOneShot(clip, 1f);
    }

    private void PlayChord(float[] freqs, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("chord", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration);
            envelope *= envelope;
            float sample = 0f;
            for (int f = 0; f < freqs.Length; f++)
                sample += Mathf.Sin(2f * Mathf.PI * freqs[f] * t);
            data[i] = sample / freqs.Length * envelope * volume;
        }

        clip.SetData(data, 0);
        sfxSource.PlayOneShot(clip, 1f);
    }

    private void PlaySweep(float startFreq, float endFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("sweep", samples, 1, sampleRate, false);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration);
            float freq = Mathf.Lerp(startFreq, endFreq, t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * volume;
        }

        clip.SetData(data, 0);
        sfxSource.PlayOneShot(clip, 1f);
    }

    private void PlayArpeggio(float[] freqs, float noteGap, float volume)
    {
        for (int i = 0; i < freqs.Length; i++)
        {
            float freq = freqs[i];
            float delay = i * noteGap;
            StartCoroutine(PlayDelayed(freq, 0.12f, volume, delay));
        }
    }

    private System.Collections.IEnumerator PlayDelayed(float freq, float duration, float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayGenerated(freq, duration, volume);
    }
}
