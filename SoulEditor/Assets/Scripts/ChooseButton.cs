using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseButton : MonoBehaviour
{
    void Start()
    {
        Toggle T = this.GetComponent<Toggle>();
        T.onValueChanged.AddListener(ChooseChosen);
    }
    private void ChooseChosen(bool val)
    {
        Image image = this.GetComponent<Image>();
        if (!val)
        {
            image.color = new Vector4(1,1,1,1);
        }
        else image.color = new Vector4((float)0.5, (float)0.5, (float)0.5, 1);
    }
}
