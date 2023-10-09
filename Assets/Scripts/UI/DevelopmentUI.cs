using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevelopmentUI : MonoBehaviour
{
    [SerializeField] private GameObject m_DevContainer;
    [SerializeField] private Button m_OpenButton, m_ExitButton;
    [SerializeField] private Dropdown m_Levels, m_Locations;

    private bool m_IsReloadScene = false;

    private void Start()
    {
        List<string> levels = new List<string>();

        for (int i = 1; i <= 74; i++)
        {
            levels.Add("Level " + i.ToString());
        }

        m_Levels.AddOptions(levels);
        m_Levels.SetValueWithoutNotify(PlayerPrefs.GetInt(Constants.CurrentLevel, 0));

        List<string> locations = new List<string>();

        for (int i = 1; i <= 7; i++)
        {
            locations.Add("Location " + i.ToString());
        }

        m_Locations.AddOptions(locations);
        m_Locations.SetValueWithoutNotify(PlayerPrefs.GetInt(Constants.LocationCount, 0));
        

        m_OpenButton.onClick.AddListener(OpenButtonClick);
        m_ExitButton.onClick.AddListener(ExitButtonClick);

        m_Levels.onValueChanged.AddListener(SetLevel);
        m_Locations.onValueChanged.AddListener(SetLocation);
    }

    private void OpenButtonClick()
    {
        m_DevContainer.SetActive(true);
    }

    private void ExitButtonClick()
    {
        if (m_IsReloadScene)
        {
            DOTween.KillAll();
            SceneManager.LoadScene(Constants.GameScene);
        }
        else
            m_DevContainer.SetActive(false);
    }

    private void SetLevel(int _level)
    {
        m_IsReloadScene = true;
        PlayerPrefs.SetInt(Constants.CurrentLevel, _level);
    }

    private void SetLocation(int _location)
    {
        m_IsReloadScene = true;
        PlayerPrefs.SetInt(Constants.LocationCount, _location);
    }
}
