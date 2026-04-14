using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public GameObject keyPromptUI;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        keyPromptUI.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            Interactable interact = hit.collider.GetComponentInParent<Interactable>();
            InDepthInteract inDepthInteract = hit.collider.GetComponentInParent<InDepthInteract>();
            DoorInteract door = hit.collider.GetComponentInParent<DoorInteract>();
            LetterInteract letter = hit.collider.GetComponentInParent<LetterInteract>();
            PotEndingInteract pot = hit.collider.GetComponentInParent<PotEndingInteract>();

            if (interact != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interact.Interact();
                }

            }
            else if (door != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.Interact();
                }
            }
            else if (inDepthInteract != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    inDepthInteract.Interact();
                }
            }
            else if (pot != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    pot.Interact();
                }
            }
            else
            {
                keyPromptUI.SetActive(false);
            }
        }
        else
        {
            keyPromptUI.SetActive(false);
        }
    }
}