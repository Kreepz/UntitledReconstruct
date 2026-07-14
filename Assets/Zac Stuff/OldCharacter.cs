using UnityEngine;

public class Character : MonoBehaviour
{
    
    public float mouseSense;
    public float moveSpeed;

    private Rigidbody rb;
    private Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        camera = transform.Find("Camera").GetComponent<Camera>(); //camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal") * mouseSense * Time.deltaTime;
        float moveY = Input.GetAxisRaw("Vertical") * mouseSense * Time.deltaTime;
        float mouseX = Input.GetAxisRaw("Mouse X") * moveSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * moveSpeed * Time.deltaTime;

        //Debug.Log($"x: {x}, y: {y}");

        rb.linearVelocity = new Vector3(moveX * moveSpeed,0,moveY * moveSpeed);
        camera.transform.rotation = Quaternion.Euler(0, mouseY, 0);
        transform.rotation = Quaternion.Euler(mouseX,0,0);

    }
}
