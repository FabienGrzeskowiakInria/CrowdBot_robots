using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReport : MonoBehaviour
{
    private List<string> reports;
    private bool updated = false;

    // Start is called before the first frame update
    void Awake()
    {
        reports = new List<string>();
    }

    public void Append_report(string st)
    {
        reports.Add(st);
        updated = true;
    }

    public string Get_report()
    {
        string str_report = string.Join("|", reports);
        
        reports.Clear();

        return str_report;
    }

}