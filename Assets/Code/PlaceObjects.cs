using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{

    public GameObject earth;
    public GameObject satelite;
    public static readonly float radius = 637.1f;

    // Start is called before the first frame update
    public void Start()
    {

        RunPythonScript();

        List<List<float>> list = new List<List<float>>();

        ReadData(ref list);


        for (int i = 0; i < list.Count; i++)
        {
            float lat = list[i][0];
            float lng = list[i][1];
            float alt = list[i][2] / 10000;
            int   id  = (int)list[i][3];

            float cosLat = Mathf.Cos(lat * Mathf.PI / 180);
            float sinLat = Mathf.Sin(lat * Mathf.PI / 180);

            float cosLong = Mathf.Cos(lng * Mathf.PI / 180);
            float sinLong = Mathf.Sin(lng * Mathf.PI / 180);

            float f_inv = 298.257224f;

            float f = 1.0f / f_inv;

            float e2 = 1 - (1 - f) * (1 - f);

            float c = 1 / Mathf.Sqrt(cosLat * cosLat + (1 - f) * (1 - f) * sinLat * sinLat);
            float s = (1 - f) * (1 - f) * c;

            float x = (radius * c + alt) * cosLat * cosLong;
            float y = (radius * s + alt) * sinLat;
            float z = (radius * c + alt) * cosLat * sinLong;

            GameObject instance = Instantiate(satelite, new Vector3(x, y, z), Quaternion.identity);
            instance.GetComponent<Info>().NORAD_ID = id;
            instance.GetComponent<Info>().index = i;
            instance.GetComponent<Info>().latlongalt = new Vector3(lat, lng, alt);
       
        }
    }

    void RunPythonScript()
    {
        string strCmdText;
        strCmdText = "/C python getdata.py && exit";
        System.Diagnostics.Process proc = System.Diagnostics.Process.Start("CMD.exe", strCmdText);
        // wait for the script to finish before continuing
        while (!proc.HasExited) { }
    }

    void ReadData(ref List<List<float>> list)
    {
        string[] lines = System.IO.File.ReadAllLines("latlongalt.dat");

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] split = line.Split(' ');
            if (split.Length < 3)
                break;
            list.Add(new List<float>());
            list[i].Add(float.Parse(split[0]));
            list[i].Add(float.Parse(split[1]));
            list[i].Add(float.Parse(split[2]));
            list[i].Add(float.Parse(split[3]));
        }
    }


    // Update is called once per frame
    public void Update()
    {
        
    }
}
