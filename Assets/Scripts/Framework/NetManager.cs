using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public enum NetEvent {
  ConnectSuccess,
  ConnectFail,
  Close
}

public class NetManager {
  private const int MAX_MESSAGE_FIRE = 10;
  private const int MAX_EVENT_FIRE = 10;
  private static readonly List<string> systemMsg = new() { "MsgPong", "MsgSyncTank" };

  private static Socket socket;
  private static bool isConnecting;
  private static bool isClosing;
  private static ByteArray readBuffer;
  private static Queue<ByteArray> writeQueue;

  public delegate void EventListener(string error);
  private static readonly Dictionary<NetEvent, EventListener> eventListeners = new();

  public delegate void MsgListener(MsgBase msg);
  private static readonly Dictionary<string, MsgListener> msgListeners = new();
  private static readonly List<MsgBase> msgList = new();
  private static int msgCount = 0;
  private static readonly List<(NetEvent, string)> eventList = new();
  private static int eventCount = 0;

  public static bool isUsePing = true;
  public static int pingInterval = 30;
  private static float lastPingTime;
  private static float lastPongTime;

  private static void InitState() {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    readBuffer = new();
    writeQueue = new();
    isConnecting = false;
    isClosing = false;

    lastPingTime = Time.time;
    lastPongTime = Time.time;
    if (!msgListeners.ContainsKey("MsgPong")) {
      AddMsgListener("MsgPong", OnMsgPong);
    }
  }

  public static void Update() {
    MsgUpdate();
    EventUpdate();
    PingUpdate();
  }

  #region 建立连接
  public static void Connect(string ip, int port) {
    if (socket != null && socket.Connected) {
      Debug.Log("NetManager connect fail: socket already connected!");
      return;
    }
    if (isConnecting) {
      Debug.Log("NetManager connect fail: socket is connecting!");
      return;
    }
    InitState();
    socket.NoDelay = true;
    isConnecting = true;
    socket.BeginConnect(ip, port, OnConnect, null);
  }

  private static void OnConnect(IAsyncResult ar) {
    try {
      socket.EndConnect(ar);
      Debug.Log("NetManager connect success.");
      AddEvent(NetEvent.ConnectSuccess, "");
      socket.BeginReceive(readBuffer.buffer, readBuffer.writeIndex, readBuffer.Remain, 0, OnReceive, null);
    } catch (SocketException ex) {
      Debug.Log("NetManager connect fail: " + ex);
      AddEvent(NetEvent.ConnectFail, ex.ToString());
    } finally {
      isConnecting = false;
    }
  }
  #endregion

  #region 关闭连接
  public static void Close() {
    if (socket == null || !socket.Connected) {
      return;
    }
    if (isConnecting || isClosing) {
      return;
    }
    if (writeQueue.Count > 0) {
      isClosing = true;
    } else {
      socket.Close();
      AddEvent(NetEvent.Close, "");
    }
  }
  #endregion

  #region 发送数据
  public static void Send(MsgBase msg) {
    if (socket == null || !socket.Connected) {
      return;
    }
    if (isClosing || isConnecting) {
      return;
    }
    byte[] nameBytes = MsgBase.EncodeName(msg);
    byte[] bodyBytes = MsgBase.Encode(msg);
    int length = nameBytes.Length + bodyBytes.Length;
    byte[] sendBytes = new byte[2 + length];
    sendBytes[0] = (byte)(length % 256);
    sendBytes[1] = (byte)(length / 256);
    Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
    Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
    ByteArray byteArray = new(sendBytes);
    int count;
    lock (writeQueue) {
      writeQueue.Enqueue(byteArray);
      count = writeQueue.Count;
    }
    if (count == 1) {
      socket.BeginSend(byteArray.buffer, byteArray.readIndex, byteArray.Length, 0, OnSend, null);
    }
  }

  private static void OnSend(IAsyncResult ar) {
    if (socket == null || !socket.Connected) {
      return;
    }
    if (isConnecting) {
      return;
    }
    try {
      int count = socket.EndSend(ar);
      ByteArray byteArray;
      lock (writeQueue) {
        byteArray = writeQueue.First();
      }
      byteArray.readIndex += count;
      if (byteArray.Length == 0) {
        lock (writeQueue) {
          writeQueue.Dequeue();
          byteArray = writeQueue.FirstOrDefault();
        }
      }
      if (byteArray != null) {
        socket.BeginSend(byteArray.buffer, byteArray.readIndex, byteArray.Length, 0, OnSend, null);
      } else if (isClosing) {
        Close();
      }
    } catch (SocketException ex) {
      Debug.Log("Socket send failed: " + ex + "!");
    }
  }
  #endregion

