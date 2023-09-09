using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    [SerializeField] public ItemData itemData;
    private InventoryViewController viewController;
    private Image spawnedItemSprite;

    public void OnSelect(BaseEventData eventData) {
        viewController.OnSlotSelected(this);
    }

    public bool isEmpty()
    {
        return itemData == null;
    }

    private void OnEnable() {
        viewController = FindObjectOfType<InventoryViewController>();
        if (itemData == null) return;

        spawnedItemSprite = Instantiate<Image>(itemData.Sprite, transform.position, Quaternion.identity, transform);
    }

    private void OnDisable() {
        if (spawnedItemSprite != null) {
            Destroy(spawnedItemSprite);
        }
    }
}
