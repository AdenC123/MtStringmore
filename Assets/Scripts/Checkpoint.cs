using UnityEngine;
using Yarn.Unity;

/// <summary>
///     Checkpoint flag that sets checkpoint position when player collides with it
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private static readonly int HoistKey = Animator.StringToHash("Hoisted");

    [Header("References")] [SerializeField]
    private Animator anim;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private string conversationStartNode;
    [SerializeField] private bool beginsInteractable;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private bool interactable;
    private bool isCurrentConversation;

    public void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        interactable = beginsInteractable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && anim.GetBool(HoistKey) == false)
        {
            anim.SetBool(HoistKey, true);
            GameManager.Instance.CheckPointPos = transform.position;
            StartConversation();
        }
    }

    private void StartConversation()
    {
        if (conversationStartNode != "")
        {
            Debug.Log("Started dialogue at checkpoint.");
            isCurrentConversation = true;
            dialogueRunner.StartDialogue(conversationStartNode);
            Time.timeScale = 0;
        }
    }

    private void EndConversation()
    {
        if (isCurrentConversation)
        {
            isCurrentConversation = false;
            Debug.Log("Ended dialogue at checkpoint.");
            DisableConversation();
            Time.timeScale = 1;
        }
    }

    [YarnCommand("enable")]
    public void EnableConversation()
    {
        interactable = true;
    }

    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
}