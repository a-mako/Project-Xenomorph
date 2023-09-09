using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewSelector : MonoBehaviour
{
    [SerializeField] private float speed = 35f;
    private RectTransform rectTransform;
    private GameObject selected;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }
    // Update is called once per frame
    private void Update()
    {
        var selectedGameObject = EventSystem.current.currentSelectedGameObject;
        selected = selectedGameObject ?? selected;
        EventSystem.current.SetSelectedGameObject(selected);

        if (selected == null) return;

        transform.position = Vector3.Lerp(transform.position, selected.transform.position, speed * Time.deltaTime);

        var otherRect = selected.GetComponent<RectTransform>();
        
        var horizontalLerp = Mathf.Lerp(rectTransform.rect.size.x, otherRect.rect.size.x, speed * Time.deltaTime);
        var verticalLerp = Mathf.Lerp(rectTransform.rect.size.y, otherRect.rect.size.y, speed * Time.deltaTime);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalLerp);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalLerp);
    }
}
