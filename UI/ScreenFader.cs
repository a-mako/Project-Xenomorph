using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    [SerializeField] private Image fader;
    private bool isBusy;

    public void FadeToBlack(float duration, Action finishedCallback) {
        if (isBusy) return;
        StartCoroutine(FadeToBlackCoroutine(duration, finishedCallback));
    }
    public void FadeFromBlack(float duration, Action finishedCallback) {
        if (isBusy) return;
        StartCoroutine(FadeFromBlackCoroutine(duration, finishedCallback));
    }

    private void Awake() {
        Instance = this;
    }
    private IEnumerator FadeToBlackCoroutine(float duration, Action finishedCallback) {
        isBusy = true;
        while (fader.color.a < 1) {
            fader.color = new Color(0,0,0, fader.color.a + (Time.deltaTime / duration));
            yield return null;
        }
        fader.color = new Color(0,0,0,1);
        isBusy = false;
        finishedCallback?.Invoke();
        yield return null;
    }
    private IEnumerator FadeFromBlackCoroutine(float duration, Action finishedCallback) {
        isBusy = true;
        while (fader.color.a > 0) {
            fader.color = new Color(0,0,0, fader.color.a - (Time.deltaTime / duration));
            yield return null;
        }
        fader.color = new Color(0,0,0,0);
        isBusy = false;
        finishedCallback?.Invoke();
        yield return null;
    }
}
