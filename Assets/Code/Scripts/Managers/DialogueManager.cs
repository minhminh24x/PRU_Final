using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
  [Header("UI")]
  public GameObject dialogueCanvas;
  public TextMeshProUGUI dialogueText;

  [Header("Dialogue")]
  [TextArea]
  public string[] lines;
  public float typingSpeed = 0.04f;

  int index;
  bool isTyping;

  void Start()
  {
    dialogueCanvas.SetActive(false);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.E))
    {
      Debug.Log("E pressed");

      if (!dialogueCanvas.activeSelf)
      {
        StartDialogue();
      }
      else
      {
        NextLine();
      }
    }
  }

  void StartDialogue()
  {
    dialogueCanvas.SetActive(true);
    index = 0;
    StopAllCoroutines();
    StartCoroutine(TypeLine());
  }

  void NextLine()
  {
    if (isTyping) return;

    index++;

    if (index < lines.Length)
    {
      StopAllCoroutines();
      StartCoroutine(TypeLine());
    }
    else
    {
      dialogueCanvas.SetActive(false);
    }
  }

  System.Collections.IEnumerator TypeLine()
  {
    isTyping = true;
    dialogueText.text = "";

    foreach (char c in lines[index])
    {
      dialogueText.text += c;
      yield return new WaitForSeconds(typingSpeed);
    }

    isTyping = false;
  }
}
