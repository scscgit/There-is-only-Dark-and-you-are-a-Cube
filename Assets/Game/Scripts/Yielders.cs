using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Courtesy of https://forum.unity.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/#post-2151707
/// </summary>
public static class Yielders
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return x == y;
        }

        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    static Dictionary<float, WaitForSeconds> _timeInterval =
        new Dictionary<float, WaitForSeconds>(100, new FloatComparer());

    static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();

    public static WaitForEndOfFrame EndOfFrame => _endOfFrame;

    public static WaitForFixedUpdate FixedUpdate => _fixedUpdate;

    public static WaitForSeconds Seconds(float seconds)
    {
        if (!_timeInterval.ContainsKey(seconds))
        {
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        }

        return _timeInterval[seconds];
    }
}
