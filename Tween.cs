using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Some aspects of Tween's implemenation is inspired by the Godot Game Engine Tween Node and Unity addon, DOTween:
// https://docs.godotengine.org/en/stable/classes/class_tween.html#class-tween-method-interpolate-property

/// Created by Peter de Vroom, 2021

///<summary>
///Tween component that can interpolate properties over a set duration with a variety of transition and ease types.
///Supported properties include bool, int, float, double, Vector2, Vector3, and Color.
///</summary>
public class Tween : MonoBehaviour
{

    /// <summary>Update tween every frame or physics tick.</summary>
    public TweenUpdateMode UpdateMode = TweenUpdateMode.Update;

    /// <summary>(Obsolete) How close a property needs to be before the interpolation ends.</summary>
    // public float Threshold = 0.1f;
    private readonly List<object> activeTweens = new List<object>();


    void Update()
    {
        if (UpdateMode == TweenUpdateMode.Update)
        {
            for (int i = 0; i < activeTweens.Count; i++)
            {
                Step(activeTweens[i]);
            }
        }
    }

    void FixedUpdate()
    {
        if (UpdateMode == TweenUpdateMode.Fixed)
        {
            for (int i = 0; i < activeTweens.Count; i++)
            {
                Step(activeTweens[i]);
            }
        }
    }

    /// <summary>
    ///Interpolates a property from <c>startValue</c> to <c>endValue</c> for <c>duration</c> seconds.
    ///The tweenCompleted Signal is emitted when InterpolateProperty finishes.
    /// </summary>
    /// <param name="getter">A delegate to get the property.</param>
    /// <param name="setter">A delegate to set the property.</param>
    /// <param name="endValue">The ending value for the interpolation.</param>
    /// <param name="duration">The duration of the interpolation in seconds.</param>
    /// <param name="transitionType">The interpolation algorithm: linear, cubic, etc. Defaults to Linear.</param>
    /// <param name="easeType">The ease method. Defaults to In.</param>
    /// <returns>A UnityEvent that is called when this tween is finished.<returns>
    public UnityEvent InterpolateProperty<T>(Func<T> getter, Action<T> setter, T endValue, float duration, 
                                             TransitionType transitionType = TransitionType.Linear, EaseType easeType = EaseType.In)
    {
        TweenInfo<T> newTween = new TweenInfo<T>(getter, setter, getter(), endValue, duration, transitionType, easeType);
        activeTweens.Add(newTween);
        return newTween.tweenCompleted;
    }

    /// <summary>
    ///Interpolates a property from <c>startValue</c> to <c>endValue</c> for <c>duration</c> seconds.
    ///The tweenCompleted Signal is emitted when InterpolateProperty finishes.
    /// </summary>
    /// <param name="getter">A delegate to get the property.</param>
    /// <param name="setter">A delegate to set the property.</param>
    /// <param name="startValue">The starting value for the interpolation.</param>
    /// <param name="endValue">The ending value for the interpolation.</param>
    /// <param name="duration">The duration of the interpolation in seconds.</param>
    /// <param name="transitionType">The interpolation algorithm: linear, cubic, etc. Defaults to Linear.</param>
    /// <param name="easeType">The ease method. Defaults to In.</param>
    /// <returns>A UnityEvent that is called when this tween is finished.<returns>
    public UnityEvent InterpolateProperty<T>(Func<T> getter, Action<T> setter, T startValue, T endValue, float duration,
                                             TransitionType transitionType = TransitionType.Linear, EaseType easeType = EaseType.In)
    {
        TweenInfo<T> newTween = new TweenInfo<T>(getter, setter, startValue, endValue, duration, transitionType, easeType);
        newTween.Setter(startValue);
        activeTweens.Add(newTween);
        return newTween.tweenCompleted;
    }

