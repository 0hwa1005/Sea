#pragma warning disable IDE0051

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    //====================ĳ���� ����(�����)=======================
    public enum CharacterState
    {
        Idle,
        Swim,
        Jump,
        Land,
        Roll,
        Die
    }

    private CharacterState curState;
    public CharacterState CharacterStateSetting { get { return curState; }set { curState = value; } }
    private Animator[] allLodAnimator;

    //====================�̵�=======================
    private float moveDir;
    private float rotDir;
    private Vector3 rotY;
    private Vector3 moveYDir;
    private Vector3 rotYDir;
    private Vector3 lookDir;
    private Vector2 mouseDelta;
    private Vector2 mouseScroll;

    public float swimSpeed;
    public float swimSpeedMax = 50.0f;
    public float swimSpeedMin = 20.0f;
    public float swimSpeedAcc = 3f;

    public float rotateSpeed;
    public float rotateSpeedMax = 100.0f;
    public float rotateSpeedMin = 20.0f;
    public float rotateSpeedAcc = 3f;

    //====================��ǲ �ý���=======================
    private PlayerInput playerInput;
    private InputActionMap mainActionMap;
    private InputAction moveAction;
    private InputAction moveYAction;
    private InputAction rotateCamAction;
    private InputAction zoomCamAction;

    public Transform cameraArm;


    void Start()
    {
        allLodAnimator = transform.GetComponentsInChildren<Animator>();
        //cameraArm = transform.GetChild(1);
        //====================��ǲ �ý���=======================
        playerInput = GetComponent<PlayerInput>();
        
        mainActionMap = playerInput.actions.FindActionMap("PlayerActions");//ActionMap ����
        
        moveAction = mainActionMap.FindAction("Move");//Move, FindAction ����
        moveYAction = mainActionMap.FindAction("MoveY");
        rotateCamAction = mainActionMap.FindAction("RotateCam");
        zoomCamAction = mainActionMap.FindAction("ZoomCam");

        //Move �׼� �̺�Ʈ ����
        moveAction.started += ctx => OnMove();
        moveAction.performed += ctx => Move(ctx);
        moveAction.canceled += ctx => OffMove();

        //MoveY �׼� �̺�Ʈ ����
        moveYAction.started += ctx => OnMoveY();
        moveYAction.performed += ctx => MoveY(ctx);
        moveYAction.canceled += ctx => OffMoveY();

        //rotateCam �׼� �̺�Ʈ ����
        rotateCamAction.performed += ctx => RotateCam(ctx);
        rotateCamAction.canceled += ctx => OffRotateCam();

        //rotateCam �׼� �̺�Ʈ ����
        zoomCamAction.performed += ctx => ZoomCam(ctx);
        zoomCamAction.canceled += ctx => OffZoomCam();
    }

    void Update()
    {

        if(rotDir != 0)
        {
            rotY.y += rotDir;
            transform.rotation = Quaternion.Euler(rotY);
        }
        if (moveYDir != Vector3.zero)
        {
            rotateSpeed = (rotateSpeed < rotateSpeedMax) ? rotateSpeed += rotateSpeedAcc * Time.deltaTime : rotateSpeedMax;
            swimSpeed = (swimSpeed < swimSpeedMax) ? swimSpeed += swimSpeedAcc * Time.deltaTime : swimSpeedMax;

            transform.rotation= Quaternion.Euler(new Vector3(rotYDir.x * -90f, rotY.y, 0) );
            transform.position += (moveYDir * swimSpeed * Time.deltaTime);
        }
        else if (moveDir!=0)
        {
            rotateSpeed = (rotateSpeed < rotateSpeedMax) ? rotateSpeed += rotateSpeedAcc * Time.deltaTime : rotateSpeedMax;
            swimSpeed = (swimSpeed < swimSpeedMax) ? swimSpeed += swimSpeedAcc * Time.deltaTime : swimSpeedMax;

            transform.rotation = Quaternion.Euler(rotY);
            transform.position+=(transform.forward*moveDir * swimSpeed * Time.deltaTime);
        }
        else
        {
            rotateSpeed = (rotateSpeed > rotateSpeedMin) ? rotateSpeed -= rotateSpeedAcc * Time.deltaTime : rotateSpeedMin;
            swimSpeed = (swimSpeed > swimSpeedMin) ? swimSpeed -= swimSpeedAcc * Time.deltaTime : swimSpeedMin;
        }

        LookAround();
        ZoomCamera();
    }

    #region C#_EVENTS
    void OnMove()
    {
        SetAnim("Swim");
    }

    void Move(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        moveDir = dir.y;
        rotDir = dir.x;
        Debug.Log(rotDir);
    }

    void OffMove()
    {
        moveDir = 0;
        rotDir = 0;
        if (moveDir != 0 || rotDir != 0) return;
        transform.rotation = Quaternion.Euler(rotY);
        SetAnim("Idle");
    }

    void OnMoveY()
    {
        SetAnim("Swim");
    }

    void MoveY(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        moveYDir.y = dir;
        rotYDir.x = dir;
    }

    void OffMoveY()
    {
        moveYDir.y = 0;
        rotYDir.x = 0;
        if (moveDir != 0 || rotDir != 0) return;
        transform.rotation = Quaternion.Euler(rotY);
        SetAnim("Idle");
    }
    void RotateCam(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>();
    }

    void OffRotateCam()
    {
        mouseDelta = Vector2.zero;
    }
    void ZoomCam(InputAction.CallbackContext ctx)
    {
        mouseScroll = ctx.ReadValue<Vector2>();
    }

    void OffZoomCam()
    {
        mouseScroll = Vector2.zero;
    }
    #endregion

    void SetAnim(string str)
    {
        if (allLodAnimator[0].GetCurrentAnimatorStateInfo(0).IsName(str)) return;
        for(int i=0;i< allLodAnimator.Length;i++)
        {
            allLodAnimator[i].SetTrigger(str);
        }
    }

    void LookAround()//���콺 �����ӿ� ���� ī�޶� ȸ��
    {
        Vector3 cameraAngle = cameraArm.rotation.eulerAngles;
        float x = cameraAngle.x - mouseDelta.y;
        x = (x < 180f) ? Mathf.Clamp(x, -40f, 40f) : Mathf.Clamp(x, 335f, 361f);
        cameraArm.rotation = Quaternion.Euler(x, cameraAngle.y  + mouseDelta.x*0.1f, cameraAngle.z);
    }
    
    void ZoomCamera()//���콺 ��ũ�ѿ� ���� ī�޶� ����/�ƿ�
    {
        float zoom = 0;
        zoom += mouseScroll.y;
        cameraArm.GetComponentInChildren<Camera>().fieldOfView += zoom;
        cameraArm.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(cameraArm.GetComponentInChildren<Camera>().fieldOfView, 55, 120);
        //Debug.Log(mouseScroll.y);
    }
}
