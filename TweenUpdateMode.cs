using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selects Tween update process mode.
/// </summary>
public enum TweenUpdateMode
{
    /// <summary>Syncronised with Unity Update calls.</summary>
    Update,
    /// <summary>Syncronised with Unity FixedUpdate calls</summary>
    Fixed
}
