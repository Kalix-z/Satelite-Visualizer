using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Info : MonoBehaviour
{
    // set by PlaceObjects.cs during initalization
    public int NORAD_ID;
    public int index;
    public Vector3 latlongalt;
    public string date = "...";

    public string GetName()
    {
        string name = "";

        string[] flines = File.ReadAllLines("data.dat");

        name = (flines[index * 3]);

        return name;
    }

    public void SetDate()
    {
        // temporary filler

        WebClient client = new WebClient();

        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

        Stream data = client.OpenRead("https://www.n2yo.com/satellite/?s=" + NORAD_ID.ToString());

        StreamReader reader = new StreamReader(data);

        string s = reader.ReadToEnd();

        

        int startIndex = s.IndexOf("href=\"/browse/?y=");
        int endIndex = startIndex;

        while (s[endIndex] != '<')
        {
            endIndex++;
        }

        string line = "";

        for (int i = startIndex; i < endIndex; i++)
        {
            line += s[i];
        }

        // now, the line ends with the date, go back through the line until we reach the closing a href '>' char
        int k = line.Length-1;
        date = "";
        while (line[k] != '>')
        {
            date += line[k];
            k--;
        }

        ReverseString(ref date);

    }

    void ReverseString(ref string s)
    {
        char[] ar = s.ToCharArray();
        Array.Reverse(ar);
        s = new string (ar);
    }

}
