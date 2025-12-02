using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerHitCollider : MonoBehaviour
{
    public PlayerController controller;
    public BoostImageContro boostContro;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Sound.PlaySound("Sound/distorKick");
            var arrow = collision.gameObject.GetComponent<Arrow>();
            if (arrow != null)
            {
                switch (arrow.arrowType)
                {
                    case Arrow.ArrowType.UpArrow:
                        if (controller) controller.OnArrowUp();
                        break;
                    case Arrow.ArrowType.DownArrow:
                        break;
                    case Arrow.ArrowType.LeftArrow:
                        break;
                    case Arrow.ArrowType.RightArrow:
                        if (controller) controller.OnArrowRight();
                        break;
                }
            }
        }
        else if (collision.gameObject.name.Contains("BoostBattery"))
        {
            Sound.PlaySound("Sound/BoostBatterySfx");
            boostContro.RestartReduction();
        }
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
            // ¼ì²éÅö×²±êÇ©
         if (collision.gameObject.CompareTag("HorHit")&&!controller.isCheckVec )
        {
            Sound.PlaySound("Sound/PlayerHit");
            if (controller) controller.HandleHorHitCollision();
        }
        else if (collision.gameObject.CompareTag("VecHit") || collision.gameObject.CompareTag("Monster") && !controller.isCheckVec)
        {
            Sound.PlaySound("Sound/PlayerHit");
            if (controller) controller.isCheckVec = true;
            if (controller) controller.HandleVecHitCollision();
        }
        else if (collision.gameObject.CompareTag("DownHit") && !controller.isCheckVec)
        {
            Sound.PlaySound("Sound/PlayerHit");
            if (controller) controller.HandleDownHitCollision();
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            Sound.PlaySound("Sound/PlayerHit");
            if (controller) controller.gameObject.SetActive(false);
            EventManager.Instance.SendMessage(Events.PlayerRestToSavePos);
            EventManager.Instance.SendMessage(Events.GameRest);
        }
    }
}
