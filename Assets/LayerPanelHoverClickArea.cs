using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayerPanelHoverClickArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public LayerPanelCollapseController collapseController;
    public Image panelBackground;

    [Header("Panel Hover Colors")]
    public Color normalColor = new Color(0.55f, 0.55f, 0.55f, 0.65f);
    public Color hoverColor = new Color(0.65f, 0.65f, 0.65f, 0.75f);

    [Header("Click Behavior")]
    public bool ignoreClicksOnChildButtons = true;

    private void Awake()
    {
        if (panelBackground == null)
        {
            panelBackground = GetComponent<Image>();
        }

        if (collapseController == null)
        {
            collapseController = GetComponent<LayerPanelCollapseController>();
        }

        ApplyNormalColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyHoverColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyNormalColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (collapseController == null)
        {
            return;
        }

        if (ignoreClicksOnChildButtons && PointerIsOnChildButton(eventData))
        {
            return;
        }

        collapseController.TogglePanel();
    }

    private bool PointerIsOnChildButton(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerPress == null)
        {
            return false;
        }

        Button clickedButton = eventData.pointerPress.GetComponentInParent<Button>();

        if (clickedButton == null)
        {
            return false;
        }

        // If the clicked button is somewhere inside this panel,
        // let that button handle the click instead of collapsing the panel.
        return clickedButton.transform.IsChildOf(transform);
    }

    private void ApplyNormalColor()
    {
        if (panelBackground != null)
        {
            panelBackground.color = normalColor;
        }
    }

    private void ApplyHoverColor()
    {
        if (panelBackground != null)
        {
            panelBackground.color = hoverColor;
        }
    }
}