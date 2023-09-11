using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DialoguePrinter : MonoBehaviour
{
    public static DialoguePrinter Instance { get; private set; }

    [SerializeField] private TMP_Text dialogueTextMesh;

    public void PrintDialogueLine(string lineToPrint, float printSpeed, Action finishedCallback)
    {
        StartCoroutine(PrintDialogueLineCoroutine(lineToPrint, printSpeed, finishedCallback));
    }
    private IEnumerator PrintDialogueLineCoroutine(string lineToPrint, float printSpeed, Action finishedCallback)
    {
        Debug.Log("Printing Line: " + lineToPrint);
        dialogueTextMesh.SetText(string.Empty);
        for (int i = 0; i < lineToPrint.Length; i++)
        {
            var character = lineToPrint[i];
            dialogueTextMesh.SetText(dialogueTextMesh.text + character);

            yield return new WaitForSeconds(printSpeed);
        }

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        dialogueTextMesh.SetText(string.Empty);

        finishedCallback?.Invoke();
        EventBus.Instance.ResumeGameplay();

        yield return null;
    }

    private void Awake() {
        Instance = this;
    }
}
