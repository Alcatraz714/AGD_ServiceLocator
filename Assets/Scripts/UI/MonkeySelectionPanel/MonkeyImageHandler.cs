using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ServiceLocator.UI
{
    public class MonkeyImageHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private Image monkeyImage;
        private MonkeyCellController owner;
        private Sprite spriteToSet;
        private RectTransform rectTransform;
        private Vector3 originalPosition;
        private Vector3 originalAnchoredPosition;

        public void ConfigureImageHandler(Sprite spriteToSet, MonkeyCellController owner)
        {
            this.spriteToSet = spriteToSet;
            this.owner = owner;
        }
        private void Awake()
        {
            monkeyImage = GetComponent<Image>();
            monkeyImage.sprite = spriteToSet;
            rectTransform = GetComponent<RectTransform>();
            originalPosition = rectTransform.position;
            originalAnchoredPosition = rectTransform.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta;
            owner.MonkeyDraggedAt(rectTransform.position); // check for valid placement
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ResetMonkey();
            owner.MonkeyDroppedAt(eventData.position); // if the drop was correct
        }

        private void ResetMonkey()
        {
            // reset to early saved positions
            monkeyImage.color = new Color(1, 1, 1, 1f);
            rectTransform.position = originalPosition;
            rectTransform.anchoredPosition = originalAnchoredPosition;
            GetComponent<LayoutElement>().enabled = false;
            GetComponent<LayoutElement>().enabled = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            monkeyImage.color = new Color(1, 1, 1, 0.6f);
        }
    }
}