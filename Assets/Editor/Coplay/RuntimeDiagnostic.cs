using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;

public class RuntimeDiagnostic
{
    public static string Execute()
    {
        EditorSceneManager.OpenScene("Assets/Main Garden/Scenes/GardenScene.unity", OpenSceneMode.Single);
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        string result = "";

        // Find SettingsCanvas
        GameObject canvasGO = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "SettingsCanvas")
            {
                canvasGO = root;
                break;
            }
        }

        if (canvasGO == null)
        {
            return "No SettingsCanvas found!";
        }

        // Temporarily enable it to inspect
        canvasGO.SetActive(true);

        // Check Canvas component
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        result += $"Canvas: renderMode={canvas.renderMode}, sortingOrder={canvas.sortingOrder}\n";

        // Check GraphicRaycaster
        GraphicRaycaster raycaster = canvasGO.GetComponent<GraphicRaycaster>();
        result += $"GraphicRaycaster: {(raycaster != null ? "present" : "MISSING!")}\n";

        // Check EventSystem in scene
        EventSystem es = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            es = root.GetComponent<EventSystem>();
            if (es != null) break;
            es = root.GetComponentInChildren<EventSystem>(true);
            if (es != null) break;
        }
        result += $"EventSystem: {(es != null ? $"found on '{es.gameObject.name}'" : "MISSING!")}\n";

        // Check all buttons
        Button[] buttons = canvasGO.GetComponentsInChildren<Button>(true);
        result += $"\nButtons ({buttons.Length}):\n";
        foreach (var btn in buttons)
        {
            Image img = btn.GetComponent<Image>();
            RectTransform rt = btn.GetComponent<RectTransform>();
            result += $"  '{btn.name}':\n";
            result += $"    active={btn.gameObject.activeSelf}, interactable={btn.interactable}\n";
            result += $"    targetGraphic={(btn.targetGraphic != null ? btn.targetGraphic.GetType().Name + " on " + btn.targetGraphic.gameObject.name : "NULL")}\n";
            result += $"    image: sprite={(img?.sprite != null ? img.sprite.name : "none")}, type={img?.type}, raycastTarget={img?.raycastTarget}, color={img?.color}\n";
            result += $"    rect: size={rt.sizeDelta}, anchoredPos={rt.anchoredPosition}\n";
            result += $"    onClick listeners={btn.onClick.GetPersistentEventCount()} (persistent)\n";
            
            // Check if any parent is blocking
            CanvasGroup cg = btn.GetComponentInParent<CanvasGroup>();
            if (cg != null)
            {
                result += $"    CanvasGroup: interactable={cg.interactable}, blocksRaycasts={cg.blocksRaycasts}\n";
            }
        }

        // Check all sliders
        Slider[] sliders = canvasGO.GetComponentsInChildren<Slider>(true);
        result += $"\nSliders ({sliders.Length}):\n";
        foreach (var slider in sliders)
        {
            RectTransform rt = slider.GetComponent<RectTransform>();
            result += $"  '{slider.name}':\n";
            result += $"    active={slider.gameObject.activeSelf}, interactable={slider.interactable}\n";
            result += $"    fillRect={(slider.fillRect != null ? slider.fillRect.name : "NULL")}\n";
            result += $"    handleRect={(slider.handleRect != null ? slider.handleRect.name : "NULL")}\n";
            result += $"    targetGraphic={(slider.targetGraphic != null ? slider.targetGraphic.gameObject.name : "NULL")}\n";
            result += $"    rect: size={rt.sizeDelta}, anchoredPos={rt.anchoredPosition}\n";
            
            // Check handle image
            if (slider.handleRect != null)
            {
                Image handleImg = slider.handleRect.GetComponent<Image>();
                result += $"    handle image: raycastTarget={handleImg?.raycastTarget}, size={slider.handleRect.sizeDelta}\n";
            }
        }

        // Check overlay - is it blocking everything?
        Transform overlay = canvasGO.transform.Find("Overlay");
        if (overlay != null)
        {
            Image overlayImg = overlay.GetComponent<Image>();
            result += $"\nOverlay: raycastTarget={overlayImg?.raycastTarget}, siblingIndex={overlay.GetSiblingIndex()}\n";
        }

        // Check panel
        Transform panel = canvasGO.transform.Find("SettingsPanel");
        if (panel != null)
        {
            Image panelImg = panel.GetComponent<Image>();
            result += $"Panel: raycastTarget={panelImg?.raycastTarget}, siblingIndex={panel.GetSiblingIndex()}\n";
        }

        // Check sibling order (overlay should be BEFORE panel)
        result += $"\nChild order of SettingsCanvas:\n";
        for (int i = 0; i < canvasGO.transform.childCount; i++)
        {
            var child = canvasGO.transform.GetChild(i);
            result += $"  [{i}] '{child.name}'\n";
        }

        // Disable canvas again
        canvasGO.SetActive(false);
        
        return result;
    }
}
