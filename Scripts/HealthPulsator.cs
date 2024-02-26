using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPulsator : MonoBehaviour
{

    public float freq_timer = 0.03f;
    public Texture[] tex;

    private int cur_tex = 0;
    private float cur_timer = 0f;

    void Update()
    {
        cur_timer += Time.deltaTime;
        if (cur_timer > freq_timer) {
            cur_timer = 0f;
            cur_tex++;
            if (cur_tex > tex.Length - 1) {
                cur_tex = 0;
            }
        }

        GetComponent<RawImage>().texture = tex[cur_tex];
    }
}
