using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BatteryUI : MonoBehaviour
{
    public Sprite[] spr;
    Image batteryImage;
    // Start is called before the first frame update
    void Start()
    {
        batteryImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.ins.batteryCount > 90)
        {
            batteryImage.sprite = spr[0];
        }else if (GameManager.ins.batteryCount > 80)
        {
            batteryImage.sprite = spr[1];
        }
        else if (GameManager.ins.batteryCount > 60)
        {
            batteryImage.sprite = spr[2];
        }
        else if (GameManager.ins.batteryCount > 30)
        {
            batteryImage.sprite = spr[3];
        }
        else if (GameManager.ins.batteryCount > 10)
        {
            batteryImage.sprite = spr[4];
        }
        else if (GameManager.ins.batteryCount == 0)
        {
            batteryImage.sprite = spr[5];
        }
    }
}
