using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameworkTest : MonoBehaviour {
  void Start() {
    NetManager.AddEventListener(NetEvent.ConnectSuccess, OnConnectSuccess);
    NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
    NetManager.AddEventListener(NetEvent.Close, OnClose);
    NetManager.AddMsgListener("MsgTest", OnMsgTest);
  }

  void Update() {
    NetManager.Update();
  }

  public void Connect() {
    NetManager.Connect("127.0.0.1", 8888);
  }

  public void DisConnect() {
    NetManager.Close();
  }

  public void Move() {
    MsgTest msg = new() {
      X = 20,
      Y = -80,
      Z = 0
    };
    NetManager.Send(msg);
  }

  private void OnMsgTest(MsgBase msg) {
    MsgTest MsgTest = (MsgTest)msg;
    Debug.Log("Call OnMsgTest " + MsgTest.X + "," + MsgTest.Y + "," + MsgTest.Z);
  }

  private void OnClose(string error) {
    Debug.Log("Call OnClose");
  }

  private void OnConnectFail(string error) {
    Debug.Log("Call OnConnectFail");
  }

  private void OnConnectSuccess(string error) {
    Debug.Log("Call OnConnectSuccess");
  }
}
