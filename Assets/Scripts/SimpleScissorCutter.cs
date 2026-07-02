using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class SimpleScissorCutter : MonoBehaviour
{
    [Header("Blade Parts")]
    public Transform bladeA;
    public Transform bladeB;
    [Range(0f, 60f)] public float snapOpenAngle = 25f;
    public float snapDuration = 0.15f;

    [Header("Cut Detection")]
    public Transform bladeTip; // Scissor blade tip
    public float cutDetectionRadius = 0.05f;
    public LayerMask cuttableLayer;
    private XRGrabInteractable grabInteractable;
    private Quaternion bladeARest;
    private Quaternion bladeBRest;
    private bool isAnimating;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(OnTriggerPressed);

        if (bladeA != null) bladeARest = bladeA.localRotation;
        if (bladeB != null) bladeBRest = bladeB.localRotation;
    }

    void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    void OnTriggerPressed(ActivateEventArgs args)
    {
        if (isAnimating) return;
        StartCoroutine(SnapAndCut());
    }

    IEnumerator SnapAndCut()
    {
        isAnimating = true;

        yield return AnimateBlades(0f, snapOpenAngle * -1f, snapDuration * 0.4f);
        TryCutNearbyObject();
        yield return AnimateBlades(snapOpenAngle * -1f, 0f, snapDuration * 0.6f);

        isAnimating = false;
    }

    IEnumerator AnimateBlades(float fromAngle, float toAngle, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(fromAngle, toAngle, t / duration);
            if (bladeA != null) bladeA.localRotation = bladeARest * Quaternion.Euler(0f, 0f, angle);
            if (bladeB != null) bladeB.localRotation = bladeBRest * Quaternion.Euler(0f, 0f, -angle);
            yield return null;
        }
    }

    void TryCutNearbyObject()
    {
        if (bladeTip == null) return;

        Collider[] hits = Physics.OverlapSphere(bladeTip.position, cutDetectionRadius, cuttableLayer);
        foreach (var hit in hits)
        {
            SimplePaperCut paper = hit.GetComponentInParent<SimplePaperCut>();
            if (paper != null && !paper.IsCut)
            {
                paper.Cut();
                break;
            }
        }
    }
}