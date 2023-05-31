using Game.Enums;
using UnityEngine;

namespace Maniac.Utils.Extension
{
    public static class TransformExtension
    {
        public static Transform ClearAllChildren(this Transform transform)
        {
            if (transform.childCount == 0) return transform;

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }

        public static void CopyDataFrom(this Transform destination, Transform source)
        {
            destination.SetPositionAndRotation(source.position,source.rotation);
            destination.localScale = source.localScale;
        }

        public static Transform Rotate2D(this Transform tf, Vector2 direction,Direction directionType = Direction.Right)
        {
            if (direction == Vector2.zero)
                return tf;

            var angle = Vector2.SignedAngle(GetCorrectDirection2D(tf, directionType), direction);
            tf.Rotate(Vector3.forward,angle);
            
            var localScale = tf.localScale;
            localScale = new Vector3( localScale.x,direction.x > 0 ? 1 : direction.x < 0 ? -1 : tf.localScale.y, localScale.z);
            tf.localScale = localScale;

            return tf;
        }

        private static Vector2 GetCorrectDirection2D(Transform tf, Direction directionType)
        {
            switch (directionType)
            {
                case Direction.Right:
                    return tf.right;
                case Direction.Left:
                    return -tf.right;
                case Direction.Top:
                    return tf.up;
                case Direction.Bottom:
                    return -tf.up;
                default:
                    return Vector2.zero;
            }
        }
    }
}