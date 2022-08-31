using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class ButtonControl: MonoBehaviour, IPointerClickHandler
    {
        [InspectorName("Right Click Events")]
        [SerializeField] private UnityEvent rightClickEvent;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                rightClickEvent?.Invoke();
            }
        }
    }
}