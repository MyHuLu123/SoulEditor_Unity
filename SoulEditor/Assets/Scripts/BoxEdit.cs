using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BoxEdit : MonoBehaviour
{
    public int Box_ID;
    public EditController Editor;

    public Text boxid;
    public InputField boxX;
    public InputField boxY;
    public InputField boxSpeed;
    public InputField boxAngle;

    public Image colorDisplayImage;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Slider alphaSlider;
    public InputField redInput;
    public InputField greenInput;
    public InputField blueInput;
    public InputField alphaInput;
    void Start()
    {
        Box_ID = -1;
        gameObject.SetActive(false);
        redInput.onValueChanged.AddListener(ColorChangeInput);
        blueInput.onValueChanged.AddListener(ColorChangeInput);
        greenInput.onValueChanged.AddListener(ColorChangeInput);
        alphaInput.onValueChanged.AddListener(ColorChangeInput);

        redSlider.onValueChanged.AddListener(ColorChangeSlider);
        blueSlider.onValueChanged.AddListener(ColorChangeSlider);
        greenSlider.onValueChanged.AddListener(ColorChangeSlider);
        alphaSlider.onValueChanged.AddListener(ColorChangeSlider);
    }
    public void ChangeColorSprite()
    {
        if (colorDisplayImage != null)
        {
            colorDisplayImage.color = new Color(
                redSlider.value,
                greenSlider.value,
                blueSlider.value,
                alphaSlider.value
            );
        }
    }
    public void ColorChangeInput(string change)   //当InputField的内容改变时，修改slider
    {
        float val;
        if(float.TryParse(redInput.text,out val))
        {
            redSlider.value = val;
        }
        if (float.TryParse(blueInput.text, out val))
        {
            blueSlider.value = val;
        }
        if (float.TryParse(greenInput.text, out val))
        {
            greenSlider.value = val;
        }
        if (float.TryParse(alphaInput.text, out val))
        {
            alphaSlider.value = val;
        }
        ChangeColorSprite();
    }
    public void ColorChangeSlider(float change)   //当slider的内容改变时，修改InputField
    {
        redInput.text = redSlider.value.ToString();
        blueInput.text = blueSlider.value.ToString();
        greenInput.text = greenSlider.value.ToString();
        alphaInput.text = alphaSlider.value.ToString();
        ChangeColorSprite();
    }
    public void LoadInfo()
    {
        AddChart.BoxData GetInfo = Editor.Boxes[Box_ID];

        boxX.text = GetInfo.x.ToString();
        boxY.text = GetInfo.y.ToString();
        boxSpeed.text = GetInfo.speed.ToString();
        boxAngle.text = GetInfo.angle.ToString();
        redSlider.value = GetInfo.color.r; redInput.text = GetInfo.color.r.ToString();
        blueSlider.value = GetInfo.color.b; blueInput.text = GetInfo.color.b.ToString();
        greenSlider.value = GetInfo.color.g; greenInput.text = GetInfo.color.g.ToString();
        alphaSlider.value = GetInfo.color.a; alphaInput.text = GetInfo.color.a.ToString();
        boxid.text = Box_ID.ToString();
    }

    public void UpdateInfo()
    {
        AddChart.BoxData UpdateInfo = new AddChart.BoxData();
        UpdateInfo.x = float.Parse(boxX.text);
        UpdateInfo.y = float.Parse(boxY.text);
        UpdateInfo.speed = float.Parse(boxSpeed.text);
        UpdateInfo.angle = float.Parse(boxAngle.text);
        UpdateInfo.color = colorDisplayImage.color;
        Editor.Boxes[Box_ID] = UpdateInfo;
    }
    // Update is called once per frame
}
