using UnityEngine;

namespace Data.Common.VisibilityChecker
{
    public static class VisibilityChecker
    {
        public static bool IsVisibleOnCamera(Camera camera, GameObject target)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            var point = target.transform.position;

            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
