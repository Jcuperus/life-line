using System;
using System.Collections;
using UnityEngine;

namespace Utility.Extensions
{
    public static class CoroutineExtensions
    {
        public static void DelayedAction(this MonoBehaviour monoBehaviour, Action action, float duration)
        {
            monoBehaviour.StartCoroutine(DelayedActionCoroutine(action, duration));
        }

        private static IEnumerator DelayedActionCoroutine(Action action, float duration)
        {
            yield return new WaitForSeconds(duration);

            action();
        }
    }
}