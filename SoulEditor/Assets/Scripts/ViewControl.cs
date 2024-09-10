using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Button Edit;
    public EditController EditController;

    public GameObject ViewCanvas;
    public GameObject ViewObjects;
    public GameObject EditCanvas;
    public GameObject EditObjects;

    public GameObject Box;
    public GameObject Tap;
    public GameObject Drag;
    public GameObject Hold;

    public Toggle Play;

    public float time;

    private List<GameObject> BoxObjects = new List<GameObject>();
    private List<List<GameObject>> NoteObjects = new List<List<GameObject>>();

    public Text MusicLength;
    public Slider SongSlider;
    private float wholeLength;
    private AudioClip music;

    void Start()
    {
        Edit.onClick.AddListener(EditMode);
        SongSlider.onValueChanged.AddListener(SongLength);
        Play.onValueChanged.AddListener(PlayOrPause);
    }
    public void PlayOrPause(bool val)
    {
        if (val)
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().time = time;
                GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            GetComponent<AudioSource>().Pause();
        }
    }
    public void EditMode()
    {
        EditController.gameObject.SetActive(true);
        EditCanvas.SetActive(true);
        EditObjects.SetActive(true);

        ViewCanvas.SetActive(false);
        ViewObjects.SetActive(false);
    }
    public void LoadView()
    {
        Camera.main.orthographicSize = (float)9;
        Vector2 v = Camera.main.ViewportToWorldPoint(new Vector2(0,0));
        Vector2 v1 = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        Debug.Log(v1-v);
        wholeLength = EditController.songLength+(float)0.08333;
        music = EditController.song;
        GetComponent<AudioSource>().clip = music;
        foreach (GameObject obj in BoxObjects)
        {
            DestroyImmediate(obj);
        }
        foreach (List<GameObject> ls in NoteObjects)
        {
            foreach(GameObject obj in ls)
            {
                DestroyImmediate(obj);
            }
            ls.Clear();
        }
        // Çå¿ÕÁÐ±í
        BoxObjects.Clear();
        NoteObjects.Clear();
        BoxObjects = new List<GameObject>();
        NoteObjects = new List<List<GameObject>>();
        AddChart.ChartData chart = EditController.chart;
        for(int i = 0; i < chart.boxnum; i++)
        {
            GameObject Obj = Instantiate(Box);
            Obj.AddComponent<ViewBoxInfo>().speed = chart.boxes[i].speed;
            Obj.transform.parent = ViewObjects.transform;
            Obj.transform.position = new Vector3(v.x+(float)chart.boxes[i].x,v.y+(float)chart.boxes[i].y, 0);
            Obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (float)chart.boxes[i].angle));
            Obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = chart.boxes[i].color;
            BoxObjects.Add(Obj);
            NoteObjects.Add(new List<GameObject>());
        }
        for(int i = 0; i < chart.notenum; i++)
        {
            AddChart.NoteData ntdt = chart.notes[i];
            if (ntdt.type == "Tap")
            {
                GameObject note = Instantiate(Tap);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                note.transform.localPosition = new Vector3(0, 0, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.GetChild(0).localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset + (float)ntdt.time_start*(float)(ntdt.speedoffset+chart.boxes[ntdt.targetbox].speed),-1);
                note.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ntdt.color;
                NoteObjects[ntdt.targetbox].Add(note);
            }
            else if (ntdt.type == "Drag")
            {
                GameObject note = Instantiate(Drag);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                note.transform.localPosition = new Vector3(0, 0, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.GetChild(0).localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset+(float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), -1);
                note.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ntdt.color;
                NoteObjects[ntdt.targetbox].Add(note);
            }
            else if(ntdt.type == "Hold")
            {
                GameObject note = Instantiate(Hold);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.time_end = ntdt.time_end;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                note.transform.localPosition = new Vector3(0, 0, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.GetChild(0).localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset + (float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), -1);
                note.transform.GetChild(2).localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset + (float)ntdt.time_end * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), -1);
                
                note.transform.GetChild(1).localPosition = (note.transform.GetChild(0).localPosition + note.transform.GetChild(2).localPosition) / 2;
                note.transform.GetChild(3).localPosition = new Vector3(note.transform.GetChild(3).localPosition.x, note.transform.GetChild(1).localPosition.y, note.transform.GetChild(3).localPosition.z);
                note.transform.GetChild(4).localPosition = new Vector3(note.transform.GetChild(4).localPosition.x, note.transform.GetChild(1).localPosition.y, note.transform.GetChild(4).localPosition.z);

                note.transform.GetChild(1).localScale = new Vector3((float)1.359375, note.transform.GetChild(2).localPosition.y- note.transform.GetChild(0).localPosition.y, 1);
                note.transform.GetChild(3).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y,1);
                note.transform.GetChild(4).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y,1);
                for (int k = 0; k < 5; k++)
                {
                    note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = ntdt.color;
                    if (k == 1)
                    {
                        var col = ntdt.color;
                        col.a = col.a * (float)0.6;
                        note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = col;
                    }
                }
                NoteObjects[ntdt.targetbox].Add(note);
            }
        }
        
    }
    public void SongLength(float val)
    {
        MusicLength.text = "Time:" + (val * wholeLength - (float)0.08333).ToString();
        time = (val * wholeLength);
        foreach (List<GameObject> ls in NoteObjects)
        {
            foreach (GameObject note in ls)
            {
                string T = note.GetComponent<ViewNoteInfo>().type;
                if (T=="Tap" || T == "Drag")
                {
                    float nowTime = Mathf.Max((float)note.GetComponent<ViewNoteInfo>().time_start - val * wholeLength,0);
                    note.transform.GetChild(0).localPosition = new Vector3(0, nowTime*(float)(note.transform.parent.GetComponent<ViewBoxInfo>().speed+note.GetComponent<ViewNoteInfo>().speedoffset), 0);
                }
                else if (T == "Hold")
                {
                    float nowTime = Mathf.Max((float)note.GetComponent<ViewNoteInfo>().time_start - val * wholeLength, 0);
                    float endTime = Mathf.Max((float)note.GetComponent<ViewNoteInfo>().time_end - val * wholeLength, 0);
                    float speed = (float)(note.transform.parent.GetComponent<ViewBoxInfo>().speed + note.GetComponent<ViewNoteInfo>().speedoffset);
                    note.transform.GetChild(0).localPosition = new Vector3(0, nowTime * speed, -1);
                    note.transform.GetChild(2).localPosition = new Vector3(0, endTime * speed, -1);

                    note.transform.GetChild(1).localPosition = new Vector3(0, (endTime + nowTime) * speed / 2, 0);
                    note.transform.GetChild(3).localPosition = new Vector3((float)0.6679688, (endTime + nowTime) * speed / 2, (float)-0.05);
                    note.transform.GetChild(4).localPosition = new Vector3((float)-0.6679688, (endTime + nowTime) * speed / 2, (float)-0.05);

                    note.transform.GetChild(1).localScale = new Vector3((float)1.359375, (endTime - nowTime) * speed, 1);
                    note.transform.GetChild(3).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y, 1);
                    note.transform.GetChild(4).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y, 1);
                }
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Play.isOn)
        {
            time += Time.deltaTime;
            SongSlider.value = time / wholeLength;
            SongLength(SongSlider.value);

        }
    }
}
