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

            if (interact != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interact.Interact();
                }

                if (Input.GetMouseButtonDown(0) && interact.itemType == Interactable.ItemType.MusicBox)
                {
                    interact.ToggleMusic();
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