    private void Step(object tween)
    {
        switch (tween)
        {
            case TweenInfo<int> iTween:
                int i = iTween.Getter();
                if ((Time.time - iTween.StartTime) < (iTween.Duration))
                {
                    float t = ApplyCalculation(iTween.StartTime, iTween.Duration, iTween.Transition, iTween.Ease);
                    i = (int) Mathf.LerpUnclamped(iTween.StartValue, iTween.EndValue, t);
                    iTween.Setter(i);
                }
                else
                {
                    iTween.Setter(iTween.EndValue);
                    activeTweens.Remove(tween);
                    iTween.tweenCompleted?.Invoke();
                }
                break;

            case TweenInfo<float> fTween:
                float f = fTween.Getter();
                if ((Time.time - fTween.StartTime) < (fTween.Duration))
                {
                    float t = ApplyCalculation(fTween.StartTime, fTween.Duration, fTween.Transition, fTween.Ease);
                    f = Mathf.LerpUnclamped(fTween.StartValue, fTween.EndValue, t);
                    fTween.Setter(f);
                }
                else
                {
                    fTween.Setter(fTween.EndValue);
                    activeTweens.Remove(tween);
                    fTween.tweenCompleted?.Invoke();
                }
                break;

            case TweenInfo<double> dTween:
                double d = dTween.Getter();
                if ((Time.time - dTween.StartTime) < (dTween.Duration))
                {
                    float t = ApplyCalculation(dTween.StartTime, dTween.Duration, dTween.Transition, dTween.Ease);
                    d = DoubleLerpUnclamped(dTween.StartValue, dTween.EndValue, t);
                    dTween.Setter(d);
                }
                else
                {
                    dTween.Setter(dTween.EndValue);
                    activeTweens.Remove(tween);
                    dTween.tweenCompleted?.Invoke();
                }
                break;

            case TweenInfo<Vector2> v2Tween:
                Vector2 vec2 = v2Tween.Getter();
                if ((Time.time - v2Tween.StartTime) < (v2Tween.Duration))
                {
                    float t = ApplyCalculation(v2Tween.StartTime, v2Tween.Duration, v2Tween.Transition, v2Tween.Ease);
                    vec2 = Vector2.LerpUnclamped(v2Tween.StartValue, v2Tween.EndValue, t);
                    v2Tween.Setter(vec2);
                }
                else
                {
                    v2Tween.Setter(v2Tween.EndValue);
                    activeTweens.Remove(tween);
                    v2Tween.tweenCompleted?.Invoke();
                }
                break;

            case TweenInfo<Vector3> v3Tween:
                Vector3 vec3 = v3Tween.Getter();
                if ((Time.time - v3Tween.StartTime) < (v3Tween.Duration))
                {
                    float t = ApplyCalculation(v3Tween.StartTime, v3Tween.Duration, v3Tween.Transition, v3Tween.Ease);
                    vec3 = Vector3.LerpUnclamped(v3Tween.StartValue, v3Tween.EndValue, t);
                    v3Tween.Setter(vec3);
                }
                else
                {
                    v3Tween.Setter(v3Tween.EndValue);
                    activeTweens.Remove(tween);
                    v3Tween.tweenCompleted.Invoke();
                }
                break;
            
            case TweenInfo<Color> cTween:
                Color col = cTween.Getter();
                if ((Mathf.Abs(cTween.EndValue.r - col.r) > Resolution) ||
                    (Mathf.Abs(cTween.EndValue.g - col.g) > Resolution) ||
                    (Mathf.Abs(cTween.EndValue.b - col.b) > Resolution) ||
                    (Mathf.Abs(cTween.EndValue.a - col.a) > Resolution)) 
                {
                    float t = ApplyCalculation(cTween.StartTime, cTween.Duration, cTween.Transition, cTween.Ease);
                    col.r = Mathf.LerpUnclamped(cTween.StartValue.r, cTween.EndValue.r, t);
                    col.g = Mathf.LerpUnclamped(cTween.StartValue.g, cTween.EndValue.g, t);
                    col.b = Mathf.LerpUnclamped(cTween.StartValue.b, cTween.EndValue.b, t);
                    col.a = Mathf.LerpUnclamped(cTween.StartValue.a, cTween.EndValue.a, t);
                    cTween.Setter(col);
                }
                else
                {
                    cTween.Setter(cTween.EndValue);
                    activeTweens.Remove(tween);
                    cTween.tweenCompleted.Invoke();
                }
                break;
            
            case TweenInfo<bool> bTween:
                if (((Time.time - bTween.StartTime) / bTween.Duration) > (1 - Resolution))
                {
                    bTween.Setter(bTween.EndValue);
                    activeTweens.Remove(tween);
                    bTween.tweenCompleted.Invoke();
                }
                break;

            default:
                activeTweens.Remove(tween);
                throw new NotSupportedException($"Interpolation of type ({tween.GetType().GetProperty("StartValue").PropertyType}) is not supported by Tween.");
        }
    }

    /// <summary>
    ///Stops and removes all active tweens.
    ///If used in combination with default startPos InterpolateProperty it can behave like a pause.
    /// </summary>
    /// <returns>The number of tweens stopped.</returns>
    public int StopAllTweens()
    {
        int numberOfTweens = activeTweens.Count;
        activeTweens.Clear();
        return numberOfTweens;
    }

    /// <summary>
    ///Linearly interpolates between a and b by t.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b"> The end value.</param>
    /// <param name="t">The interpolation value between the two doubles.</param>
    private double DoubleLerpUnclamped(double a, double b, float t)
    {
        return a + (b - a) * t;
    }


