using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Button button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable) {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Cursor.lockState == CursorLockMode.Confined) {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Awake() {
        button = this.gameObject.GetComponent<Button>();
    }
}