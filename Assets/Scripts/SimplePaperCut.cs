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
        leftRb = paperHalfLeft.GetComponent<Rigidbody>();
        rightRb = paperHalfRight.GetComponent<Rigidbody>();
        if (leftRb != null) leftRb.isKinematic = true;
        if (rightRb != null) rightRb.isKinematic = true;
    }

    public void Cut()
    {
        if (IsCut) return;
        IsCut = true;

        if (leftRb != null)
        {
            leftRb.isKinematic = false;
            leftRb.AddForce(-transform.right * separationForce, ForceMode.Impulse);
        }
        if (rightRb != null)
        {
            rightRb.isKinematic = false;
            rightRb.AddForce(transform.right * separationForce, ForceMode.Impulse);
        }
    }
}