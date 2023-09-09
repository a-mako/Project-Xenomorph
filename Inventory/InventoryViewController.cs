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
    [SerializeField] private ViewSelector selector;
    [SerializeField] private GameObject inventoryViewObject;
    [SerializeField] private GameObject contextMenuObject;
    [SerializeField] private GameObject firstContextMenuOption;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private ItemSlot currentSlot;
    [SerializeField] private ScreenFader fader;
    [SerializeField] private List<Button> contextMenuIgnore;

    private enum State {
        menuClosed,
        menuOpen,
        contextMenu
    }

    private State state;

    public void UseItem() {
        fader.FadeToBlack(1f, FadeToUseItemCallback);
        
    }
    public void FadeToUseItemCallback() {
        contextMenuObject.SetActive(false);
        inventoryViewObject.SetActive(false);
        FadeFromMenuCallback();
        EventBus.Instance.UseItem(currentSlot.itemData);
        EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
        if (currentSlot.itemData.Type == ItemData.ItemType.Consumable) {
            currentSlot.itemData = null;
        }
        state = State.menuClosed;
        EventSystem.current.SetSelectedGameObject(null);
        foreach (var button in contextMenuIgnore) {
                button.interactable = true;
        }
    }
    public void OnSlotSelected(ItemSlot selectedSlot) {
        selector.gameObject.SetActive(true);
        currentSlot = selectedSlot;
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
                selector.gameObject.SetActive(false);
                EventBus.Instance.PauseGameplay();
                fader.FadeToBlack(0.3f, FadeToMenuCallback);
                state = State.menuOpen;
                EventSystem.current.SetSelectedGameObject(null);
            }
            else if (state == State.menuOpen) {
                fader.FadeToBlack(0.3f, FadeFromMenuCallback);
                state = State.menuClosed;
                currentSlot = null;
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
        if (Input.GetKeyDown(KeyCode.E)) {
            if (state == State.menuOpen) {
                if (EventSystem.current.currentSelectedGameObject.TryGetComponent<ItemSlot> (out var slot)) {
                    state = State.contextMenu;
                    contextMenuObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(firstContextMenuOption);
                    foreach (var button in contextMenuIgnore) {
                        button.interactable = false;
                    }
                }
            }
        }
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
