using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEdit : MonoBehaviour
{
    private int TargetId;
    private int NoteIdOfBox;
    private AddChart.NoteData NoteInfo;
    private GameObject targetObject;

    public EditController Editor;

    public string type;
    public Text targetbox;
    public Text timeStart;
    public Text timeEnd;
    public InputField beatStart;
    public InputField beatEnd;
    public InputField XOffset;
    public InputField YOffset;
    public InputField SpeedOffset;
    public InputField AngleOffset;

    public Image colorDisplayImage;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Slider alphaSlider;
    public InputField redInput;
    public InputField greenInput;
    public InputField blueInput;
    public InputField alphaInput;

    public Button delete;
    void Start()
    {
        gameObject.SetActive(false);
        beatStart.text = "0";
        beatEnd.text = "0";

        redInput.onValueChanged.AddListener(ColorChangeInput);
        blueInput.onValueChanged.AddListener(ColorChangeInput);
        greenInput.onValueChanged.AddListener(ColorChangeInput);
        alphaInput.onValueChanged.AddListener(ColorChangeInput);

        redSlider.onValueChanged.AddListener(ColorChangeSlider);
        blueSlider.onValueChanged.AddListener(ColorChangeSlider);
        greenSlider.onValueChanged.AddListener(ColorChangeSlider);
        alphaSlider.onValueChanged.AddListener(ColorChangeSlider);

        beatStart.onValueChanged.AddListener(ChangeTime);
        beatEnd.onValueChanged.AddListener(ChangeTime);

        beatStart.onEndEdit.AddListener(JudgeTime);
        beatEnd.onEndEdit.AddListener(JudgeTime);
        delete.onClick.AddListener(OnDelete);
    }
    public void ChangeTime(string change)
    {
        timeStart.text = (float.Parse(beatStart.text)/(Editor.chart.bpm / 60)).ToString();
        if(type=="Hold")timeEnd.text = (float.Parse(beatEnd.text) / (Editor.chart.bpm / 60)).ToString();

        if (type != "Hold")
        {
            var V = targetObject.transform.localPosition;
            V.y = float.Parse(beatStart.text) * 50;
            targetObject.transform.localPosition = V;
        }
        else
        {
            var y1 = float.Parse(beatStart.text) * 50;
            var y2 = float.Parse(beatEnd.text) * 50;
            var V = targetObject.transform.localPosition;
            V.y = (y1 + y2) / 2;
            var S = targetObject.transform.localScale;
            S.y = y2 - y1;
            targetObject.transform.localPosition = V;
            targetObject.transform.localScale = S;
        }
    }
    public void JudgeTime(string change)
    {
        if (type == "Hold")
        {
            if (float.Parse(timeStart.text) > float.Parse(timeEnd.text))
            {
                timeEnd.text = timeStart.text;
                beatEnd.text = beatStart.text;
            }
        }
       
        
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
        if (float.TryParse(redInput.text, out val))
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
    public void LoadInfo(int boxid,int notePos)
    {
        TargetId = boxid;
        NoteIdOfBox = notePos;
        NoteInfo = Editor.Notes[boxid][notePos];
        targetObject = Editor.Notes_WatchAble[boxid][notePos];
        type = NoteInfo.type;
        targetbox.text = boxid.ToString();
        timeStart.text = NoteInfo.time_start.ToString();
        timeEnd.text = NoteInfo.time_end.ToString();
        beatStart.text = NoteInfo.beat_start.ToString();
        beatEnd.text = NoteInfo.beat_end.ToString();
        XOffset.text = NoteInfo.xoffset.ToString();
        YOffset.text = NoteInfo.yoffset.ToString();
        SpeedOffset.text = NoteInfo.speedoffset.ToString();
        AngleOffset.text = NoteInfo.angleoffset.ToString();
        redSlider.value = NoteInfo.color.r; redInput.text = NoteInfo.color.r.ToString();
        blueSlider.value = NoteInfo.color.b; blueInput.text = NoteInfo.color.b.ToString();
        greenSlider.value = NoteInfo.color.g; greenInput.text = NoteInfo.color.g.ToString();
        alphaSlider.value = NoteInfo.color.a; alphaInput.text = NoteInfo.color.a.ToString();

        if (type != "Hold")
        {
            beatEnd.interactable = false;
        }
        else beatEnd.interactable = true;
    }

    private void OnDelete()
    {
        Editor.Notes[TargetId].RemoveAt(NoteIdOfBox);
        Destroy(Editor.Notes_WatchAble[TargetId][NoteIdOfBox]);
        Editor.Notes_WatchAble[TargetId].RemoveAt(NoteIdOfBox);
        Editor.NoteEditor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void UpdateInfo()
    {
        NoteInfo.xoffset = float.Parse(XOffset.text);
        NoteInfo.yoffset = float.Parse(YOffset.text);
        NoteInfo.speedoffset = float.Parse(SpeedOffset.text);
        NoteInfo.angleoffset = float.Parse(AngleOffset.text);
        NoteInfo.color = colorDisplayImage.color;
        NoteInfo.time_start = float.Parse(timeStart.text);
        NoteInfo.time_end = float.Parse(timeEnd.text);
        NoteInfo.beat_start = float.Parse(beatStart.text);
        NoteInfo.beat_end = float.Parse(beatEnd.text);
        Editor.Notes[TargetId][NoteIdOfBox] = NoteInfo;
    }
}
