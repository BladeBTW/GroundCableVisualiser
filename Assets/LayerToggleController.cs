using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LayerToggleController : MonoBehaviour
{
    [System.Serializable]
    public class LayerToggle
    {
        [Header("Info")]
        public string layerName;
        public string subtitle;

        [Header("Scene Object")]
        public GameObject layerObject;

        [Header("UI")]
        public Button button;
        public TMP_Text titleText;
        public TMP_Text subtitleText;
        public Image eyeIconImage;
        public Image leftIconImage;

        [HideInInspector]
        public bool isVisible = true;
    }

    [Header("Layer Toggles")]
    public LayerToggle[] layers;

    [Header("Reset Button")]
    public Button resetButton;

    [Header("Eye Icon Sprites")]
    public Sprite visibleEyeIcon;
    public Sprite hiddenEyeIcon;

    [Header("UI Colors")]
    public Color visibleTextColor = Color.white;
    public Color hiddenTextColor = new Color(1f, 1f, 1f, 0.35f);

    public Color visibleIconColor = Color.white;
    public Color hiddenIconColor = new Color(1f, 1f, 1f, 0.35f);

    private void Start()
    {
        InitializeLayersAsVisible();
        ConnectButtons();
        ConnectResetButton();
    }

    private void InitializeLayersAsVisible()
    {
        foreach (LayerToggle layer in layers)
        {
            if (layer == null)
            {
                continue;
            }

            layer.isVisible = true;

            if (layer.layerObject != null)
            {
                layer.layerObject.SetActive(true);
            }

            RefreshLayerUI(layer);
        }
    }

    private void ConnectButtons()
    {
        foreach (LayerToggle layer in layers)
        {
            if (layer == null || layer.button == null)
            {
                continue;
            }

            LayerToggle capturedLayer = layer;

            capturedLayer.button.onClick.RemoveAllListeners();
            capturedLayer.button.onClick.AddListener(() =>
            {
                ToggleLayer(capturedLayer);
            });
        }
    }

    private void ConnectResetButton()
    {
        if (resetButton == null)
        {
            return;
        }

        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetAllLayers);
    }

    private void ToggleLayer(LayerToggle layer)
    {
        if (layer == null)
        {
            return;
        }

        layer.isVisible = !layer.isVisible;

        if (layer.layerObject != null)
        {
            layer.layerObject.SetActive(layer.isVisible);
        }

        RefreshLayerUI(layer);
    }

    public void ResetAllLayers()
    {
        foreach (LayerToggle layer in layers)
        {
            if (layer == null)
            {
                continue;
            }

            layer.isVisible = true;

            if (layer.layerObject != null)
            {
                layer.layerObject.SetActive(true);
            }

            RefreshLayerUI(layer);
        }
    }

    private void RefreshLayerUI(LayerToggle layer)
    {
        if (layer == null)
        {
            return;
        }

        Color textColor = layer.isVisible ? visibleTextColor : hiddenTextColor;
        Color iconColor = layer.isVisible ? visibleIconColor : hiddenIconColor;

        if (layer.titleText != null)
        {
            layer.titleText.text = layer.layerName;
            layer.titleText.color = textColor;
        }

        if (layer.subtitleText != null)
        {
            layer.subtitleText.text = layer.subtitle;
            layer.subtitleText.color = textColor;
        }

        if (layer.eyeIconImage != null)
        {
            layer.eyeIconImage.sprite = layer.isVisible ? visibleEyeIcon : hiddenEyeIcon;
            layer.eyeIconImage.color = iconColor;
            layer.eyeIconImage.preserveAspect = true;
            layer.eyeIconImage.raycastTarget = false;
        }

        if (layer.leftIconImage != null)
        {
            layer.leftIconImage.color = iconColor;
            layer.leftIconImage.preserveAspect = true;
            layer.leftIconImage.raycastTarget = false;
        }
    }
}