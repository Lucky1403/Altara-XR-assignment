using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class SimpleSyringe : MonoBehaviour
{
    public float loadDuration = 0.4f;
    public float injectDuration = 0.4f;

    [Header("Plunger Movement")]
    public Transform plunger; // plunger gameobject transform
    public Vector3 plungerPressedLocalPosition = new Vector3(0.242f, 1.787f, 0.852602f); //initial position of the plunger when the syringe is empty
    public Vector3 plungerDrawnLocalPosition = new Vector3(0.271f, 0.586f, 0.852602f); // position of the plunger after medicine fills

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