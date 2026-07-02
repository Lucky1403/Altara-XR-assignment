using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Attach to the Syringe root (has the XR Grab Interactable).
/// Trigger press #1 (needle empty): plays a "load" animation - fluid visual fills up.
/// Trigger press #2 (needle touching a target AND already loaded): plays an "inject"
/// animation - fluid visual empties, feedback plays at the contact point.
/// No plunger dragging - fully driven by the controller trigger button.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class SimpleSyringe : MonoBehaviour
{
    public float loadDuration = 0.4f;
    public float injectDuration = 0.4f;

    [Header("Plunger Movement")]
    [Tooltip("The plunger transform, child of this syringe")]
    public Transform plunger;
    [Tooltip("Plunger's local position when empty / fully pressed in")]
    public Vector3 plungerPressedLocalPosition = new Vector3(0.242f, 1.787f, 0.852602f);
    [Tooltip("Plunger's local position when loaded / fully drawn back")]
    public Vector3 plungerDrawnLocalPosition = new Vector3(0.271f, 0.586f, 0.852602f);

    [Header("Needle / Injection Target Detection")]
    public Transform needleTip;
    public float injectDetectionRadius = 0.03f;
    public LayerMask injectableLayer;

    private XRGrabInteractable grabInteractable;
    private bool isEmpty = true;
    private bool isAnimating;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(OnTriggerPressed);
        SetPlungerLocalPosition(plungerPressedLocalPosition);
    }

    void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    void OnTriggerPressed(ActivateEventArgs args)
    {
        if (isAnimating) return;

        if (isEmpty)
        {
            StartCoroutine(LoadRoutine());
        }
        else
        {
            Transform target = FindNearbyInjectable();
            if (target != null)
                StartCoroutine(InjectRoutine(target));
            // If full but no target nearby, trigger press does nothing.
        }
    }

    Transform FindNearbyInjectable()
    {
        if (needleTip == null) return null;
        Collider[] hits = Physics.OverlapSphere(needleTip.position, injectDetectionRadius, injectableLayer);
        return hits.Length > 0 ? hits[0].transform : null;
    }

    IEnumerator LoadRoutine()
    {
        isAnimating = true;

        float t = 0f;
        while (t < loadDuration)
        {
            t += Time.deltaTime;
            float normalized = t / loadDuration;
            // Plunger moves from the empty position to the filled position.
            SetPlungerLocalPosition(Vector3.Lerp(plungerPressedLocalPosition, plungerDrawnLocalPosition, normalized));
            yield return null;
        }
        SetPlungerLocalPosition(plungerDrawnLocalPosition);

        isEmpty = false;
        isAnimating = false;
    }

    IEnumerator InjectRoutine(Transform target)
    {
        isAnimating = true;

        float t = 0f;
        while (t < injectDuration)
        {
            t += Time.deltaTime;
            float normalized = t / injectDuration;
            // Plunger moves back from the filled position to the empty position.
            SetPlungerLocalPosition(Vector3.Lerp(plungerDrawnLocalPosition, plungerPressedLocalPosition, normalized));
            yield return null;
        }
        SetPlungerLocalPosition(plungerPressedLocalPosition);

        isEmpty = true;
        isAnimating = false;
    }

    void SetPlungerLocalPosition(Vector3 localPosition)
    {
        if (plunger == null) return;
        plunger.localPosition = localPosition;
    }
}