  #region 接收数据
  private static void OnReceive(IAsyncResult ar) {
    try {
      int count = socket.EndReceive(ar);
      if (count == 0) {
        Close();
        return;
      }
      readBuffer.writeIndex += count;
      OnReceiveData();
      if (readBuffer.Remain < 8) {
        readBuffer.Resize(readBuffer.Length * 2);
      }
      socket.BeginReceive(readBuffer.buffer, readBuffer.writeIndex, readBuffer.Remain, 0, OnReceive, null);
    } catch (SocketException ex) {
      Debug.Log("Socket receive failed: " + ex + "!");
    }
  }

  private static void OnReceiveData() {
    if (readBuffer.Length < 2) {
      return;
    }
    Int16 length = readBuffer.ReadInt16();
    if (readBuffer.Length < 2 + length) {
      return;
    }
    readBuffer.readIndex += 2;
    string msgName = MsgBase.DecodeName(readBuffer.buffer, readBuffer.readIndex, out int readCount);
    if (msgName == "" || readCount > length) {
      Debug.Log("Msg Decode name failed!");
      return;
    }
    if (!systemMsg.Contains(msgName)) {
      Debug.Log("Receive msg: " + msgName + ".");
    }
    MsgBase msg = MsgBase.Decode(msgName, readBuffer.buffer, readBuffer.readIndex + readCount, length - readCount);
    readBuffer.readIndex += length;
    readBuffer.CheckAndMove();
    lock (msgList) {
      msgList.Add(msg);
    }
    msgCount++;
    OnReceiveData();
  }
  #endregion

  #region 事件分发
  public static void AddEventListener(NetEvent netEvent, EventListener eventListener) {
    if (eventListeners.ContainsKey(netEvent)) {
      eventListeners[netEvent] += eventListener;
    } else {
      eventListeners.Add(netEvent, eventListener);
    }
  }

  public static void RemoveEventListener(NetEvent netEvent, EventListener eventListener) {
    if (!eventListeners.ContainsKey(netEvent)) {
      return;
    }
    eventListeners[netEvent] -= eventListener;
    if (eventListeners[netEvent] == null) {
      eventListeners.Remove(netEvent);
    }
  }

  private static void FireEvent(NetEvent netEvent, string error) {
    if (!eventListeners.ContainsKey(netEvent)) {
      return;
    }
    eventListeners[netEvent](error);
  }

  private static void AddEvent(NetEvent netEvent, string error) {
    eventList.Add((netEvent, error));
    eventCount++;
  }

  private static void EventUpdate() {
    if (eventCount == 0) {
      return;
    }
    for (int i = 0; i < MAX_EVENT_FIRE; i++) {
      NetEvent? netEvent = null;
      string error = "";
      lock (eventList) {
        if (eventList.Count > 0) {
          (netEvent, error) = eventList[0];
          eventList.RemoveAt(0);
          eventCount--;
        }
      }
      if (netEvent == null) {
        break;
      }
      FireEvent(netEvent.Value, error);
    }
  }
  #endregion

  #region 消息分发
  public static void AddMsgListener(string msgName, MsgListener msgListener) {
    if (msgListeners.ContainsKey(msgName)) {
      msgListeners[msgName] += msgListener;
    } else {
      msgListeners.Add(msgName, msgListener);
    }
  }

  public static void RemoveMsgListener(string msgName, MsgListener msgListener) {
    if (!msgListeners.ContainsKey(msgName)) {
      return;
    }
    msgListeners[msgName] -= msgListener;
    if (msgListeners[msgName] == null) {
      msgListeners.Remove(msgName);
    }
  }

  private static void FireMsg(MsgBase msg) {
    if (!msgListeners.ContainsKey(msg.ProtoName)) {
      return;
    }
    msgListeners[msg.ProtoName](msg);
  }

  private static void MsgUpdate() {
    if (msgCount == 0) {
      return;
    }
    for (int i = 0; i < MAX_MESSAGE_FIRE; i++) {
      MsgBase msg = null;
      lock (msgList) {
        if (msgList.Count > 0) {
          msg = msgList[0];
          msgList.RemoveAt(0);
          msgCount--;
        }
      }
      if (msg == null) {
        break;
      }
      FireMsg(msg);
    }
  }
  #endregion

  #region 心跳机制
  private static void PingUpdate() {
    if (!isUsePing) {
      return;
    }
    if (Time.time - lastPingTime > pingInterval) {
      Send(new MsgPing());
      lastPingTime = Time.time;
    }
    if (Time.time - lastPongTime > pingInterval * 4) {
      Close();
    }
  }

  private static void OnMsgPong(MsgBase msg) {
    lastPongTime = Time.time;
  }
  #endregion
}
