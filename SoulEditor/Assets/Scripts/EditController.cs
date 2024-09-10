using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class EditController : MonoBehaviour
{
    public Slider CameraSlider;
    public float MinCameraSize;
    public float MaxCameraSize;

    public GameObject Square;
    public GameObject Border;
    public GameObject EditCanvas;
    public GameObject ViewCanvas;
    public GameObject EditObject;
    public GameObject ViewObject;

    public Dropdown BeatType;
    public InputField PerBeat;
    public InputField BeatSum;
    public Dropdown Tween;
    public Dropdown Ease;
    public RectTransform Base;
    public Toggle TapAndHold;
    public Toggle Drag;

    public Button AddBox;
    public Button View;
    public Button Save;
    public Toggle AddNote;
    public Toggle Choose;
    public Toggle Event;

    public BoxEdit BoxObject;
    public NoteEdit NoteEditor;

    public AddChart.ChartData chart;

    private string[] Info;
    private string Folderpath;

    [HideInInspector] public GameObject Borders;
    [HideInInspector] public GameObject NotesSquare;
    [HideInInspector] public List<GameObject> Lines;
    [HideInInspector] public List<GameObject> BoxLines;
    [HideInInspector] public List<AddChart.BoxData> Boxes;
    public List<List<AddChart.NoteData>> Notes = new List<List<AddChart.NoteData>>();
    public List<List<GameObject>> Notes_WatchAble = new List<List<GameObject>>();

    private Vector3 MouseStartPos;
    private Vector3 NowMousePos;

    [HideInInspector] public float songLength;
    private int beats;
    private int NowPerBeat = 2;
    private int NowBeatSum = 4;

    private Toggle NowNoteType;
    public AudioClip song;
    private List<GameObject> Show_cubes = new List<GameObject>();
    private int SquareSum;
    private bool Chosen = false;
    private bool NoteTimeChosen = false;
    private int ChooseBox = -1;
    private double NowNoteStartTime;
    private double NowNoteStartBeat;
    private GameObject NowNote;

    private string chartpath;
    // Start is called before the first frame update

    void Start()
    {
        string path = Application.persistentDataPath;
        if (!File.Exists(path + "/LoadLevel.txt"))
        {
            Debug.LogError("Cannot Find the chart file");
        }
        Info = File.ReadAllLines(path + "/LoadLevel.txt");
        Folderpath = path + "/" + Info[0];
        chartpath = Folderpath + "/" + Info[1];
        if (File.Exists(chartpath))
        {
            string jsonData = File.ReadAllText(chartpath);
            chart = JsonUtility.FromJson<AddChart.ChartData>(jsonData);
        }
        else
        {
            Debug.LogError("Couldn't Find chart file!");
        }
        StartCoroutine(GetAudioDuration());
        float width = Base.sizeDelta.x;
        float height = Base.sizeDelta.y;
        SquareSum = (int)width;
        for (int i = 0; i <= SquareSum; i++)
        {
            GameObject squ = new GameObject("Panel");
            Show_cubes.Add(squ);
            Image panelImage = squ.AddComponent<Image>();
            panelImage.color = Color.white; // 设置Panel的背景颜色
            squ.transform.SetParent(Base,false);
            RectTransform rec =  squ.GetComponent<RectTransform>();
            rec.anchoredPosition = new Vector2(-width/2+ width * (float)(1.0 * i / SquareSum), -height/2 + height* (float)(1.0 * i / SquareSum)); // 设置Panel的锚点位置
            rec.sizeDelta = new Vector2(3, 3); // 设置Panel的
            
        }
        //Box分割线
        GameObject line = Instantiate(Border);line.transform.position = new Vector3(0,0,0);
        line.transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
        line.transform.parent = EditObject.transform;
        FixOnCamera O = line.AddComponent<FixOnCamera>();O.fixY = true;
        BoxLines.Add(line);
        for (int i = 0; i < chart.boxnum; i++)
        {
            Boxes.Add(chart.boxes[i]);
            line = Instantiate(Border); line.transform.position = new Vector3((i+1)*8, 0, 0);
            line.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            O = line.AddComponent<FixOnCamera>(); O.fixY = true;
            BoxLines.Add(line);
            Notes.Add(new List<AddChart.NoteData>());
            Notes_WatchAble.Add(new List<GameObject>());
        }
        for(int i = 0; i < chart.notenum; i++)
        {
            AddChart.NoteData Ntdt = chart.notes[i];
            Notes[Ntdt.targetbox].Add(Ntdt);

            if(Ntdt.type == "Drag")
            {
                GameObject newNote = Instantiate(Square);
                newNote.transform.position = new Vector3((float)(Ntdt.targetbox + 0.5) * 8, (float)Ntdt.beat_start * 50, -5);
                newNote.transform.localScale = new Vector3(6, 1, 1);
                newNote.transform.parent = EditObject.transform;
                newNote.GetComponent<SpriteRenderer>().color = Color.yellow;
                newNote.AddComponent<BoxCollider>();
                Notes_WatchAble[Ntdt.targetbox].Add(newNote);
            }
            else if(Ntdt.type == "Tap")
            {
                GameObject newNote = Instantiate(Square);
                newNote.transform.position = new Vector3((float)(Ntdt.targetbox + 0.5) * 8, (float)Ntdt.beat_start * 50, -5);
                newNote.transform.localScale = new Vector3(6, 1, 1);
                newNote.transform.parent = EditObject.transform;
                newNote.GetComponent<SpriteRenderer>().color = Color.blue;
                newNote.AddComponent<BoxCollider>();
                Notes_WatchAble[Ntdt.targetbox].Add(newNote);
            }
            else
            {
                GameObject newNote = Instantiate(Square);
                float y1 = (float)Ntdt.beat_start * 50;
                float y2 = (float)Ntdt.beat_end * 50;
                newNote.transform.position = new Vector3((float)(Ntdt.targetbox + 0.5) * 8,(y1+y2)/2, -5);
                newNote.transform.localScale = new Vector3(6, (y2-y1), 1);
                newNote.transform.parent = EditObject.transform;
                newNote.GetComponent<SpriteRenderer>().color = Color.blue;
                newNote.AddComponent<BoxCollider>();
                Notes_WatchAble[Ntdt.targetbox].Add(newNote);
            }
        }
        BeatType.onValueChanged.AddListener(ChangeBeatType);
        Tween.onValueChanged.AddListener(ChangeAnimShow);
        Ease.onValueChanged.AddListener(ChangeAnimShow);
        AddBox.onClick.AddListener(ChangeAddBox);
        View.onClick.AddListener(ViewMode);
        Save.onClick.AddListener(SaveChart);
    }
    private void ViewMode()
    {
        if(BoxObject.gameObject.activeSelf==true) BoxObject.UpdateInfo();
        SaveEdit();
        ViewCanvas.SetActive(true);
        ViewCanvas.GetComponent<ViewControl>().LoadView();
        ViewObject.SetActive(true);
        EditCanvas.gameObject.SetActive(false);
        EditObject.SetActive(false);
        this.gameObject.SetActive(false);

    }
    private void ChangeBeatType(int index)
    {
        string type = BeatType.options[index].text;
        switch (type)
        {
            case "2/4":
                AdjustLines(2, 4);
                NowPerBeat = 2;
                NowBeatSum = 4;
                break;
            case "3/4":
                AdjustLines(3, 4);
                NowPerBeat = 3;
                NowBeatSum = 4;
                break;
            case "4/4":
                AdjustLines(4, 4);
                NowPerBeat = 4;
                NowBeatSum = 4;
                break;
            case "6/8":
                AdjustLines(6, 8);
                NowPerBeat = 6;
                NowBeatSum = 8;
                break;
            case "others":
                string text_perbeat = PerBeat.text;
                string text_beatsum = BeatSum.text;
                int val_perbeat, val_beatsum;
                if (!int.TryParse(text_perbeat, out val_perbeat) && val_perbeat<=0)
                {
                    Debug.LogError("Wrong PerBeat!");
                    BeatType.value = 0;
                    AdjustLines(2, 4);
                    return;
                }
                if (!int.TryParse(text_beatsum, out val_beatsum) && val_beatsum<=0)
                {
                    Debug.LogError("Wrong BeatSum!");
                    BeatType.value = 0;
                    AdjustLines(2, 4);
                    return;
                }
                AdjustLines(val_perbeat, val_beatsum);
                NowPerBeat = val_perbeat;
                NowBeatSum = val_beatsum;
                break;
        }
    }

    private void ChangedBorder(GameObject line,int i,int perbeat,int beatsum)
    {
        line.transform.position = new Vector3(0, i * 50 / beatsum, 1);
        line.transform.parent = Borders.transform;
        SpriteRenderer spr = line.GetComponentInChildren<SpriteRenderer>();
        if (i % perbeat == 0)
        {
            spr.color = new Color32(0, 44, 154, 255);
        }
        else
        {
            spr.color = Color.black;
        }
        TextMesh text = line.GetComponentInChildren<TextMesh>();
        if (text != null)
        {
            int whole = i / beatsum;
            int left = i % beatsum;
            // 修改文本
            string T = whole.ToString();
            if (left != 0)
            {
                T += " + " + left.ToString() + "/" + beatsum.ToString();
       
            }
            text.text = T;
            if(i % perbeat == 0)
            {
                text.color = new Color32(0, 44, 154, 255);
            }
            else
            {
                text.color = Color.black;
            }
        }
    }
    private void AdjustLines(int perbeat,int beatsum)
    {
        int L = Lines.Count;
        int newsum = beats * beatsum;
        if(newsum <= L)
        {
            for(int i = 0; i < newsum; i++)
            {
                ChangedBorder(Lines[i], i, perbeat, beatsum);
            }
            Lines.RemoveRange(newsum,L-newsum);
        }
        else
        {
            for(int i = 0; i < newsum; i++)
            {
                if (i < L)
                {
                    ChangedBorder(Lines[i], i, perbeat, beatsum);
                }
                else
                {
                    GameObject line = Instantiate(Border);
                    FixOnCamera O = line.AddComponent<FixOnCamera>();
                    O.fixX = true;
                    ChangedBorder(line, i, perbeat, beatsum);
                    Lines.Add(line);
                }
            
            }
        }
    }
    IEnumerator GetAudioDuration()
    {
        // 使用UnityWebRequest下载MP3文件
        string pMusicPath = "";
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            pMusicPath = @"file://" + Folderpath + "/" + Info[2]+".mp3";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            pMusicPath = @"file:///" + Folderpath + "/" + Info[2] + ".mp3";
        }
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(pMusicPath, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.responseCode != 200)
        {
            Debug.LogError("Error: " + www.error);
            yield break;
        }

        // 获取AudioClip
       song = DownloadHandlerAudioClip.GetContent(www);
       if(song != null)
        {
            songLength = song.length;
            float length = songLength * chart.bpm / 60 + 1;
            beats = Mathf.RoundToInt(length);
            Borders = new GameObject("Borders");
            NotesSquare = new GameObject("NotesSquare");
            NotesSquare.transform.position = Borders.transform.position = new Vector3(0, 0, 0);
            NotesSquare.transform.parent = Borders.transform.parent = EditObject.transform;
            int beatsum = 4;int perbeat = 2;
            for (int i = 0; i < beats*4; i++)
            {
                GameObject line = Instantiate(Border);
                FixOnCamera O = line.AddComponent<FixOnCamera>();
                O.fixX = true;
                ChangedBorder(line, i, perbeat, beatsum);
                Lines.Add(line);
            }
        }
    }

    private void ChangeAnimShow(int index)
    {
        int ease = Ease.value;
        switch (Tween.options[Tween.value].text)
        {
            case "Linear":
                for (int i = 0; i < Show_cubes.Count; i++)
                {
                    ChangeSquareSingle(i, 1.0 * i / SquareSum);
                }
                break;
            case "Sine":
                switch (ease){
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInSine(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutSine(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutSine(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Quad":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInQuad(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutQuad(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutQuad(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Cubic":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInCubic(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutCubic(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutCubic(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Quart":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInQuart(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutQuart(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutQuart(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Quint":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInQuint(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutQuint(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutQuint(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Expo":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInExpo(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutExpo(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutExpo(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Circ":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInCirc(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutCirc(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutCirc(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Back":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInBack(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutBack(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutBack(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Elastic":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInElastic(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutElastic(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutBack(1.0 * i / SquareSum));
                        break;
                }
                break;
            case "Bounce":
                switch (ease)
                {
                    case 0:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInBounce(1.0 * i / SquareSum));
                        break;
                    case 1:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseOutBounce(1.0 * i / SquareSum));
                        break;
                    case 2:
                        for (int i = 0; i < Show_cubes.Count; i++) ChangeSquareSingle(i, AnimationCollection.EaseInOutBounce(1.0 * i / SquareSum));
                        break;
                }
                break;

        }
    }
    private void ChangeSquareSingle(int index,double pro)
    {
        float width = Base.sizeDelta.x;
        float height = Base.sizeDelta.y;
        RectTransform rec = Show_cubes[index].GetComponent<RectTransform>();
        rec.anchoredPosition = new Vector2(-width / 2 + width * (float)(1.0 * index / SquareSum), -height / 2 + height * (float)pro);
        
    }

    private void ChangeAddBox()
    {
        AddChart.BoxData NewBox = new AddChart.BoxData();
        NewBox.x = NewBox.y = 0;
        NewBox.angle = 90;
        NewBox.color = Color.white;
        NewBox.speed = 10;
        Boxes.Add(NewBox);

        GameObject line = Instantiate(Border); line.transform.position = new Vector3(Boxes.Count * 8, 0, 0);
        line.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        line.transform.parent = EditObject.transform;
        FixOnCamera O = line.AddComponent<FixOnCamera>(); O.fixY = true;
        BoxLines.Add(line);
        Notes.Add(new List<AddChart.NoteData>());
        Notes_WatchAble.Add(new List<GameObject>());
    }
    private void CheckPresses()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < 1185)
            {
                
                Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int id = (int)(ClickPos.x / 8);
                if (id >= 0 && id < Boxes.Count)
                {
                    if (id == ChooseBox && AddNote.isOn)
                    {
                        BoxObject.gameObject.SetActive(false); BoxObject.UpdateInfo();
                        double beat = 1.0 * (int)(ClickPos.y * NowBeatSum / 50 + 0.5) / NowBeatSum;
                        double time = beat / (1.0 * chart.bpm / 60);
                        if (Drag.isOn)
                        {
                            AddChart.NoteData newDrag = new AddChart.NoteData();
                            newDrag.angleoffset = newDrag.speedoffset = newDrag.xoffset = newDrag.yoffset = 0;
                            newDrag.type = "Drag";
                            newDrag.targetbox = ChooseBox;
                            newDrag.color = Color.white;
                            newDrag.beat_start = beat;
                            newDrag.time_start = time;
                            GameObject newDragWatchAble = Instantiate(Square);
                            newDragWatchAble.transform.position = new Vector3((float)(ChooseBox + 0.5) * 8, (float)beat * 50, -5);
                            newDragWatchAble.transform.localScale = new Vector3(6, 1, 1);
                            newDragWatchAble.transform.parent = EditObject.transform;
                            newDragWatchAble.GetComponent<SpriteRenderer>().color = Color.yellow;
                            newDragWatchAble.AddComponent<BoxCollider>();
                            Notes[ChooseBox].Add(newDrag);
                            Notes_WatchAble[ChooseBox].Add(newDragWatchAble);
                            Debug.Log(NoteEditor.gameObject.activeSelf);
                            if (NoteEditor.gameObject.activeSelf == true)
                            {
                                NoteEditor.UpdateInfo();
                            }
                            NoteEditor.gameObject.SetActive(true);
                            NoteEditor.LoadInfo(ChooseBox, Notes[ChooseBox].Count - 1);
                        }
                        else if (TapAndHold.isOn)
                        {
                            NoteTimeChosen = true;
                            NowNoteStartTime = time;
                            NowNoteStartBeat = beat;
                            NowNote = Instantiate(Square);
                            NowNote.transform.position = new Vector3((float)(ChooseBox + 0.5) * 50, (float)beat * 50, -5);
                            NowNote.transform.localScale = new Vector3(6, 1, 1);
                            NowNote.GetComponent<SpriteRenderer>().color = Color.blue;
                        }
                        else Chosen = false;
                    }
                    else if (Choose.isOn)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
                        {
                            var HitObject = hit.collider.gameObject;
                            bool find = false;
                            for (int i = 0; i < Notes.Count && !find; i++)
                            {
                                for (int j = 0; j < Notes[i].Count && !find; j++)
                                {
                                    if (Notes_WatchAble[i][j] == HitObject)
                                    {
                                        find = true;
                                        Chosen = false;
                                        ChooseBox = -1;
                                        if (BoxObject.gameObject.activeSelf == true)
                                        {
                                            BoxObject.UpdateInfo();
                                        }
                                        if (NoteEditor.gameObject.activeSelf == true)
                                        {
                                            NoteEditor.UpdateInfo();
                                        }
                                        BoxObject.gameObject.SetActive(false);
                                        NoteEditor.gameObject.SetActive(true);
                                        NoteEditor.LoadInfo(i, j);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Chosen = true;
                            ChooseBox = id;
                            if (BoxObject.gameObject.activeSelf == true)
                            {
                                BoxObject.UpdateInfo();
                            }
                            if (NoteEditor.gameObject.activeSelf == true)
                            {
                                NoteEditor.UpdateInfo();
                            }
                            BoxObject.gameObject.SetActive(true);
                            NoteEditor.gameObject.SetActive(false);
                            BoxObject.Box_ID = ChooseBox;
                            BoxObject.LoadInfo();
                        }

                    }
                    else
                    {
                        ChooseBox = id;
                        Chosen = true;
                        if (BoxObject.gameObject.activeSelf == true)
                        {
                            BoxObject.UpdateInfo();
                        }
                        if (NoteEditor.gameObject.activeSelf == true)
                        {
                            NoteEditor.UpdateInfo();
                        }
                        NoteEditor.gameObject.SetActive(false);
                        BoxObject.gameObject.SetActive(true);
                        BoxObject.Box_ID = ChooseBox;
                        BoxObject.LoadInfo();
                    }

                }
                else
                    {
                        Chosen = false;
                        ChooseBox = -1;
                        if (BoxObject.gameObject.activeSelf == true)
                        {
                            BoxObject.UpdateInfo();
                        }
                        BoxObject.gameObject.SetActive(false);
                        if (NoteEditor.gameObject.activeSelf == true)
                        {
                            NoteEditor.UpdateInfo();
                        }
                        NoteEditor.gameObject.SetActive(false);

                    }
                
            }
        }
        else if (Input.GetMouseButton(0) && NoteTimeChosen && AddNote.isOn) 
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            double beat = 1.0 * (int)(MousePos.y * NowBeatSum / 50 + 0.5) / NowBeatSum;
            double time = beat / (1.0 * chart.bpm / 60);
            if (beat-NowNoteStartBeat<=0.01)
            {
                NowNote.transform.position = new Vector3((float)(ChooseBox + 0.5) * 8, (float)NowNoteStartBeat*50, -5);
                NowNote.transform.localScale = new Vector3(6, 1, 1);
            }
            else
            {
                NowNote.transform.position = new Vector3((float)(ChooseBox + 0.5) * 8, ((float)(NowNoteStartBeat +beat) * 50) / 2, -5);
                NowNote.transform.localScale = new Vector3(6, (float)(beat-NowNoteStartBeat) * 50, 1);
            }
        }
        else if(Input.GetMouseButtonUp(0) && NoteTimeChosen && AddNote.isOn)
        {
            Debug.Log(ChooseBox);Debug.Log(Notes_WatchAble.Count);
            NoteTimeChosen = false;
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            double beat = 1.0 * (int)(MousePos.y * NowBeatSum / 50 + 0.5) / NowBeatSum;
            double time = beat / (1.0 * chart.bpm / 60);
            if (beat - NowNoteStartBeat <= 0.01)
            {
                AddChart.NoteData newTap = new AddChart.NoteData();
                newTap.angleoffset = newTap.speedoffset = newTap.xoffset = newTap.yoffset = 0;
                newTap.type = "Tap";
                newTap.targetbox = ChooseBox;
                newTap.color = Color.white;
                newTap.beat_start = beat;
                newTap.time_start = time;
                NowNote.transform.parent = EditObject.transform;
                Notes[ChooseBox].Add(newTap);
                NowNote.AddComponent<BoxCollider>();
                Notes_WatchAble[ChooseBox].Add(NowNote);
                NoteEditor.gameObject.SetActive(true);
                NoteEditor.LoadInfo(ChooseBox, Notes[ChooseBox].Count - 1);
            }
            else
            {
                AddChart.NoteData newHold = new AddChart.NoteData();
                newHold.angleoffset = newHold.speedoffset = newHold.xoffset = newHold.yoffset = 0;
                newHold.type = "Hold";
                newHold.targetbox = ChooseBox;
                newHold.color = Color.white;
                newHold.beat_start = NowNoteStartBeat;newHold.beat_end = beat;
                newHold.time_start = NowNoteStartTime;newHold.time_end = time;
                NowNote.transform.parent = EditObject.transform;
                NowNote.AddComponent<BoxCollider>();
                Notes[ChooseBox].Add(newHold);
                Notes_WatchAble[ChooseBox].Add(NowNote);
                NoteEditor.gameObject.SetActive(true);
                NoteEditor.LoadInfo(ChooseBox, Notes[ChooseBox].Count - 1);
            }
        }
    }
    private void SaveEdit()//存储小规模的修改
    {
        chart.boxnum = Boxes.Count;
        chart.boxes = new AddChart.BoxData[chart.boxnum];
        for (int i=0;i< chart.boxnum; i++)
        {
            chart.boxes[i] = Boxes[i];
        }
        int Notenum = 0;
        List<AddChart.NoteData> NotesAll = new List<AddChart.NoteData>();
        for (int i = 0; i < Notes.Count; i++)
        {
            Notenum += Notes[i].Count;
            for(int j = 0; j < Notes[i].Count; j++)
            {
                NotesAll.Add(Notes[i][j]);
            }
        }
        NotesAll.Sort((a, b) =>
        {
            int res = a.time_start.CompareTo(b.time_start);
            if (res != 0) return res;
            res = a.time_end.CompareTo(b.time_end);
            if (res != 0) return res;
            return string.Compare(a.type,b.type);
        });
        chart.notenum = Notenum;
        chart.notes = new AddChart.NoteData[chart.notenum];
        for (int i = 0; i < Notenum; i++)
        {
            chart.notes[i] = NotesAll[i];
        }
        NotesAll.Clear();
    }
    public void SaveChart()//保存修改到本地
    {
        SaveEdit();
        string jsonData = JsonUtility.ToJson(chart);
        File.WriteAllText(chartpath,jsonData);
    }
    void Update()
    {
        CheckPresses();
        Camera.main.orthographicSize = MinCameraSize + (MaxCameraSize - MinCameraSize) * CameraSlider.value;
        if (Input.GetMouseButtonDown(1))
        {
            NowMousePos = MouseStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            NowMousePos = Input.mousePosition;
            Vector3 DeltaPos = (NowMousePos - MouseStartPos);
            Camera.main.transform.position += new Vector3(-DeltaPos.x*Camera.main.orthographicSize/300, -DeltaPos.y * Camera.main.orthographicSize / 300, 0);
            MouseStartPos = NowMousePos;
        }

        
    }
}
