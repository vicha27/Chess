using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoManager : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    Resolution[] resolutions;

    public TMP_Dropdown resolutionDropdown; //TMPro.TMP_Dropdown[] myDropDownList = GetComponentsInChildren<TMPro.TMP_Dropdown>();

    [SerializeField]
    private ProfileSettings m_profiles;

    public Toggle fullestScreen;

    //Use this for initialization
    void Awake()
    {
        if (m_profiles != null)
        {
            m_profiles.SetProfile(m_profiles);
        }
    }

    void Start()
    {
        if (Settings.profile)
        {
            Settings.profile.GetVideoSettings();
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x" + resolutions[i].height;
                options.Add(option);
                if (i == Settings.profile.resolutionIndexV)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
            fullestScreen.isOn = Settings.profile.fullScreenToggle;
        }
    }

    public void ApplyChanges()
    {
        if (Settings.profile)
        {
            Settings.profile.SaveVideoSettings();
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }

    public void CancelChanges()
    {
        if (Settings.profile)
            Settings.profile.GetVideoSettings();
    }
}
