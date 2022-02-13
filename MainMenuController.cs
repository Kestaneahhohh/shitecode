using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public UIDocument UIDocument;
    [SerializeField]
    private GunDB guns;

    //buttons
    public Button startButton;
    public Button loadoutButton;
    public Button settingsButton;
    public Button quitButton;

    //panels
    public VisualElement mainBG;
    public VisualElement mainPanel;
    public VisualElement settingsPanel;
    public VisualElement loadoutPanel;
    public VisualElement gunsContainer;

    //inputs
    public RadioButtonGroup qualitySelector;
    public Slider lookSensitivity;
    public Slider aimSensitivity;

    private void Start()
    {
        var root = UIDocument.rootVisualElement;

        //buttons
        startButton = root.Q<Button>("start");
        loadoutButton = root.Q<Button>("loadoutButton");
        settingsButton = root.Q<Button>("mainSettings");
        quitButton = root.Q<Button>("quit");

        //panels
        mainBG = root.Q<VisualElement>("background");
        mainPanel = root.Q<VisualElement>("mainPanel");
        settingsPanel = root.Q<VisualElement>("mainSettingsPanel");
        loadoutPanel = root.Q<VisualElement>("loadoutPanel");
        gunsContainer = root.Q<VisualElement>("gunsContainer");

        //inputs
        qualitySelector = root.Q<RadioButtonGroup>("qualitySelector");
        lookSensitivity = root.Q<Slider>("lookSensitivity");
        aimSensitivity = root.Q<Slider>("aimSensitivity");

        //bindings
        startButton.clicked += StartButtonPressed;
        loadoutButton.clicked += LoadoutButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
        quitButton.clicked += QuitButtonPressed;

        mainPanel.style.display = DisplayStyle.Flex;
        settingsPanel.style.display = DisplayStyle.None;
        loadoutPanel.style.display = DisplayStyle.None;

        PlayerPrefs.SetString("currentGun", "null");

        StartCoroutine(AddEventListeners());
    }

    private void Update()
    {
        if (settingsPanel.style.display == DisplayStyle.Flex)
        {
            QualitySettings.SetQualityLevel(qualitySelector.value);
        }

        if (settingsPanel.style.display == DisplayStyle.Flex && Input.GetMouseButtonDown(1))
        {
            mainPanel.style.display = DisplayStyle.Flex;
            settingsPanel.style.display = DisplayStyle.None;
        }

        if (loadoutPanel.style.display == DisplayStyle.Flex && Input.GetMouseButtonDown(1))
        {
            mainPanel.style.display = DisplayStyle.Flex;
            loadoutPanel.style.display = DisplayStyle.None;
        }
    }


    private void StartButtonPressed()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadoutButtonPressed()
    {
        mainPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.None;
        loadoutPanel.style.display = DisplayStyle.Flex;
    }

    private void SettingsButtonPressed()
    {
        mainPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;
        loadoutPanel.style.display = DisplayStyle.None;

        qualitySelector.value = QualitySettings.GetQualityLevel();
    }

    private void QuitButtonPressed()
    {
        Application.Quit();
    }

    IEnumerator AddEventListeners()
    {
        yield return StartCoroutine(GenerateGunList());

        var root = UIDocument.rootVisualElement;

        var items = root.Q<VisualElement>("gunsContainer").Children();

        foreach (Button item in items)
        {
            item.clicked += () => PlayerPrefs.SetString("currentGun", item.text);
        }
    }

    IEnumerator GenerateGunList()
    {
        foreach (GunData gun in guns.DB)
        {
            var item = new Button();
            item.AddToClassList("loadoutItem");
            item.text = gun.name;

            gunsContainer.Add(item);
        }

        yield return null;
    }
}
