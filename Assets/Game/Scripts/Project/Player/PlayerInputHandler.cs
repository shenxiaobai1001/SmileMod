using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private NewActions inptutControl;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inptutControl = new NewActions();
        inptutControl.InputContro.LeftJump.performed += ctx => LeftJump();
        inptutControl.InputContro.RightJump.performed += ctx => RightJump();
        inptutControl.InputContro.LeftMove.performed += ctx => LeftMove();
        inptutControl.InputContro.RightMove.performed += ctx => RightMove();
        inptutControl.InputContro.LeftMove.canceled += ctx => CLeftMove();
        inptutControl.InputContro.RightMove.canceled += ctx => CRightMove();
        inptutControl.InputContro.Jump.performed += ctx => Jump();
        inptutControl.InputContro.Jump.canceled += ctx => CJump();
        inptutControl.InputContro.Boost.performed += ctx => Boost();
        inptutControl.InputContro.Boost.canceled += ctx => CBoost();

    }

    private void Start()
    {
        
    }
    
    void LeftMove()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        bool protect = ModSystemController.Instance.Protecket;
        if (ModSystemController.Instance.reverseJump && !protect)
        {
            playerController.OnPlayerControStateChange(PlayerControState.RRun);
        }
        else
        {
            playerController.OnPlayerControStateChange(PlayerControState.LRun);
        }
    }
    void RightMove()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        bool protect = ModSystemController.Instance.Protecket;
        if (ModSystemController.Instance.reverseJump && !protect)
        {
            playerController.OnPlayerControStateChange(PlayerControState.LRun);
        }
        else
        {
            playerController.OnPlayerControStateChange(PlayerControState.RRun);
        }
    }
    void CLeftMove()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        bool protect = ModSystemController.Instance.Protecket;
         if (ModSystemController.Instance.reverseJump&&!protect)
        {
            playerController.OnPlayerControStateChange(PlayerControState.CanelRRun);
        }
        else
        {
            playerController.OnPlayerControStateChange(PlayerControState.CanelLRun);
        }

    }
    void CRightMove()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        bool protect = ModSystemController.Instance.Protecket;
        if (ModSystemController.Instance.reverseJump && !protect)
        {
            playerController.OnPlayerControStateChange(PlayerControState.CanelLRun);
        }
        else
        {
            playerController.OnPlayerControStateChange(PlayerControState.CanelRRun);
        }
    }

    void LeftJump()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        bool protect = ModSystemController.Instance.Protecket;
        if (ModSystemController.Instance.reverseJump && !protect)
        {
            playerController.OnPlayerControStateChange(PlayerControState.RStickJump);
        }
        else
        {
            playerController.OnPlayerControStateChange(PlayerControState.LStickJump);
        }
     
    }

    void RightJump()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        bool protect = ModSystemController.Instance.Protecket;
        if (ModSystemController.Instance.reverseJump && !protect)
        {
            playerController.OnPlayerControStateChange(PlayerControState.LStickJump);
        }
        else
        {
            playerController.OnPlayerControStateChange(PlayerControState.RStickJump);
        }
    }
    void Jump()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        playerController.OnPlayerControStateChange(PlayerControState.Jump);
    }

    void CJump()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        playerController.OnPlayerControStateChange(PlayerControState.CJump);
    }
    void Boost()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        playerController.OnPlayerControStateChange(PlayerControState.Boost);
    }

    void CBoost()
    {
        if (playerController.isHit) return;
        playerController.OnPlayerControStateChange(PlayerControState.CanelBoost);
    }
    void FastMove()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        playerController.OnPlayerControStateChange(PlayerControState.ToFast);
    }

    void CBoostFastMove()
    {
        if (GameController.Instance.isAutomatic) return;
        if (playerController.isHit) return;
        playerController.OnPlayerControStateChange(PlayerControState.CancelFast);
    }
    private void OnEnable()
    {
        inptutControl.InputContro.Enable();
    }

    private void OnDisable()
    {
        inptutControl.InputContro.Disable();
    }
}