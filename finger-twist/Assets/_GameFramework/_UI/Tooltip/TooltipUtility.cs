using UnityEngine;

namespace GameFramework.UI.Utils
{
    public static class TooltipUtility
    {
        private static void SetTooltipPosition(ITooltipView tooltipView, RectTransform slotTransform)
        {
            Canvas.ForceUpdateCanvases();

            var tooltipCorners = new Vector3[4];
            var slotCorners = new Vector3[4];
            var below = slotTransform.position.y > Screen.height * 0.5f;
            var right = slotTransform.position.x < Screen.width * 0.5f;
            var slotCorner = GetCornerIndex(below, right);
            var tooltipCorner = GetCornerIndex(!below, !right);

            tooltipView.TooltipRect.GetWorldCorners(tooltipCorners);
            slotTransform.GetWorldCorners(slotCorners);
            tooltipView.TooltipRect.position = slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + tooltipView.TooltipRect.position;
        }

        private static int GetCornerIndex(bool below, bool right)
        {
            if (below && !right)
                return 0;
            if (!below && !right)
                return 1;
            if (!below && right)
                return 2;

            return 3;
        }
    }
}
