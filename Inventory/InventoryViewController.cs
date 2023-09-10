using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;

public class InventoryViewController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryViewObject;
    [SerializeField] private GameObject contextMenuObject;
    [SerializeField] private GameObject firstContextMenuOption;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private ItemSlot currentSlot;
    [SerializeField] private ScreenFader fader;
    [SerializeField] private List<Button> contextMenuIgnore;
    private Button itemSlotButton;
    private Color itemSlotButtonDisabledColor;

    private enum State {
        menuClosed,
        menuOpen,
        contextMenu
    }

    private State state;

    public void OpenMenu() {
        EventBus.Instance.PauseGameplay();
        fader.FadeToBlack(0.3f, FadeToMenuCallback);
        EventSystem.current.SetSelectedGameObject(itemSlots[0].gameObject);
        OnSlotSelected(itemSlots[0]);
        state = State.menuOpen;
    }
    public void CloseMenu() {
        fader.FadeToBlack(0.3f, FadeFromMenuCallback);
        state = State.menuClosed;
        currentSlot = null;
        itemNameText.SetText("");
        itemDescriptionText.SetText("");
    }
    public void UseItem() {
        fader.FadeToBlack(1f, FadeToUseItemCallback);
        
    }
    public void FadeToUseItemCallback() {
        contextMenuObject.SetActive(false);
        inventoryViewObject.SetActive(false);
        FadeFromMenuCallback();
        ButtonColorToUnselected(itemSlotButtonDisabledColor);
        EventBus.Instance.UseItem(currentSlot.itemData);
        EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
        if (currentSlot.itemData.Type == ItemData.ItemType.Consumable) {
            currentSlot.itemData = null;
        }
        itemNameText.SetText("");
        itemDescriptionText.SetText("");
        state = State.menuClosed;
        EventSystem.current.SetSelectedGameObject(null);
        foreach (var button in contextMenuIgnore) {
                button.interactable = true;
        }
    }
    public void OnSlotSelected(ItemSlot selectedSlot) {
        currentSlot = selectedSlot;
        itemSlotButton = currentSlot.gameObject.GetComponent<Button>();
        itemSlotButtonDisabledColor = itemSlotButton.colors.disabledColor;
        if (selectedSlot.itemData == null) {
            itemNameText.ClearMesh();
            itemDescriptionText.ClearMesh();
            return;
        }
        itemNameText.SetText(selectedSlot.itemData.Name);
        itemDescriptionText.SetText(selectedSlot.itemData.Description[0]);
    }

    private void OnEnable() {
        EventBus.Instance.onPickUpItem += OnItemPickedUp;
    }
    private void OnDisable() {
        EventBus.Instance.onPickUpItem -= OnItemPickedUp;
    }
    private void OnItemPickedUp(ItemData itemData)
    {
        foreach (var slot in itemSlots) {
            if (slot.isEmpty()) {
                slot.itemData = itemData;
                break;
            }
        }
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (state == State.menuClosed) {
                OpenMenu();
            }
            else if (state == State.menuOpen) {
                CloseMenu();
            }
            else if (state == State.contextMenu) {
                contextMenuObject.SetActive(false);
                foreach (var button in contextMenuIgnore) {
                        button.interactable = true;
                }
                ButtonColorToUnselected(itemSlotButtonDisabledColor);
                EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
                state = State.menuOpen;
            }
        }

        if (Input.GetButtonDown("Cancel")) {
            if (state == State.menuOpen) {
                CloseMenu();

            }
            else if (state == State.contextMenu) {
                contextMenuObject.SetActive(false);
                    foreach (var button in contextMenuIgnore) {
                            button.interactable = true;
                    }
                    EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
                    state = State.menuOpen;
            }
        }

        // Open Context Menu
        if (Input.GetButtonDown("Submit")) {
            if (state == State.menuOpen) {
                if (EventSystem.current.currentSelectedGameObject.TryGetComponent<ItemSlot> (out var slot)) {
                    if (currentSlot.itemData != null) {
                        state = State.contextMenu;
                        contextMenuObject.SetActive(true);
                        ButtonColorToSelected(Color.red);
                        EventSystem.current.SetSelectedGameObject(firstContextMenuOption);
                        foreach (var button in contextMenuIgnore) {
                            button.interactable = false;
                        }
                    }
                }
            }
        }
    }
    private void ButtonColorToSelected(Color color)
    {
        var itemSlotButtonColor = itemSlotButton.colors;
        itemSlotButtonColor.disabledColor = color;
        itemSlotButton.colors = itemSlotButtonColor;
    }
    private void ButtonColorToUnselected(Color color)
    {
        var itemSlotButtonColor = itemSlotButton.colors;
        itemSlotButtonColor.disabledColor = color;
        itemSlotButton.colors = itemSlotButtonColor;
    }
    private void FadeToMenuCallback() {
        inventoryViewObject.SetActive(true);
        fader.FadeFromBlack(0.3f, null);
    }
    private void FadeFromMenuCallback() {
        inventoryViewObject.SetActive(false);
        fader.FadeFromBlack(0.3f, EventBus.Instance.ResumeGameplay);
    }
}
