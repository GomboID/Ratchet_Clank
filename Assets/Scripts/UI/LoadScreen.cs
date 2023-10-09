using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] private Image m_LoadProgress;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        AsyncOperation m_SceneLoading = SceneManager.LoadSceneAsync(Constants.GameScene);
        m_SceneLoading.allowSceneActivation = false;
        float minTimer = 3f;

        while (minTimer > 0)
        {
            m_LoadProgress.fillAmount = Mathf.Lerp(m_LoadProgress.fillAmount, m_SceneLoading.progress / 0.9f, 1f - minTimer / 3f);
            yield return new WaitForSeconds(Time.deltaTime);

            minTimer -= Time.deltaTime;
            minTimer = Mathf.Clamp(minTimer, 0f, 3f);
            if (minTimer <= 0)
                m_SceneLoading.allowSceneActivation = true;
        }
    }
}
