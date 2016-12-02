using System;
using System.Collections.Generic;
using UnityEngine;

public class NetEventMessage
{
    private static Dictionary<string, string> codesMeaning = new Dictionary<string, string>();
    public static Dictionary<string, string> CodesMeaning
    {
        get
        {
            if (codesMeaning == null)
            {
                codesMeaning = new Dictionary<string, string>();
                codesMeaning.Add("0", "Room doesn't exist");
                codesMeaning.Add("1", "Signaling server's websocket was closed");
                codesMeaning.Add("2", "The host has disconnected");
                codesMeaning.Add("3", "Room already exists");
                codesMeaning.Add("4", "The room was blocked");
                codesMeaning.Add("5", "ServerConnection != 1");
                codesMeaning.Add("6", "Asked to connect to a different server");
                codesMeaning.Add("7", "Incoming");
                codesMeaning.Add("8", "Outgoing");
            }
            return codesMeaning;
        }
    }

    public const string ROOM_DOESNT_EXIST = "0";
    public const string SIGNALING_SERVER_CLOSED = "1";
    public const string HOST_DISCONNECTED = "2";
    public const string ROOM_ALREADY_EXISTS = "3";
    public const string ROOM_BLOCKED = "4";
    public const string SERVER_CONNECTION_NOT_1 = "5";
    public const string ASKED_TO_CONNECT_TO_DIFFERENT_SERVER = "6";
    public const string INCOMING = "7";
    public const string OUTGOING = "8";




}
