using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Canvas")] 
    [SerializeField] private CanvasGroup _uiCanvas;
    [Header("Audio")] 
    [SerializeField] private Slider _sliderMasterVolume;
    [SerializeField] private Slider _sliderMusicVolume;
    [SerializeField] private Slider _sliderSoundsVolume;

    private Action _onClose;

    private void OnEnable()
    {
        _uiCanvas.alpha = 0f;
        _uiCanvas.DOFade(1f, 0.5f).OnComplete(FadeInComplete);

        UpdateAudioDisplay();
    }

    public void SliderValueChanged()
    {
        AudioManager.Instance.GlobalVolume = _sliderMasterVolume.value;
        AudioManager.Instance.MusicVolume = _sliderMusicVolume.value;
        AudioManager.Instance.SfxVolume = _sliderSoundsVolume.value;

        SaveDataHelper.SaveDeviceData();

        AudioManager.Instance.UpdateVolumeFromSaveData();

        UpdateAudioDisplay();
    }

    private void UpdateAudioDisplay()
    {
        _sliderMasterVolume.SetValueWithoutNotify(SaveDataHelper.ActiveDeviceData.GlobalVolume);
        _sliderMusicVolume.SetValueWithoutNotify(SaveDataHelper.ActiveDeviceData.MusicVolume);
        _sliderSoundsVolume.SetValueWithoutNotify(SaveDataHelper.ActiveDeviceData.SfxVolume);
    }


    public void ButtonClose()
    {
        _uiCanvas.DOFade(0f, 0.5f).OnComplete(FadeOutComplete);
    }

    public void OpenMenu(Action onCloseCallback)
    {
        gameObject.SetActive(true);
        _onClose += onCloseCallback;
    }

    private void FadeInComplete()
    {
    }

    private void FadeOutComplete()
    {
        gameObject.SetActive(false);
        _onClose?.Invoke();
        _onClose = null;
    }
}