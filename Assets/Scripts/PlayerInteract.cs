using UnityEngine;
using System.Collections;

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
            Interactable standardInteract = hit.collider.GetComponent<Interactable>();
            InDepthInteract inDepthInteract = hit.collider.GetComponent<InDepthInteract>();
            PotInteract potInteract = hit.collider.GetComponent<PotInteract>();

            if (standardInteract != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    standardInteract.Interact();
                }

                if (Input.GetMouseButtonDown(0) && standardInteract.itemType == Interactable.ItemType.MusicBox)
                {
                    standardInteract.ToggleMusic();
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
            else if (potInteract != null)
            {
                keyPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    potInteract.Interact();
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
