
using System;
using System.Collections;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;


public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    public float groundDistance;
    private Animator _animator;
    public CinemachineFreeLook cm;
   private GameObject _serverMenu;
   private GameObject _chatMenu;
   bool _isServerMenuOpen = true;
    bool _isChatMenuOpen = true;
    public Transform cameraTransform;

    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(HideServerMenu());
    }

    IEnumerator HideServerMenu()
    {

        yield return new WaitForEndOfFrame();
        if (_serverMenu)
        {
            _serverMenu.SetActive(false);
            _isServerMenuOpen = false;
        }

        if (_chatMenu)
        {
            _chatMenu.SetActive(false);
            _isChatMenuOpen = false;
        }
    }

    private void Awake()
    {
      
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            //Gets references to the server menu and chatmenu
            _serverMenu = GameObject.Find("NetworkUI"); 
            _chatMenu = GameObject.Find("MessageSystem");
            cm.Priority = 1;
            HideCursor();
            
        }
        else
        {
            cm.Priority = 0;
        }
    }
    


    private void Update()
    {
        Movement();
        ToggleMenus();
    }

    private void ToggleMenus()
    {
        //enable and disable the ui so 
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!_serverMenu) return;
            if (_isServerMenuOpen)
            {
               
                _serverMenu.SetActive(false);
                _isServerMenuOpen = false;
                HideCursor();
                
            }
            else
            {
                
                ShowCursor();
                _serverMenu.SetActive(true);
                _isServerMenuOpen = true;
                
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_chatMenu) return;
            if (_isChatMenuOpen)
            {
                HideCursor();
                _chatMenu.SetActive(false);
                _isChatMenuOpen = false;
                
            }
            else
            {
                ShowCursor();
                _chatMenu.SetActive(true);
                _isChatMenuOpen = true;
                ChatManager.Singleton.FocusOnInputField();
            }
        }
    }

    private void Movement()
    {
        if(_isChatMenuOpen) return;
        if(!IsOwner)return;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
 
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * verticalInput + right * horizontalInput;
        desiredMoveDirection.Normalize();

        transform.Translate(desiredMoveDirection * speed * Time.deltaTime, Space.World);

        if(desiredMoveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(desiredMoveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }

        if (Input.GetButtonDown("Jump")  && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance);
    }
    
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }
    
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 
    }
}