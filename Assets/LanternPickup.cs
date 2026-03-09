using UnityEngine;

public class LanternPickup : MonoBehaviour
{
    public GameObject playerLantern;
    private bool playerNearby = false;





    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickUpLantern();
        }

    }

    void PickUpLantern()
    {
        playerLantern.SetActive(true);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}