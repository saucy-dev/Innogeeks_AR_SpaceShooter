using UnityEngine;
using UnityEditor;
public class SlingshotShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public AudioClip shootSound;
    public float maxForce = 1000f;       // Maximum shoot force
    public float chargeMultiplier = 5f;  // How fast force increases while dragging

    private float currentForce = 0f;
    private bool isCharging = false;

    private Vector2 startTouchPos;   // Finger down position
    private Vector2 currentTouchPos; // Finger drag position

    private AudioSource audioSource;
    private GameObject currentProjectile;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
#if UNITY_EDITOR
        // Mouse input for testing in editor
        if (Input.GetMouseButtonDown(0))
        {
            StartCharge(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            ContinueCharge(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }
#else
        // Touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                StartCharge(touch.position);

            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                ContinueCharge(touch.position);

            else if (touch.phase == TouchPhase.Ended)
                Release();
        }
#endif
    }

    void StartCharge(Vector2 touchPos)
    {
        float allowedRegion = Screen.height / 3f;
        if (touchPos.y > allowedRegion) return;

        startTouchPos = touchPos;
        isCharging = true;
        currentForce = 0f;
    }

    void ContinueCharge(Vector2 touchPos)
    {
        if (!isCharging) return;

        currentTouchPos = touchPos;
        float distance = Vector2.Distance(startTouchPos, currentTouchPos);

        if (currentProjectile == null && distance > 10f) // drag at least 10px before spawning
        {
            currentProjectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            currentProjectile.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (currentProjectile == null) return; // no projectile yet

        currentForce = Mathf.Clamp(distance * chargeMultiplier, 0f, maxForce);

        // Move projectile back visually
        float pullNormalized = Mathf.Clamp01(distance / 200f);
        Vector3 pullOffset = -Camera.main.transform.forward * pullNormalized * 0.2f;
        currentProjectile.transform.position = shootPoint.position + pullOffset;
    }

    void Release()
    {
        if (!isCharging) return;

        if (currentProjectile != null)
        {
            if(audioSource !=null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
            Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * currentForce);
            Destroy(currentProjectile, 5f);
        }

        currentProjectile = null;
        isCharging = false;
    }
}