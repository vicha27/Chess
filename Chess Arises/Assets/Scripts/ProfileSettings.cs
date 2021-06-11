using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.InputSystem;

//when video refers to class Volume, use class Sound

public class Settings
{
    public static ProfileSettings profile;
}

[CreateAssetMenu(menuName = "ChessArises/Create Profile")]
public class ProfileSettings : ScriptableObject
{
    //Save the Player preferences with the Prefix 'Settings_'
    public bool saveInPlayerPrefs = true;
    public string prefPrefix = "Settings_";
    //AudioMixer controls all audio
    public AudioMixer audioMixer;
    public Sound[] volumeControl;
    //Resolution array for the different resolutions, use Resolution Index V for choice, fullScreen true or false
    Resolution[] resolutions;
    public int resolutionIndexV;
    public bool fullScreenToggle;

    public void SetProfile(ProfileSettings profile)
    {
        Settings.profile = profile;
    }

    public float GetAudioLevels(string name)
    {
        float volume = 1f;
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return volume;
        }

        for(int i = 0; i < volumeControl.Length; i++)
        {
            if(volumeControl[i].name != name)
            {
                continue;
            }
            else
            {
                if (saveInPlayerPrefs)
                {
                    if(PlayerPrefs.HasKey(prefPrefix + volumeControl[i].name))
                    {
                        volumeControl[i].volume = PlayerPrefs.GetFloat(prefPrefix + volumeControl[i].name);
                    }
                }
                volumeControl[i].tempVolume = volumeControl[i].volume;
                if (audioMixer)
                    audioMixer.SetFloat(volumeControl[i].name, Mathf.Log10(volumeControl[i].volume) * 20f);
                volume = volumeControl[i].volume;
                break;
            }
        }
        return volume;
    }

    public void GetAudioLevels()
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return;
        }

        for (int i = 0; i < volumeControl.Length; i++)
        {
            if (saveInPlayerPrefs)
            {
                if (PlayerPrefs.HasKey(prefPrefix + volumeControl[i].name))
                {
                    volumeControl[i].volume = PlayerPrefs.GetFloat(prefPrefix + volumeControl[i].name);
                }
            }
            //reset the audio volume
            volumeControl[i].tempVolume = volumeControl[i].volume;

            float vol2 = volumeControl[i].volume;
            //set the mixer to match the volume //try if it fails Mathf.Log10(volumeControl[i].volume) * 20f
            audioMixer.SetFloat(volumeControl[i].name, Mathf.Log10(volumeControl[i].volume) * 20f);         
        }
    }

    public void SetAudioLevels(string name, float volume)
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return;
        }
        if (AudioManager.muted)
        {
            for (int i = 0; i < volumeControl.Length; i++)
            {
                if (volumeControl[i].name != name)
                {
                    continue;
                }
                else
                {
                    //Debug.Log(volumeControl[i].name + " is the same value as " + name);
                    audioMixer.SetFloat(volumeControl[i].name, Mathf.Log10(volume) * 20);
                    volumeControl[i].tempVolume = volume;
                    //volumeControl[i].volume = volume;
                    break;
                }
            }
        }
    }

    public void Mute()
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return;
        }

        float volume = 0f;
        for (int i = 0; i < volumeControl.Length; i++)
        {
            audioMixer.SetFloat(volumeControl[i].name, -60);
            volumeControl[i].volume = volume;
        }
    }

    public void SaveAudioLevels()
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return;
        }

        float volume = 0f;
        for(int i = 0; i < volumeControl.Length; i++)
        {
            volume = volumeControl[i].tempVolume;
            if (saveInPlayerPrefs)
            {
                PlayerPrefs.SetFloat(prefPrefix + volumeControl[i].name, volume);
            }
            audioMixer.SetFloat(volumeControl[i].name, Mathf.Log10(volume) * 20);
            volumeControl[i].volume = volume;
        }
    }

    public void SaveVideoSettings()
    { 
        if (saveInPlayerPrefs)
        {
            resolutions = Screen.resolutions;
            Resolution resolution = resolutions[resolutionIndexV];
            bool boolFull = fullScreenToggle;
            int fullScreen = 1;
            if (boolFull)
                fullScreen = 1;
            else
                fullScreen = 0;
            PlayerPrefs.SetInt(prefPrefix + "Quality", QualitySettings.GetQualityLevel());
            PlayerPrefs.SetInt(prefPrefix + "Fullscreen", fullScreen);
            PlayerPrefs.SetInt(prefPrefix + "Resolution_Width", resolution.width); //might have to change later if this doesn't get saved - 05/15/2021
            PlayerPrefs.SetInt(prefPrefix + "Resolution_Height", resolution.height);
        }
    }

    public void GetVideoSettings()
    {
        if (saveInPlayerPrefs)
        {
            if (PlayerPrefs.HasKey(prefPrefix + "Quality"))
            {
               bool boolFull = true;
               int qualityIndex = PlayerPrefs.GetInt(prefPrefix + "Quality");
               int fullscreen = PlayerPrefs.GetInt(prefPrefix + "Fullscreen");
               int width =  PlayerPrefs.GetInt(prefPrefix + "Resolution_Width"); //might have to change later if this doesn't get saved - 05/15/2021
               int height = PlayerPrefs.GetInt(prefPrefix + "Resolution_Height");
               SetQuality(qualityIndex);
               if (fullscreen == 1)
                   boolFull = true;
               else
                   boolFull = false;
               SetFullscreen(boolFull);
               Screen.SetResolution(width, height, boolFull);
            }
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullScreen)
    {
        //Debug.Log(isFullScreen + " is the bool");
        fullScreenToggle = isFullScreen;
        Screen.fullScreen = isFullScreen;
        //Debug.Log(Screen.fullScreen + " is the screen");
    }

    public void SetResolution(int resolutionIndex)
    {
        resolutions = Screen.resolutions;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionIndexV = resolutionIndex;
    }

    public void SaveControlSettings(InputActionAsset actions)
    {
        if (saveInPlayerPrefs)
        {
            var rebinds = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(prefPrefix + "ControlActionRebinds", rebinds);
        }
    }//end SaveControlSettings

    public void GetControlSettings(InputActionAsset actions)
    {
        if (saveInPlayerPrefs)
        {
            if (PlayerPrefs.HasKey(prefPrefix + "ControlActionRebinds"))
            {
                var rebinds = PlayerPrefs.GetString(prefPrefix + "ControlActionRebinds");
                if (!string.IsNullOrEmpty(rebinds))
                {
                    actions.LoadBindingOverridesFromJson(rebinds);
                }
            }//end if
        }//end if
    }//end GetControlSettings
}
