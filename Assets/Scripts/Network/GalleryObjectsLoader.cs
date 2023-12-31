using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class GalleryObjectsLoader : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private string _url = "http://data.ikppbb.com/test-task-unity-data/pics/";
        #endregion

        #region Fields
        private Dictionary<string, Sprite> _imageNames = null;
        private List<string> _imageKeys = null;
        private CancellationTokenSource _disableCancellation = null;

        private const string GROUP_NAME = "name";
        #endregion

        #region Properties
        public string URL => _url;
        public Dictionary<string, Sprite> ImageNames => _imageNames;
        public List<string> ImageKeys => _imageKeys;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            DontDestroyOnLoad(this);

            _imageNames = new Dictionary<string, Sprite>();
            _imageKeys = new List<string>();
            _disableCancellation = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _disableCancellation.Cancel();
            _disableCancellation.Dispose();
        }
        #endregion

        #region Public Methods
        internal async UniTaskVoid WebLoad(IProgress<float> progress)
        {
            UnityWebRequest request = await UnityWebRequest.Get(_url)
                .SendWebRequest()
                .ToUniTask(progress)
                .AttachExternalCancellation(_disableCancellation.Token);

            Regex regex = new Regex("<a href=\".*\">(?<name>.*jpg)</a>");
            MatchCollection matches = regex.Matches(request.downloadHandler.text);
            request.Dispose();

            if (matches.Count == 0)
            {
                Debug.Log("There are no objects.");
                return;
            }

            foreach (Match match in matches.Cast<Match>())
            {
                if (!match.Success) continue;

                string key = match.Groups[GROUP_NAME].Value;
                if (!_imageNames.ContainsKey(key))
                {
                    _imageNames.Add(key, null);
                }
            }

            _imageNames = _imageNames.OrderBy(x => int.Parse(Regex.Replace(x.Key, "[^0-9]+", "0"))).ToDictionary(obj => obj.Key, obj => obj.Value);
            _imageKeys = _imageNames.Keys.ToList();
        }
        #endregion
    }
}
