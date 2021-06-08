using UnityEngine;

namespace GameFramework.Strategy
{
    public interface ISomeStrategy : IStrategyContainer { }

    public class BoxShape : ISomeStrategy
    {
        public int BoxCount;
        public int BoxCount1;
        public int BoxCount2;
        public int BoxCount3;

    }
    public class SphereShape : ISomeStrategy
    {
        [SerializeField] private int _sphereCount;
        [SerializeField] private float[] _spheresArray = new float[50];
    }
    public class CapsuleShape : ISomeStrategy
    {
        [SerializeField] private int _capsuleContCount;
        [SerializeField] private string[] _someStringArray;
    }

    public class StrategyComponentTest : MonoBehaviour
    {
        public int SomeInt;
        public string SomeString;
        public bool SomeBool;
        [SerializeReference, StrategyContainer]
        public ISomeStrategy SomeStrategy;

        public GameObject[] SomeArray;

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                if (SomeStrategy == null)
                    Debug.Log("Chlen is null");
                else
                    Debug.Log(SomeStrategy.GetType().Name);
            }
        }
    }
}