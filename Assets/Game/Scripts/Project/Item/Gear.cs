using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sawwer;

public class Gear : MonoBehaviour
{
    public enum GearType { Gear1, Gear2 }
    public GearType gearType = GearType.Gear1; // 选择显示的齿轮
    public Transform grearPos;
    public GameObject grear1;
    public GameObject grear2;
    public GameObject Collider1;
    public GameObject Collider2;

    float rotationSpeed = 180f;    // 旋转速度(度/秒)

    private void Start()
    {
        Collider1.SetActive(gearType== GearType.Gear1);
        Collider2.SetActive(gearType == GearType.Gear2);
        grear1.SetActive(gearType == GearType.Gear1);
        grear2.SetActive(gearType == GearType.Gear2);
    }

    private void Update()
    {
        // 旋转齿轮
        RotateSaw();
    }
    // 旋转齿轮
    private void RotateSaw()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        grearPos.transform.Rotate(0f, 0f, -rotationAmount);
    }
}
