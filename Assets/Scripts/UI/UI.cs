using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour, ISaveManager
{

    [Header("End Screen")]
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private UI_FadeScreen fadeScreen;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject InGameUI;

    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        SwitchTo(InGameUI);
        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchWithKeyTo(optionsUI);
    }

    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;

            if (!fadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(7, null);
            _menu.SetActive(true);
        }

        if (GameManager.instance != null)
        {
            if (_menu == InGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(InGameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parametr == pair.Key)
                    item.LoadSLider(pair.Value);
            }
        }
    }

    public void SaveData(GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }

    public IEnumerator LoadSceneWithFadeEffect(float _delay, string _sceneName)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(_sceneName);
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();

    public void SaveAndExitButton() => StartCoroutine(WaitOnExitGame(2, "MainMenu"));

    private IEnumerator WaitOnExitGame(float _delay, string _scene)
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.1f);
        SwitchTo(InGameUI);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(_scene);
    }

}
