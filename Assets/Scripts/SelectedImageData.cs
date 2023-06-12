using System;
using UnityEngine;

namespace Data
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class SelectedImageData : MonoBehaviour
    {
        #region Fields
        private string _url, _name;
        private Sprite _selectedSprite = null;
        #endregion

        #region Public Methods
        internal void SendImageData(string name, string url, Sprite sprite = null)
        {
            _name = name;
            _url = url;
            _selectedSprite = sprite;
        }

        internal Tuple<string, string, Sprite> GetSelectedSprite() => Tuple.Create(_url, _name, _selectedSprite);
        #endregion
    }
}