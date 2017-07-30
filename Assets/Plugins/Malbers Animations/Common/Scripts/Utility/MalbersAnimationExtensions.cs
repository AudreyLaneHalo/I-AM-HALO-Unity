using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public static class MalbersAnimationsExtensions
{
    /// <summary>
    /// Find the first transform grandchild with this name inside this transform
    /// </summary>
    public static Transform FindGrandChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindGrandChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// Invoke with Parameters
    /// </summary>
    public static void InvokeWithParams(this MonoBehaviour sender, string method, object args)
    {
        var methodPtr = sender.GetType().GetMethod(method);

        if (methodPtr != null)
        {
            if (args != null)
            {
                var arguments = new object[1] { args };
                methodPtr.Invoke(sender, arguments);
            }
            else
            {
                methodPtr.Invoke(sender, null);
            }
        }
    }


    /// <summary>
    /// Invoke with Parameters and Delay
    /// </summary>
    public static void Invoke(this MonoBehaviour behaviour, string method, object options, float delay)
    {
        behaviour.StartCoroutine(_invoke(behaviour, method, delay, options));
    }

    private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, float delay, object options)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        Type instance = behaviour.GetType();
        MethodInfo mthd = instance.GetMethod(method);
        mthd.Invoke(behaviour, new object[] { options });

        yield return null;
    }
}