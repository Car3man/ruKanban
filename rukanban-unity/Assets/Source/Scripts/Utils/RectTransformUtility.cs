using UnityEngine;

namespace RuKanban.Utils
{
    public static class RectTransformExtensions
    {
        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            Vector3[] corners = new Vector3[4];
            
            a.GetWorldCorners(corners);
            Rect aRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            b.GetWorldCorners(corners);
            Rect bRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
            
            return aRect.Overlaps(bRect);
        }
    }
}