using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public UIDocument UIDocument;

    //buttons
    public Button startButton;
    public Button respawnButton;
    public Button settingsButton;
    public Button quitButton;

    //panels
    public VisualElement mainBG;
    public VisualElement mainPanel;
    public VisualElement settingsPanel;

    //inputs
    public RadioButtonGroup qualitySelector;
    public Slider lookSensitivity;
    public Slider aimSensitivity;

    private void Start()
    {
        var root = UIDocument.rootVisualElement;

        //buttons
        startButton = root.Q<Button>("start");
        settingsButton = root.Q<Button>("mainSettings");
        quitButton = root.Q<Button>("quit");

        //panels
        mainBG = root.Q<VisualElement>("background");
        mainPanel = root.Q<VisualElement>("mainPanel");
        settingsPanel = root.Q<VisualElement>("mainSettingsPanel");

        //inputs
        qualitySelector = root.Q<RadioButtonGroup>("qualitySelector");
        lookSensitivity = root.Q<Slider>("lookSensitivity");
        aimSensitivity = root.Q<Slider>("aimSensitivity");

        //bindings
        startButton.clicked += StartButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
        quitButton.clicked += QuitButtonPressed;

        settingsPanel.style.display = DisplayStyle.None;
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
    }


    private void StartButtonPressed()
    {
        SceneManager.LoadScene(1);
    }

    private void SettingsButtonPressed()
    {
        mainPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;

        qualitySelector.value = QualitySettings.GetQualityLevel();
    }

    private void QuitButtonPressed()
    {
        Application.Quit();
    }
}
