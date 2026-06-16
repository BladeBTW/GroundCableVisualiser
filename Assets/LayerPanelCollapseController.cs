using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class LayerPanelCollapseController : MonoBehaviour
{
    [Header("Main Panel")]
    public RectTransform panelRect;

    [Header("Panel Widths")]
    public float expandedWidth = 300f;
    public float collapsedWidth = 120f;

    [Header("Collapse Button")]
    public Button collapseButton;

    [Header("Texts To Hide When Collapsed")]
    public TMP_Text[] textObjectsToHide;

    [Header("Objects To Hide When Collapsed")]
    public GameObject[] objectsToHide;

    [Header("Layer Button Layout")]
    public RectTransform[] layerButtons;
    public float expandedButtonLeft = 12f;
    public float expandedButtonRight = 12f;
    public float collapsedButtonLeft = 8f;
    public float collapsedButtonRight = 8f;

    [Header("Left Icons")]
    public RectTransform[] leftIcons;
    public Vector2 expandedLeftIconPosition = new Vector2(22f, 0f);
    public Vector2 collapsedLeftIconPosition = new Vector2(18f, 0f);

    [Header("Eye Icons")]
    public RectTransform[] eyeIcons;
    public Vector2 expandedEyeIconPosition = new Vector2(-22f, 0f);
    public Vector2 collapsedEyeIconPosition = new Vector2(-18f, 0f);

    [Header("Reset Buttons")]
    public RectTransform[] resetButtons;
    public float expandedResetLeft = 12f;
    public float expandedResetRight = 12f;
    public float collapsedResetLeft = 8f;
    public float collapsedResetRight = 8f;

    [Header("State")]
    public bool startCollapsed = false;

    [SerializeField]
    private bool isCollapsed;

    private void Awake()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        isCollapsed = startCollapsed;

        if (collapseButton != null)
        {
            collapseButton.onClick.RemoveAllListeners();
            collapseButton.onClick.AddListener(TogglePanel);
        }
        else
        {
            Debug.LogError("Collapse Button is not assigned.");
        }

        ApplyState();
    }

    private void OnValidate()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }

        if (!Application.isPlaying)
        {
            isCollapsed = startCollapsed;
            ApplyState();
        }
    }

    public void TogglePanel()
    {
        isCollapsed = !isCollapsed;
        ApplyState();
    }

    public void ExpandPanel()
    {
        isCollapsed = false;
        ApplyState();
    }

    public void CollapsePanel()
    {
        isCollapsed = true;
        ApplyState();
    }

    private void ApplyState()
    {
        SetPanelWidth();
        SetTextVisibility();
        SetObjectVisibility();
        SetLayerButtonWidths();
        SetIconPositions();
        SetResetButtonWidths();

        if (panelRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        }
    }

    private void SetPanelWidth()
    {
        if (panelRect == null)
        {
            return;
        }

        float width = isCollapsed ? collapsedWidth : expandedWidth;

        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 0.5f);

        panelRect.offsetMin = new Vector2(0f, panelRect.offsetMin.y);
        panelRect.offsetMax = new Vector2(width, panelRect.offsetMax.y);
    }

    private void SetTextVisibility()
    {
        foreach (TMP_Text textObject in textObjectsToHide)
        {
            if (textObject != null)
            {
                textObject.gameObject.SetActive(!isCollapsed);
            }
        }
    }

    private void SetObjectVisibility()
    {
        foreach (GameObject objectToHide in objectsToHide)
        {
            if (objectToHide != null)
            {
                objectToHide.SetActive(!isCollapsed);
            }
        }
    }

    private void SetLayerButtonWidths()
    {
        foreach (RectTransform layerButton in layerButtons)
        {
            if (layerButton != null)
            {
                SetHorizontalOffsets(
                    layerButton,
                    isCollapsed ? collapsedButtonLeft : expandedButtonLeft,
                    isCollapsed ? collapsedButtonRight : expandedButtonRight
                );
            }
        }
    }

    private void SetIconPositions()
    {
        foreach (RectTransform leftIcon in leftIcons)
        {
            if (leftIcon != null)
            {
                leftIcon.anchoredPosition = isCollapsed
                    ? collapsedLeftIconPosition
                    : expandedLeftIconPosition;
            }
        }

        foreach (RectTransform eyeIcon in eyeIcons)
        {
            if (eyeIcon != null)
            {
                eyeIcon.anchoredPosition = isCollapsed
                    ? collapsedEyeIconPosition
                    : expandedEyeIconPosition;
            }
        }
    }

    private void SetResetButtonWidths()
    {
        foreach (RectTransform resetButton in resetButtons)
        {
            if (resetButton != null)
            {
                SetHorizontalOffsets(
                    resetButton,
                    isCollapsed ? collapsedResetLeft : expandedResetLeft,
                    isCollapsed ? collapsedResetRight : expandedResetRight
                );
            }
        }
    }

    private void SetHorizontalOffsets(RectTransform rectTransform, float left, float right)
    {
        rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
    }
}