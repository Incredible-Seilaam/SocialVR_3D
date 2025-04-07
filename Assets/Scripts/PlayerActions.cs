using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private TextMeshPro UseText;
    [SerializeField] private Transform Camera;
    [SerializeField] private float MaxUseDistance = 5f;
    [SerializeField] private LayerMask UseLayers;
    [SerializeField] private Inspect inspectController;

    private void Start()
    {
        UseText.gameObject.SetActive(true);

        // Ensure inspectController is not null at the start
        if (inspectController == null)
        {
            inspectController = GetComponent<Inspect>();
        }
    }

    private void Update()
    {
        // Perform raycast to check if player is looking at an interactable object
        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, MaxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<Door>(out Door door))
            {
                UseText.SetText(door.IsOpen ? "Close \"E\"" : "Open \"E\"");
                // UseText.text = door.IsOpen ? "Close 'E'" : "Open 'E'";
            }
            else if (hit.transform.CompareTag("pickable"))
            {
                UseText.SetText("Pick-up \"E\"");
            }
            else
            {
                UseText.gameObject.SetActive(false);
                return;
            }

            // Update UI position and visibility
            UseText.gameObject.SetActive(true);
            UseText.transform.position = hit.point - (hit.point - Camera.position).normalized * 0.01f;
            UseText.transform.rotation = Quaternion.LookRotation((hit.point - Camera.position).normalized);

            // Check for "E" key press to trigger action
            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleInteraction(hit);
            }
        }
        else
        {
            UseText.gameObject.SetActive(false);
        }
    }

    private void HandleInteraction(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<Door>(out Door door))
        {
            if (door.IsOpen)
            {
                door.Close();
            }
            else
            {
                door.Open(transform.position);
            }
        }
        else if (hit.transform.CompareTag("pickable") && inspectController != null)
        {
            inspectController.PickUpObject(hit.transform.gameObject);
        }
    }
}
