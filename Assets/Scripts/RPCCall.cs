using System;

[Serializable]
public struct RPCCall
{
    public string methodName;
    public object[] parameters;

    public RPCCall(string methodName, object[] parameters) : this()
    {
        this.methodName = methodName;
        this.parameters = parameters;
    }
}
