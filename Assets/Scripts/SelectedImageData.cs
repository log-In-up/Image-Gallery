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
        private string _url;
        private Sprite _selectedSprite = null;
        #endregion

        #region Public Methods
        internal void SendImageData(Sprite sprite = null, string url = null)
        {
            _selectedSprite = sprite;
            _url = url;
        }

        internal Tuple<string, Sprite> GetSelectedSprite() => Tuple.Create(_url, _selectedSprite);
        #endregion
    }
}