using UnityEngine;

public static class FloatExtensions
{
    public static float Remap(this float v, float from1, float to1, float from2, float to2){
        return Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, v));
    }
    public static Vector3 Lerp(this float v, float from1, float to1, Vector3 from2, Vector3 to2){
        return new Vector3(
            Mathf.Lerp(from2.x, to2.x, Mathf.InverseLerp(from1, to1, v)),
            Mathf.Lerp(from2.y, to2.y, Mathf.InverseLerp(from1, to1, v)),
            Mathf.Lerp(from2.z, to2.z, Mathf.InverseLerp(from1, to1, v))
        );
    }
    public static Vector2 Lerp(this float v, float from1, float to1, Vector2 from2, Vector2 to2){
        return new Vector3(
            Mathf.Lerp(from2.x, to2.x, Mathf.InverseLerp(from1, to1, v)),
            Mathf.Lerp(from2.y, to2.y, Mathf.InverseLerp(from1, to1, v))
        );
    }
}
