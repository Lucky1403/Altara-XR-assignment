using UnityEngine;
public class SimplePaperCut : MonoBehaviour
{
    public Transform paperHalfLeft;
    public Transform paperHalfRight;
    public float separationForce = 0.25f;

    public bool IsCut { get; private set; }

    private Rigidbody leftRb;
    private Rigidbody rightRb;

    void Start()
    {
        if (paperHalfLeft == null)
            Debug.LogWarning("SimplePaperCut: paperHalfLeft is not assigned on " + name);
        else
            paperHalfLeft.gameObject.SetActive(false);

        if (paperHalfRight == null)
            Debug.LogWarning("SimplePaperCut: paperHalfRight is not assigned on " + name);
        else
            paperHalfRight.gameObject.SetActive(false);

    }

    public void Cut()
    {
        Debug.Log("SimplePaperCut.Cut() called on " + name + " (IsCut=" + IsCut + ")");
        if (IsCut)
        {
            Debug.Log("SimplePaperCut: already cut, skipping on " + name);
            return;
        }

        IsCut = true;

        if (paperHalfLeft != null)
        {
            leftRb = paperHalfLeft.GetComponent<Rigidbody>();
            paperHalfLeft.gameObject.SetActive(true);
            Debug.Log("SimplePaperCut: activated existing left half " + paperHalfLeft.name);
        }
        else
        {
            Debug.LogWarning("SimplePaperCut: paperHalfLeft reference is missing on " + name);
        }

        if (paperHalfRight != null)
        {
            rightRb = paperHalfRight.GetComponent<Rigidbody>();
            paperHalfRight.gameObject.SetActive(true);
            Debug.Log("SimplePaperCut: activated existing right half " + paperHalfRight.name);
        }
        else
        {
            Debug.LogWarning("SimplePaperCut: paperHalfRight reference is missing on " + name);
        }

        if (leftRb == null)
            Debug.LogWarning("SimplePaperCut: left half has no Rigidbody on " + name);
        if (rightRb == null)
            Debug.LogWarning("SimplePaperCut: right half has no Rigidbody on " + name);

        if (leftRb != null)
        {
            leftRb.isKinematic = false;
            leftRb.useGravity = true;
            leftRb.AddForce(-transform.right * separationForce, ForceMode.Impulse);
        }
        if (rightRb != null)
        {
            rightRb.isKinematic = false;
            rightRb.useGravity = true;
            rightRb.AddForce(transform.right * separationForce, ForceMode.Impulse);
        }

        if (paperHalfLeft != null)
            paperHalfLeft.SetParent(null);
        if (paperHalfRight != null)
            paperHalfRight.SetParent(null);

        if (leftRb != null || rightRb != null)
            Debug.Log("SimplePaperCut: cut completed on " + name + " with separation force " + separationForce);

        Debug.Log("SimplePaperCut: destroying original paper object " + name);
        Destroy(gameObject);
    }
}