using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int lives = 3;
    public int nextUpgradeScore = 1500;

    private Text scoreText;
    private Text livesText;
    private GameObject upgradePanel;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateUI();
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();

        if (score >= nextUpgradeScore)
        {
            nextUpgradeScore += 1500;
            TriggerUpgrade();
        }
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI();
        
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.DamageGlitch();
        }

        if (lives <= 0)
        {
            isGameOver = true;
            Invoke("RestartScene", 1f);
        }
    }

    private void RestartScene()
    {
        Instance = null;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void TriggerUpgrade()
    {
        Time.timeScale = 0f;
        if (upgradePanel != null) upgradePanel.SetActive(true);
    }

    public void SelectUpgrade(WeaponType type)
    {
        Time.timeScale = 1f;
        if (upgradePanel != null) upgradePanel.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerShooting ps = player.GetComponent<PlayerShooting>();
            if (ps != null) ps.currentWeapon = type;

            PlayerWeaponVisuals pwv = player.GetComponent<PlayerWeaponVisuals>();
            if (pwv != null) pwv.UpdateVisuals(type);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score.ToString("D4");
        if (livesText != null)
            livesText.text = "LIVES: " + lives;
    }

    private void CreateUI()
    {
        // Ensure EventSystem exists for UI interaction
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Score - top left
        scoreText = CreateText(canvasObj.transform, "ScoreText",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(30, -30), new Color(0f, 1f, 1f, 1f));

        // Lives - top right
        livesText = CreateText(canvasObj.transform, "LivesText",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-30, -30), new Color(1f, 0.3f, 0.3f, 1f));
        livesText.alignment = TextAnchor.UpperRight;

        CreateUpgradeUI(canvasObj.transform);
    }

    private void CreateUpgradeUI(Transform canvasTransform)
    {
        // Panel Background
        upgradePanel = new GameObject("UpgradePanel");
        upgradePanel.transform.SetParent(canvasTransform, false);
        RectTransform rect = upgradePanel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        Image bg = upgradePanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f);

        // Title
        Text title = CreateText(upgradePanel.transform, "Title", 
            new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.5f), 
            Vector2.zero, Color.yellow);
        title.text = "LEVEL UP!\nCHOOSE WEAPON";
        title.alignment = TextAnchor.MiddleCenter;

        // Buttons
        CreateUpgradeButton("Double Shot", new Vector2(0.5f, 0.6f), WeaponType.DoubleShot);
        CreateUpgradeButton("Spread Shot", new Vector2(0.5f, 0.4f), WeaponType.SpreadShot);
        CreateUpgradeButton("Tail Gun", new Vector2(0.5f, 0.2f), WeaponType.TailGun);

        upgradePanel.SetActive(false);
    }

    private void CreateUpgradeButton(string label, Vector2 anchor, WeaponType type)
    {
        GameObject btnObj = new GameObject("Btn_" + label);
        btnObj.transform.SetParent(upgradePanel.transform, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300, 80);

        Image bg = btnObj.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() => SelectUpgrade(type));

        Text text = CreateText(btnObj.transform, "Text", 
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 
            Vector2.zero, Color.white);
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 80);
    }

    private Text CreateText(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 60);

        Text text = obj.AddComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 32);
        text.fontSize = 36;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }
}