    /// Easing calculations were sourced from Andrey Sitnik anad Ivan Solovev's Open Source Easings.net: https://easings.net/
    private float ApplyCalculation(float start, float duration, TransitionType transition, EaseType ease)
    {
        float t = (Time.time - start) / duration;
        switch (transition)
        {
            case TransitionType.Linear:
                return t;
            
            case TransitionType.Sine:
                switch (ease)
                {
                    case EaseType.In:
                        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
                    case EaseType.Out:
                        return Mathf.Sin((t * Mathf.PI) / 2);
                    case EaseType.In_Out:
                        return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Quad:
                switch (ease)
                {
                    case EaseType.In:
                        return t * t;
                    case EaseType.Out:
                        return 1 - (1 - t) * (1 - t);
                    case EaseType.In_Out:
                        return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Cubic:
                switch (ease)
                {
                    case EaseType.In:
                        return t * t * t;
                    case EaseType.Out:
                        return 1 - Mathf.Pow(1 - t, 3);
                    case EaseType.In_Out:
                        return t < 0.5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Quart:
                switch (ease)
                {
                    case EaseType.In:
                        return t * t * t * t;
                    case EaseType.Out:
                        return 1 - Mathf.Pow(1 - t, 4);
                    case EaseType.In_Out:
                        return t < 0.5 ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Quint:
                switch (ease)
                {
                    case EaseType.In:
                        return t * t * t * t * t;
                    case EaseType.Out:
                        return 1 - Mathf.Pow(1 - t, 5);
                    case EaseType.In_Out:
                        return t < 0.5 ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Expo:
                switch (ease)
                {
                    case EaseType.In:
                        return t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
                    case EaseType.Out:
                        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
                    case EaseType.In_Out:
                        return t == 0
                        ? 0 
                        : t == 1
                        ? 1
                        : t < 0.5
                        ? Mathf.Pow(2, 20 * t - 10) / 2
                        : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Elastic:
                float c4 = (2 * Mathf.PI) / 3;
                switch (ease)
                {
                    case EaseType.In:
                        return t == 0
                               ? 0
                               : t == 1
                               ? 1
                               : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
                    case EaseType.Out:
                        return t == 0
                               ? 0
                               : t == 1
                               ? 1
                               : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
                    case EaseType.In_Out:
                        float c5 = (2 * Mathf.PI) / 4.5f;
                        return t == 0
                               ? 0
                               : t == 1
                               ? 1
                               : t < 0.5
                               ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2
                               : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Circ:
                switch (ease)
                {
                    case EaseType.In:
                        return t == 0
                               ? 0
                               : t >= 1
                               ? 1
                               : 1 - Mathf.Sqrt(1 - Mathf.Pow(t, 2));
                    case EaseType.Out:
                        return Mathf.Sqrt(1 - (1 - t) * (1 - t));
                    case EaseType.In_Out:
                        return t < 0.5
                               ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
                               : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Bounce:
                switch (ease)
                {
                    case EaseType.In:
                        return 1 - BounceEaseOut(1 - t);
                    case EaseType.Out:
                        return BounceEaseOut(t);
                    case EaseType.In_Out:
                        return t < 0.5
                               ? (1 - BounceEaseOut(1 - 2 * t)) / 2
                               : (1 + BounceEaseOut(2 * t - 1)) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            case TransitionType.Back:
                float c1 = 1.70158f;
                float c2 = c1 * 1.525f;
                float c3 = c1 + 1;
                switch (ease)
                {
                    case EaseType.In:
                        return c3 * t * t * t - c1 * t * t;
                    case EaseType.Out:
                        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
                    case EaseType.In_Out:
                        return t < 0.5
                               ? (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
                               : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected EaseType");
                }
            default:
                throw new ArgumentOutOfRangeException("Unexpected TransitionType");
        }

        float BounceEaseOut(float t)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;
            return t < 1 / d1
            ? n1 * t * t
            : t < 2 / d1
            ? n1 * (t -= 1.5f / d1) * t + 0.75f
            : t < 2.5 / d1
            ? n1 * (t -= 2.25f / d1) * t + 0.9375f
            : n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }


    private readonly struct TweenInfo<T>
    {
        public Func<T> Getter { get; }
        public Action<T> Setter { get; }
        public T StartValue { get; }
        public T EndValue { get; }
        public float Duration { get; }
        public float StartTime { get; }
        public TransitionType Transition { get; }
        public  EaseType Ease { get; }

        /// <summary>Event that is invoked when a tween has finished.</summary>
        public UnityEvent tweenCompleted { get; }

        public TweenInfo(Func<T> getter, Action<T> setter, T startValue, T endValue, float duration, TransitionType transitionType=TransitionType.Linear, EaseType easeType = EaseType.In)
        {
            Getter = getter;
            Setter = setter;
            StartValue = startValue;
            EndValue = endValue;
            Duration = duration;
            StartTime = Time.time;
            Transition = transitionType;
            Ease = easeType;
            tweenCompleted = new UnityEvent();
        }
    }
}

