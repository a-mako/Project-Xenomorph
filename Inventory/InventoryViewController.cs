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
    [SerializeField] private GameObject currentButton;
    [SerializeField] private GameObject playerMenuHead;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private ItemSlot currentSlot;
    [SerializeField] private ScreenFader fader;
    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private List<Button> contextMenuIgnore;
    [SerializeField] private List<Button> contextMenuButtons;
    private Button itemSlotButton;
    private Color itemSlotButtonDisabledColor = new Color(0.784f, 0.784f, 0.784f, 0.502f);

    private enum State {
        menuClosed,
        menuOpen,
        contextMenu
    }

    private State state;

    public void OpenMenu() {
        EventBus.Instance.PauseGameplay();
        EventBus.Instance.OpenInventory();
        fader.FadeToBlack(0.3f, FadeToMenuCallback);
        playerMenuHead.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(itemSlots[0].gameObject);
        OnSlotSelected(itemSlots[0]);
        state = State.menuOpen;
    }
    public void CloseMenu() {
        EventBus.Instance.CloseInventory();
        Cursor.visible = false;
        fader.FadeToBlack(0.3f, FadeFromMenuCallback);
        playerMenuHead.gameObject.SetActive(false);
        state = State.menuClosed;
        currentSlot = null;
        itemNameText.SetText("");
        itemDescriptionText.SetText("");
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenContextMenu() {
        if (EventSystem.current.currentSelectedGameObject.TryGetComponent<ItemSlot> (out var slot)) {
            if (currentSlot.itemData != null) {
                state = State.contextMenu;
                contextMenuObject.SetActive(true);
                ButtonColorToSelected(Color.red);
                foreach (var button in contextMenuIgnore) {
                    button.interactable = false;
                }
                EnableDisableContextMenuOptions();
            }
        }
    }

    private void EnableDisableContextMenuOptions()
    {
        foreach (var button in contextMenuButtons) {
            button.interactable = true;
        }
        if (currentSlot.itemData.Type == ItemData.ItemType.Consumable) {
            foreach(var button in contextMenuButtons) {
                if (button.name == "Equip Button") {
                    button.interactable = false;
                }
            }
        }
        else if (currentSlot.itemData.Type == ItemData.ItemType.Weapon) {
            foreach(var button in contextMenuButtons) {
                if (button.name == "Use Button") {
                    button.interactable = false;
                }
                if (button.name == "Discard Button") {
                    button.interactable = false;
                }
            }
        }
        else if (currentSlot.itemData.Type == ItemData.ItemType.KeyItem) {
            foreach(var button in contextMenuButtons) {
                if (button.name == "Equip Button") {
                    button.interactable = false;
                }
                if (button.name == "Discard Button") {
                    button.interactable = false;
                }
            }
        }
        foreach (var button in contextMenuButtons) {
            if (button.interactable) {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                break;
            }
        }
    }

    public void CloseContextMenu()
    {
        contextMenuObject.SetActive(false);
        foreach (var button in contextMenuIgnore) {
                button.interactable = true;
        }
        EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
        ButtonColorToUnselected(itemSlotButtonDisabledColor);
        state = State.menuOpen;
    }

    public void OnSlotSelected(ItemSlot selectedSlot) {
        currentSlot = selectedSlot;
        itemSlotButton = currentSlot.gameObject.GetComponent<Button>();
        if (selectedSlot.itemData == null) {
            itemNameText.ClearMesh();
            itemDescriptionText.ClearMesh();
            return;
        }
        itemNameText.SetText(selectedSlot.itemData.Name);
        itemDescriptionText.SetText(selectedSlot.itemData.Description[0]);
    }

    public void UseItem() {
        fader.FadeToBlack(1f, FadeToUseItemCallback);
        
    }
    public void FadeToUseItemCallback() {
        EventBus.Instance.UseItem(currentSlot.itemData);
        EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
        if (currentSlot.itemData.Type == ItemData.ItemType.Consumable) {
            currentSlot.itemData = null;
        }
        CloseContextMenu();
        CloseMenu();
        ButtonColorToUnselected(itemSlotButtonDisabledColor);
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

    private void CheckMouseOrKeyboardInput() {
        if (Input.GetButtonDown("Vertical") || Input.GetButtonDown("Horizontal")) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (Input.GetButtonDown("Submit") && currentButton.name == "Esc Button") {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
                CloseContextMenu();
            }
        }

        else if (state == State.menuOpen) {
            CheckMouseOrKeyboardInput();
            if (EventSystem.current.currentSelectedGameObject) {
                currentButton = EventSystem.current.currentSelectedGameObject;
            }
            if (Input.GetButtonDown("Submit")) {
                if (EventSystem.current.currentSelectedGameObject == null) {
                    EventSystem.current.SetSelectedGameObject(currentButton);
                }
                if (currentButton.name == "Esc Button") {
                    CloseMenu();
                }
                OpenContextMenu();
            }
            else if (Input.GetButtonDown("Cancel")) {
                if (state == State.menuOpen) {
                    CloseMenu();

                }
                else if (state == State.contextMenu) {
                    CloseContextMenu();
                }
            }
        }

        else if (state == State.contextMenu) {
            CheckMouseOrKeyboardInput();
            if (EventSystem.current.currentSelectedGameObject) {
                currentButton = EventSystem.current.currentSelectedGameObject;
            }
            if (Input.GetButtonDown("Submit")) {
                if (currentButton.name == "Use Button") {
                    UseItem();
                }
            }
        }
    }
}
