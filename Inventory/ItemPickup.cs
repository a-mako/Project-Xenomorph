using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private void OnTriggerStay(Collider other) {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKey(KeyCode.E)) {
            EventBus.Instance.PickUpItem(itemData);
            Destroy(gameObject);
        }
    }
}
