using UnityEngine;

namespace UserInterface
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public class SceneUICore : MonoBehaviour
    {
        #region MonoBehaviour API
        protected virtual void Awake() => AutorotateSettings();

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickEscape();
            }
        }
        #endregion

        #region Virtual Methods
        protected internal virtual void AutorotateSettings() { }

        protected internal virtual void OnClickEscape() { }
        #endregion
    }
}