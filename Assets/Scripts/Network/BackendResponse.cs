using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BackendResponse
{
	public string task_id;
	public string status;
	public Result result;
	public string traceback;
	public object[] children;
}

[Serializable]
public class Result
{
	public string exc_type;
	public string[] exc_message;
	public string exc_module;
	public string task;
	public string status;
	public string userid;
	public string error;
}
