using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ControlManager : MonoBehaviour
{
    [SerializeField]
    private ProfileSettings m_profiles;

    [SerializeField]
    public InputActionAsset inputActions;

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
            CancelChanges();
        }
    }

    public void ApplyChanges()
    {
        if (Settings.profile)
        {
            Settings.profile.SaveControlSettings(inputActions);
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }//end of ApplyChanges

    public void CancelChanges()
    {
        if (Settings.profile)
        {
            Settings.profile.GetControlSettings(inputActions);
        }
    }//end of CancelChanges


    public void ResetBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("Settings_ControlActionRebinds");
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }//end of reset bindings
}//end of ControlManager
