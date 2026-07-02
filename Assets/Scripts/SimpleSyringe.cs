using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class SimpleSyringe : MonoBehaviour
{
    private enum SyringeState { Empty, Loaded }

    [Header("Fluid Visual")]
    [Tooltip("Thin cylinder mesh inside the barrel, scaled by fill amount")]
    public Transform fluidVisual;
    public float fluidFullScaleY = 1f;
    public float loadDuration = 0.4f;
    public float injectDuration = 0.4f;

    [Header("Needle / Injection Target Detection")]
    public Transform needleTip;
    public float injectDetectionRadius = 0.03f;
    public LayerMask injectableLayer;

    [Header("Feedback")]
    public ParticleSystem loadParticles;
    public ParticleSystem injectParticles;
    public AudioSource loadSound;
    public AudioSource injectSound;

    private XRGrabInteractable grabInteractable;
    private SyringeState state = SyringeState.Empty;
    private bool isAnimating;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(OnTriggerPressed);
        SetFluidScale(0f);
    }

    void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    void OnTriggerPressed(ActivateEventArgs args)
    {
        if (isAnimating) return;

        if (state == SyringeState.Empty)
        {
            StartCoroutine(LoadRoutine());
        }
        else if (state == SyringeState.Loaded)
        {
            Transform target = FindNearbyInjectable();
            if (target != null)
                StartCoroutine(InjectRoutine(target));
            // If loaded but no target nearby, trigger press does nothing -
            // this matches "needle must be on the object" from the brief.
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
        if (loadParticles != null) loadParticles.Play();
        if (loadSound != null) loadSound.PlayOneShot(loadSound.clip);

        float t = 0f;
        while (t < loadDuration)
        {
            t += Time.deltaTime;
            SetFluidScale(Mathf.Lerp(0f, 1f, t / loadDuration));
            yield return null;
        }
        SetFluidScale(1f);

        state = SyringeState.Loaded;
        isAnimating = false;
    }

    IEnumerator InjectRoutine(Transform target)
    {
        isAnimating = true;
        if (injectParticles != null)
        {
            injectParticles.transform.position = target.position;
            injectParticles.Play();
        }
        if (injectSound != null) injectSound.PlayOneShot(injectSound.clip);

        float t = 0f;
        while (t < injectDuration)
        {
            t += Time.deltaTime;
            SetFluidScale(Mathf.Lerp(1f, 0f, t / injectDuration));
            yield return null;
        }
        SetFluidScale(0f);

        state = SyringeState.Empty;
        isAnimating = false;
    }

    void SetFluidScale(float normalized)
    {
        if (fluidVisual == null) return;
        Vector3 scale = fluidVisual.localScale;
        scale.y = fluidFullScaleY * normalized;
        fluidVisual.localScale = scale;
    }
}