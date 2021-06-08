using UnityEngine;

namespace GameFramework.AI
{


    public class SeeSensorTest : MonoBehaviour
    {
        public Transform NearestTransform;
        public LayerMask LayerMask = ~0;
        private ISeeSensor _seeSensor;

        void Start()
        {
            _seeSensor = GetComponent<ISeeSensor>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_seeSensor.SeeTarget(LayerMask, out var seenTransform))
                NearestTransform = seenTransform;
        }
    }
}
