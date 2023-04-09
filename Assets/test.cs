using System.Collections;
using System.Collections.Generic;
using System.IO;
using BinaryPack;
using Maniac.Utils;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<int> points = new List<int>();
        points.Add(0);
        points.Add(0);
        points.Add(4);
        points.Add(6);
        points.Add(8);
        points.Add(6);

        
        
        var bytes = Helper.Serialize(points);
        var data = Helper.Deserialize<List<int>>(bytes);
        
        Debug.Log(data.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
