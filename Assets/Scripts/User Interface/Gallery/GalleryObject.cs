using Cysharp.Threading.Tasks;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UserInterface
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class GalleryObject : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private Button _button = null;
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private Image _image = null;
        #endregion

        #region Fields
        private bool _imageIsSet;
        private string _downloadUrl;
        private CancellationTokenSource _disableCancellation = null;
        #endregion

        #region Properties
        public RectTransform RectTransform => _rectTransform;
        public bool ImageIsSet => _imageIsSet;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _imageIsSet = false;
            _disableCancellation = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
            _disableCancellation.Cancel();
        }
        #endregion

        #region Button Handlers
        private void OnButtonClick()
        {

        }
        #endregion

        #region Public Methods
        internal void Init(string url, string imageName)
        {
            _downloadUrl = $"{url}{imageName}";
        }

        internal async UniTaskVoid LaunchImageDownload()
        {
            _imageIsSet = true;

            UnityWebRequest request = await UnityWebRequestTexture.GetTexture(_downloadUrl)
                .SendWebRequest()
                .WithCancellation(_disableCancellation.Token);

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
            _image.overrideSprite = sprite;

        }
        #endregion
    }
}