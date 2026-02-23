using GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class InstructionMenu : MonoBehaviour
{
    public Button backHomeButton;
    public Slider volumeSlider;

    void Start()
    {
        if (backHomeButton != null) backHomeButton.onClick.AddListener(GoBackHome);
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(SetVolume);
        if (volumeSlider) volumeSlider.value = PlayerPrefs.HasKey("BgmVolume") ? PlayerPrefs.GetFloat("BgmVolume") : 0.6f;
    }

    private void SetVolume(float v)
    {
        PlayerPrefs.SetFloat("BgmVolume", v);
        if (GameManager.Ist.bgmAudio) GameManager.Ist.bgmAudio.volume = v;
    }

    private void GoBackHome()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
