using GameFramework.Strategy;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public sealed class DraggableButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeReference, StrategyContainer] 
        public IMoveButtonCondition MoveButtonConditionSettings;
        
        [SerializeReference, StrategyContainer] 
        public IPlaceButtonCondition PlaceButtonConditionSettings;

        public bool IsPlaced { get; private set; }

        private Vector3 _initialButtonPosition;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(IsPlaced)
                return;
            
            _initialButtonPosition = transform.localPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if( IsPlaced)
                return;
            
            var newPosition = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
            newPosition.z = transform.localPosition.z;
            transform.localPosition = newPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (PlaceButtonConditionSettings.CanPlace(transform.position, vector3 => transform.localPosition = vector3 ))
                IsPlaced = true;
            else
                transform.localPosition = _initialButtonPosition;
        }
    }
}
