using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class menu : MonoBehaviour
{
    public List<string[]> FileInfo = new List<string[]>();
    public Dropdown History;
    public Button Enter;
    void Start()
    {
        var path = Application.persistentDataPath + "/LoadLevel.txt";
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        path = Application.persistentDataPath + "/NowFiles.txt";
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        var Info = File.ReadAllLines(path);
        if (Info.Length == 0) Enter.interactable = false;
        else
        {
            Enter.interactable = true;

        }
        for (int i = 0; i < Info.Length; i += 3)
        {
            FileInfo.Add(new string[] {Info[i],Info[i+1],Info[i+2]});
            History.options.Add(new Dropdown.OptionData(Info[i]));
        }
        History.onValueChanged.AddListener(choose);
        Enter.onClick.AddListener(enter);
    }

    // Update is called once per frame
    private void choose(int val)
    {
        var path = Application.persistentDataPath + "/LoadLevel.txt";
        string[] s = new string[3] {FileInfo[val][0], FileInfo[val][1],FileInfo[val][2] };
        File.WriteAllLines(path,s);

    }
    private void enter()
    {
        SceneManager.LoadScene("edit");
    }
}
