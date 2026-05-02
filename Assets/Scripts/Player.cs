using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float minPitch = -40f;
    public float maxPitch = 80f;

    [Header("Climb Settings")]
    public float stepHeight = 0.3f; // ความสูงของขั้นบันไดที่ก้าวข้ามได้
    public float stepSmooth = 0.1f; // ความนุ่มนวลในการก้าว

    private Rigidbody rb;
    private float pitch = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // กันตัวละครล้มคว่ำ
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up, mouseX);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 movement = transform.forward * vertical + transform.right * horizontal;
        movement.y = 0; // ตัดแกน Y เพื่อไม่ให้เดินแล้วตัวลอย
        movement.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // --- ส่วนที่แก้ไขเรื่องบันได ---
        if (movement.magnitude > 0.1f) 
        {
            StepClimb(movement);
        }

        Vector3 velocity = movement * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    // ฟังก์ชันช่วยก้าวข้ามขั้นบันได
    void StepClimb(Vector3 direction)
    {
        // ยิง Raycast เช็คว่าข้างหน้ามีขั้นบันไดไหม
        RaycastHit hitLower;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), direction, out hitLower, 0.6f))
        {
            RaycastHit hitUpper;
            // เช็คว่าความสูงนั้นไม่เกินค่าที่เรากำหนด (Step Height)
            if (!Physics.Raycast(transform.position + new Vector3(0, stepHeight, 0), direction, out hitUpper, 0.7f))
            {
                // ถ้าข้างล่างชนแต่ข้างบนไม่ชน แสดงว่าเป็นขั้นบันได ให้เพิ่มแรงส่งขึ้นเบาๆ
                rb.position += new Vector3(0f, stepSmooth, 0f);
            }
        }
    }
}