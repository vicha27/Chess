using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

[RequireComponent(typeof(Slider))]
public class MessySliders : MonoBehaviour
{
    Slider slider
    {
        get { return GetComponent<Slider>(); }
    }

    [Header("Volume Name")]
    [Tooltip("This is the name of the exposed parameter")]
    [SerializeField]
    private string volumeName;

    [Header("Volume Label")]
    [SerializeField]
    private TextMeshProUGUI volumeLabel;

    private void Start()
    {
        ResetSliderValue();
        slider.onValueChanged.AddListener(delegate
        {
            UpdateValueOnChange(slider.value);
        });
    }

    // Update is called once per frame
    public void UpdateValueOnChange(float value)
    {
        if (volumeLabel != null)
            volumeLabel.text = Mathf.Round(value * 100.0f).ToString() + "%";
        if (Settings.profile)
        {
            Settings.profile.SetAudioLevels(volumeName, value);
            //Debug.Log("Volume has changed for " + volumeName + ". " + string.Format("{0:N2}",value) + " should be the new value");
        }
    }

    public void ResetSliderValue()
    {
        if (Settings.profile)
        {
            float volume = Settings.profile.GetAudioLevels(volumeName);

            UpdateValueOnChange(volume);
            slider.value = volume;
        }
    }
}
