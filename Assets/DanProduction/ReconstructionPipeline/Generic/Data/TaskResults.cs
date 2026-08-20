using System.Collections.Generic;
using UnityEngine;

public class TaskResults
{
    public bool ResultSubmitted { get; private set; }
    public bool Success { get; private set; }
    public string Caption { get; private set; }
    public List<string> Warnings = new();
    public List<string> Errors =  new();


    public void SubmitResults(bool success, string caption)
    {
        ResultSubmitted = true;
        Success = success;
        Caption = caption;
    }

    public void AppendIssues(TaskResults incomingResults)
    {
        Warnings.AddRange(incomingResults.Warnings);
        Errors.AddRange(incomingResults.Errors);
    }
}
