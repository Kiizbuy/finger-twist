using DG.Tweening;
using System;
using UnityEngine;

namespace GameFramework.InteractableSystem
{
    public enum DoorState
    {
        Open = 0,
        Closed = 1,
        Locked = 2
    }

    public enum OpenType
    {
        Move,
        Rotate,
    }

    public class Door : MonoBehaviour, IInteractableObject
    {
        [SerializeField] private DoorState _doorState;
        [SerializeField] private OpenType _openType;
        [SerializeField] private Transform _doorPoint;

        [SerializeField] private float _transformDuration = 5f;

        //TODO - Move to SO settings
        [SerializeField] private Vector3 _startOpenPoint;
        [SerializeField] private Vector3 _endOpenPoint;

        [SerializeField] private Vector3 _rotationAxis;
        [SerializeField, Range(-180, 180)] private float _startRotationAngle;
        [SerializeField, Range(-180, 180)] private float _endRotationAngle;


        private Vector3 _startOpenPointWorld;
        private Vector3 _endOpenPointWorld;


        private void Start()
        {
            if (_doorPoint == null)
            {
                Debug.LogError("_door point field is null", gameObject);
                return;
            }

            _startOpenPointWorld = _doorPoint.position + _doorPoint.TransformVector(_startOpenPoint);
            _endOpenPointWorld = _doorPoint.position + _doorPoint.TransformVector(_endOpenPoint);

        }

        public void LockDoor() => _doorState = DoorState.Locked;
        public void UnlockDoor() => _doorState = DoorState.Closed;

        public bool InteractRequirements()
        {
            return _doorState != DoorState.Locked;
        }

        public void Interact()
        {
            if (_doorState == DoorState.Locked)
                return;
            _doorState = _doorState == DoorState.Open ? DoorState.Closed : DoorState.Open;

            switch (_doorState)
            {
                case DoorState.Open:
                    MoveDoor(_endOpenPointWorld, _endRotationAngle);
                    break;
                case DoorState.Closed:
                    MoveDoor(_startOpenPointWorld, _startRotationAngle);
                    break;
                case DoorState.Locked:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void MoveDoor(Vector3 endPosition, float rotationAngle)
        {
            var doorQuaternion = Quaternion.AngleAxis(rotationAngle, _rotationAxis);

            switch (_openType)
            {
                case OpenType.Move:
                    _doorPoint.DOMove(endPosition, _transformDuration);
                    break;
                case OpenType.Rotate:
                    _doorPoint.DOLocalRotateQuaternion(doorQuaternion, _transformDuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
