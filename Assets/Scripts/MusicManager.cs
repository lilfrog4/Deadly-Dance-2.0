using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioSource mainSongForThisScene;
    public AudioSource damageSound;
    public AudioSource loseSound;
    public AudioSource winSound;
    public Slider slider;
    public float volume;
    public float SongStartDelay;

    private const string VolumePrefKey = "MusicVolume";

    void Start()
    {
        // ��������� ����������� ��������� ��� ���������� �������� �� ��������� (0.5f)
        LoadVolume();

        // ������������� ��������� ��� ���� AudioSource
        ApplyVolumeToAllAudioSources();

        // mainSongForThisScene.Play();
        if (SongStartDelay < 0)
        {
            mainSongForThisScene.time = SongStartDelay * -1f;
            mainSongForThisScene.Play();
        }
        else
        {
            StartCoroutine(DelayedPlay(SongStartDelay));
        }
    }

    public void SliderMusic()
    {
        volume = slider.value;
        SaveVolume();
        ApplyVolumeToAllAudioSources();
    }
    IEnumerator DelayedPlay(float delay)        // Добавил на случай, если надо будет задержать трек
    {
        yield return new WaitForSeconds(delay);
        mainSongForThisScene.Play();
    }

    private void ApplyVolumeToAllAudioSources()
    {
        mainSongForThisScene.volume = volume;

        if (damageSound != null)
            damageSound.volume = volume;

        if (loseSound != null)
            loseSound.volume = volume;

        if (winSound != null)
            winSound.volume = volume;
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save(); // ����������� ����������
    }

    private void LoadVolume()
    {
        // ��������� ����������� ���������, ���� � ��� - ���������� 0.5f
        volume = PlayerPrefs.GetFloat(VolumePrefKey, 0.5f);

        // ��������� �������� ��������
        if (slider != null)
        {
            slider.value = volume;
        }
    }

    // �������������� ����� ��� ������ � �������� �� ��������� (�����������)
    public void ResetToDefaultVolume()
    {
        volume = 0.5f;
        slider.value = volume;
        SaveVolume();
        ApplyVolumeToAllAudioSources();
    }
}