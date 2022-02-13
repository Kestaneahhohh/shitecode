using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public CharacterController characterController;

    public GameObject player;

    public UIDocument UIDocument;

    //buttons
    public Button resumeButton, respawnButton, settingsButton, quitButton;

    //panels
    public VisualElement mainBG, HUD, mainPanel, settingsPanel;

    //inputs
    public RadioButtonGroup qualitySelector;
    public FloatField lookSensitivity, aimSensitivity;

    //HUD bullshit
    public Label currentAmmo, reserveAmmo;
    public VisualElement health;

    bool paused;

    private void Start()
    {
        paused = true;
        player.GetComponent<Framework>().paused = true;

        var root = UIDocument.rootVisualElement;

        //buttons
        resumeButton = root.Q<Button>("resume");
        respawnButton = root.Q<Button>("respawn");
        settingsButton = root.Q<Button>("settings");
        quitButton = root.Q<Button>("quit");

        //panels
        mainBG = root.Q<VisualElement>("background");
        HUD = root.Q<VisualElement>("HUD");
        mainPanel = root.Q<VisualElement>("mainPanel");
        settingsPanel = root.Q<VisualElement>("settingsPanel");

        //inputs
        qualitySelector = root.Q<RadioButtonGroup>("qualitySelector");
        lookSensitivity = root.Q<FloatField>("lookSensitivity");
        aimSensitivity = root.Q<FloatField>("aimSensitivity");

        //bindings
        resumeButton.clicked += ResumeButtonPressed;
        respawnButton.clicked += RespawnButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
        quitButton.clicked += QuitButtonPressed;

        //HUD bullshit
        currentAmmo = root.Q<Label>("currentAmmo");
        reserveAmmo = root.Q<Label>("reserveAmmo");
        health = root.Q<VisualElement>("health");
    }

    private void Update()
    {
        if (!paused)
        {
            Resume();

            currentAmmo.text = player.GetComponent<Framework>().currentAmmo.ToString();
            reserveAmmo.text = player.GetComponent<Framework>().reserveAmmo.ToString();
            health.style.width = Length.Percent(player.GetComponent<Framework>().health);
        } 
        else
        {
            Pause();
        }

        if (!paused && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        } 
        else if (paused && Input.GetKeyDown(KeyCode.Escape)) {
            Resume();
        }

        if (paused && settingsPanel.style.display == DisplayStyle.Flex && Input.GetMouseButtonDown(1))
        {
            mainPanel.style.display = DisplayStyle.Flex;
            settingsPanel.style.display = DisplayStyle.None;
        }

        if (paused && settingsPanel.style.display == DisplayStyle.Flex)
        {
            player.GetComponent<Framework>().mouseSensitivity = lookSensitivity.value;
            player.GetComponent<Framework>().scopedSensitivity = aimSensitivity.value;
            
            QualitySettings.SetQualityLevel(qualitySelector.value);
        }
    }

    private void Pause()
    {
        paused = true;
        player.GetComponent<Framework>().paused = true;

        HUD.style.display = DisplayStyle.None;
        mainPanel.style.display = DisplayStyle.Flex;

        player.GetComponent<Framework>().move = Vector3.zero;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    private void Resume()
    {
        paused = false;
        player.GetComponent<Framework>().paused = false;

        HUD.style.display = DisplayStyle.Flex;
        mainPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void ResumeButtonPressed()
    {
        Resume();
    }

    private void RespawnButtonPressed()
    {
        characterController.enabled = false;
        player.transform.position = Vector3.zero;
        characterController.enabled = true;
        Resume();
    }

    private void SettingsButtonPressed()
    {
        mainPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;

        lookSensitivity.value = player.GetComponent<Framework>().mouseSensitivity;
        aimSensitivity.value = player.GetComponent < Framework>().scopedSensitivity;

        qualitySelector.value = QualitySettings.GetQualityLevel();
    }

    private void QuitButtonPressed()
    {
        SceneManager.LoadScene(0);
    }
}
