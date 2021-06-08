using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GameFramework.Components
{
    public class TaggableObject : MonoBehaviour
    {
        [SerializeField, Tag]
        private List<string> _tags = new List<string>();

        public bool HaveTag(string tagName) => _tags.Contains(tagName);

        public void AddNewTag(string tagName)
        {
            if (!_tags.Contains(tagName))
                _tags.Add(tagName);
        }

        public void RemoveTag(string tagName)
        {
            if (_tags.Contains(tagName))
                _tags.Remove(tagName);
        }
    }
}

