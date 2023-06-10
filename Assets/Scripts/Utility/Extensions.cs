using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public static class Extensions
    {
        public static void GetSafeComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            if (gameObject.TryGetComponent(out T foundComponent))
            {
                component = foundComponent;
            }
            else
                throw new MissingComponentException($"There is no {typeof(T)} component on the {gameObject}.");
        }

        private static int CountCornersVisibleFrom(this RectTransform rectTransform, RectTransform mask)
        {
            Rect screenBounds = new(mask.anchoredPosition.x, mask.anchoredPosition.y, mask.rect.width, mask.rect.height);

            Vector3[] maskCorners = new Vector3[4];
            rectTransform.GetWorldCorners(maskCorners);

            int visibleCorners = 0;
            for (int index = 0; index < maskCorners.Length; index++)
            {
                if (screenBounds.Contains(maskCorners[index]))
                {
                    visibleCorners++;
                }
            }
            return visibleCorners;
        }

        public static bool IsVisible(this RectTransform rectTransform, RectTransform mask)
        {
            return CountCornersVisibleFrom(rectTransform, mask) > 0;
        }
    }
}