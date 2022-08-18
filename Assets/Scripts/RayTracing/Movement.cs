using UnityEngine;

public class Movement : MonoBehaviour
{
    public float _movementSpeed;
    public float _cameraSensitivity = 3f;
    public float _maxYAngle = 80f;

    Vector3 _input;

    private Vector2 _currentRotation;
    private bool _lockRotation;
    private Camera _cam;

    private Rigidbody _rb;
    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update() {

        // movement input
        _input = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))  _input.x += -1;
        if (Input.GetKey(KeyCode.D)) _input.x += 1;
        if (Input.GetKey(KeyCode.LeftShift))  _input.y += -1;
        if (Input.GetKey(KeyCode.Space)) _input.y += 1;
        if (Input.GetKey(KeyCode.W)) _input.z += 1;
        if (Input.GetKey(KeyCode.S)) _input.z += -1;
        
        // exit
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();

        // camera rotation input
        if (!_lockRotation) {
            _currentRotation.x += Input.GetAxis("Mouse X") * _cameraSensitivity * (_cam.fieldOfView / 90);
            _currentRotation.y -= Input.GetAxis("Mouse Y") * _cameraSensitivity * (_cam.fieldOfView / 90);
            _currentRotation.x = Mathf.Repeat(_currentRotation.x, 360);
            _currentRotation.y = Mathf.Clamp(_currentRotation.y, -_maxYAngle, _maxYAngle);
            _cam.transform.rotation = Quaternion.Euler(_currentRotation.y, _currentRotation.x, 0);
        }

        // speed adjustment
        _movementSpeed = Mathf.Clamp(_movementSpeed + Input.mouseScrollDelta.y, 0, 30);

        // lock/release mouse cursor in Game
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _lockRotation = true;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _lockRotation = false;
            }
        }
    } 

    private void FixedUpdate() {
        // get forward and right direction of camera
        Vector3 forward = _cam.transform.forward;
        Vector3 right = _cam.transform.right;
        Vector3 upward = new Vector3(0,1,0);

        // don't change y direction to move
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // get direction and speed to move:
        Vector3 desiredMoveDirection = forward * _input.z + right * _input.x + upward * _input.y;
        desiredMoveDirection.Normalize();

        _rb.velocity = desiredMoveDirection * _movementSpeed;
    }
}