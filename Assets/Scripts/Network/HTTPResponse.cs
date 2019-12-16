using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTTPResponse
{
    long code;
    string message;
    Dictionary<string, string> headers;

    public long Code { get { return code; } }
    public string Message { get { return message; } }
    public Dictionary<string, string> Headers { get { return headers; } }

    public HTTPResponse(long code, string message, Dictionary<string, string> headers)
    {
        this.code = code;
        this.message = message;
        this.headers = headers;
    }
}
