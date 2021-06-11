using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private ProfileSettings m_profiles;

    [SerializeField]
    private TMP_Text MuteText;

    [SerializeField]
    private TMP_Text UnmuteText;

    [SerializeField]
    private GameObject MuteSymbol;

    [SerializeField]
    private GameObject UnmuteSymbol;

    [SerializeField]
    private GameObject ApplyButton;

    public static bool muted = true;

    [SerializeField]
    private List<MessySliders> m_VolumeSliders = new List<MessySliders>();

    //Use this for initialization
    void Awake()
    {
        if(m_profiles != null)
        {
            m_profiles.SetProfile(m_profiles);
        }
    }

    void Start()
    {
        if (Settings.profile && Settings.profile.audioMixer != null)
            Settings.profile.GetAudioLevels();
    }

    // Update is called once per frame
    void Update()
    {
        if (EndGameMenu.GameHasEnded)
        {
            Settings.profile.Mute();
        }
    }

    public void ApplyChanges()
    {
        if (Settings.profile && Settings.profile.audioMixer != null && muted)
        {
            Settings.profile.SaveAudioLevels();
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }

    public void CancelChanges()
    {
        if (Settings.profile && Settings.profile.audioMixer != null && muted)
        {
            Settings.profile.GetAudioLevels();
            for (int i = 0; i < m_VolumeSliders.Count; i++)
            {
                m_VolumeSliders[i].ResetSliderValue();
            }
        }
    }

    public void Mute()
    {
        muted = !muted;
        MuteText.gameObject.SetActive(muted);
        UnmuteText.gameObject.SetActive(!muted);
        MuteSymbol.SetActive(muted);
        UnmuteSymbol.SetActive(!muted);
        ApplyButton.SetActive(muted);
        if (Settings.profile && Settings.profile.audioMixer != null && !muted)
        {
            Settings.profile.Mute();
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
        if (Settings.profile && Settings.profile.audioMixer != null && muted)
        {
            CancelChanges();
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }

}
