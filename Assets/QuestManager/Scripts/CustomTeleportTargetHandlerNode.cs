using UnityEngine;

namespace Quest 
{
	/// <summary>
	/// 自定义传送，可兼容自由传送和定点传送
	/// </summary>
    public class CustomTeleportTargetHandlerNode : TeleportTargetHandlerNode
    {
		/// <summary>
		/// This method will be called while the LocmotionTeleport component is in the aiming state, once for each
		/// line segment that the targeting beam requires. 
		/// The function should return true whenever an actual target location has been selected.
		/// </summary>
		protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
		{
			// If the ray hits the world, consider it valid and update the aimRay to the end point.
			if (!LocomotionTeleport.AimCollisionTest(start, end, AimCollisionLayerMask | TeleportLayerMask, out AimData.TargetHitInfo))
			{
				return false;
			}
			OvrTeleportPoint tp = AimData.TargetHitInfo.collider.gameObject.GetComponent<OvrTeleportPoint>();
			if (tp == null)
			{
				// If the ray hits the world, consider it valid and update the aimRay to the end point.
				var d = (end - start).normalized;

				end = start + d * AimData.TargetHitInfo.distance;
				return true;
			}

			// The targeting test discovered a valid teleport node. Now test to make sure there is line of sight to the 
			// actual destination. Since the teleport destination is expected to be right on the ground, use the LOSOffset 
			// to bump the collision check up off the ground a bit.
			var dest = tp.destTransform.position;
			var offsetEnd = new Vector3(dest.x, dest.y + LOSOffset, dest.z);
			if (LocomotionTeleport.AimCollisionTest(start, offsetEnd, AimCollisionLayerMask & ~TeleportLayerMask, out AimData.TargetHitInfo))
			{
				return false;
			}

			end = dest;
			return true;
		}
	}
}
