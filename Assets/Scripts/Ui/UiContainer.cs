using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
public class UiContainer : MonoBehaviour
{
    [SerializeField] private LvlController _lvlController;

    [Header("GameLvl")]
    [SerializeField] private GameObject _scoreBar;
    [SerializeField] private TMP_Text _inLvlScore;
    [SerializeField] private TMP_Text _multiplayerText;

    [Header("EndLvlPanel")]
    [SerializeField] private GameObject _endLvlPanel;
    [SerializeField] private TMP_Text _lvlEnded;
    [SerializeField] private TMP_Text _totalScore;
    [SerializeField] private Button _nextLvlButton;

    [Header("StartPanel")]
    [SerializeField] private GameObject _swipeToStartPanel;

    private UiLocalization _localization;

    private Languages _currentLanguage;
    private readonly string _localizationPath = "Localization/";

    private void Awake()
    {
        _currentLanguage = Languages.English; // Posibility yo be choozen
        string path = _localizationPath + _currentLanguage.ToString();
        _localization = Resources.Load<UiLocalization>(path);
        _nextLvlButton.onClick.AddListener(() =>
        {
            _lvlController.NextLvl();
            PrepareLvl();
        });
        _lvlController.LvlPointsChanged += OnLvlPointsChanged;
        _lvlController.MultiplierChanged += OnMultiplayerChanged;
        _lvlController.LvlCompleted += OnLvlCompleted;
        _lvlController.LvlStarted += OnLvlStarted;
        PrepareLvl();
    }

    public string GetText(UiElement element)
    {
        try
        {
            return  _localization.UiDatas.First(d => d.UiElement == element).Text;
        }
         catch
        {
            return "Element is not localized";
        }
    }
    private void PrepareLvl()
    {
        _endLvlPanel.SetActive(false);
        _swipeToStartPanel.SetActive(true);
        _scoreBar.SetActive(true);
        _inLvlScore.text = "0"; 
    }

    private void OnLvlStarted()
    {
        _swipeToStartPanel.SetActive(false);
    }

    private void OnLvlCompleted()
    {
        _scoreBar.SetActive(false);
        _endLvlPanel.SetActive(true);
        _totalScore.text = GetText(UiElement.TotalScore) + _lvlController.LvlPoints;
        _lvlEnded.text = GetText(UiElement.Lvl) + " " + ProjectPrefs.CurrentLvl.GetValue();
       
    }

    private void OnLvlPointsChanged(int value)
    {
        _inLvlScore.text = value.ToString();
    }

    private void OnMultiplayerChanged(int value)
    {
        if (!_lvlController.IsLvlStarted)
            return;

        _multiplayerText.text = "x" +value;
        _multiplayerText.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        _lvlController.LvlPointsChanged -= OnLvlPointsChanged;
        _lvlController.MultiplierChanged -= OnMultiplayerChanged;
        _nextLvlButton.onClick.RemoveAllListeners();
        _lvlController.LvlStarted -= OnLvlStarted;
        _lvlController.LvlCompleted -= OnLvlCompleted;
    }
}
