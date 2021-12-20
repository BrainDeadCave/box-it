using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The fields of a backend response
/// </summary>
[Serializable]
public class BackendResponse
{
	public string task_id;
	/// <summary>
	/// Maybe SUCCESS or FAILURE
	/// </summary>
	public string status;
	public Result result;
	/// <summary>
	/// Not filled unless an error occurred
	/// </summary>
	public string traceback;
	public object[] children;
}
/// <summary>
/// The fields of a backend response's result section
/// </summary>
[Serializable]
public class Result
{
	public string exc_type;
	public string[] exc_message;
	public string exc_module;
	public string task;
	public string status;
	public string userid;
	public string valuelevel;
	public string coin;
	public string speedup;
	/// <summary>
	/// Not filled unless an error occurred
	/// </summary>
	public string error;
}
