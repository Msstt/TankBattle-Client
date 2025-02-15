using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class BrawlNetManager {
  private static readonly byte[] buffer = new byte[1024];
  private static Socket socket;
  public delegate void MsgHandler(string[] args);
  private static readonly Dictionary<string, MsgHandler> handlers = new();
  private static readonly List<string> msgQueue = new();

  public static void AddHandler(string topic, MsgHandler handler) {
    handlers[topic] = handler;
  }

  public static string GetDesc() {
    if (socket == null || !socket.Connected) {
      return "";
    }
    return socket.LocalEndPoint.ToString();
  }

  public static void Connect(string ip, int port) {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    try {
      socket.Connect(ip, port);
      Debug.Log("Socket connect success.");
    } catch (SocketException ex) {
      Debug.Log("Socket connect failed: " + ex);
    }
    socket.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, null);
  }

  private static void OnReceive(IAsyncResult ar) {
    try {
      int bytesCount = socket.EndReceive(ar);
      msgQueue.Add(System.Text.Encoding.UTF8.GetString(buffer, 0, bytesCount));
      Debug.Log("Receive message: " + msgQueue[^1]);
      socket.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, null);
    } catch (SocketException ex) {
      Debug.Log("Socket receive failed: " + ex);
    }
  }

  public static void Send(string topic, string[] args) {
    if (socket == null || !socket.Connected) {
      return;
    }
    string msg = String.Copy(topic) + "|";
    foreach (object arg in args) {
      msg += arg + ",";
    }
    if (args.Length > 0) {
      msg = msg[..^1];
    }
    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg);
    socket.BeginSend(buffer, 0, buffer.Length, 0, OnSend, null);
  }

  private static void OnSend(IAsyncResult ar) {
    try {
      socket.EndSend(ar);
    } catch (SocketException ex) {
      Debug.Log("Socket send failed: " + ex);
    }
  }

  public static void Update() {
    if (msgQueue.Count == 0) {
      return;
    }
    string[] msg = msgQueue[0].Split('|');
    if (handlers.ContainsKey(msg[0])) {
      MsgHandler handler = handlers[msg[0]];
      if (msg.Length > 1) {
        handler(msg[1].Split(','));
      } else {
        handler(new string[0]);
      }
    }
    msgQueue.RemoveAt(0);
  }
}
