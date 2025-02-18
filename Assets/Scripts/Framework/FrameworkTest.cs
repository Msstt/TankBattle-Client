using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameworkTest : MonoBehaviour {
  void Start() {
    NetManager.AddEventListener(NetEvent.ConnectSuccess, OnConnectSuccess);
    NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
    NetManager.AddEventListener(NetEvent.Close, OnClose);
    NetManager.AddMsgListener("MsgMove", OnMsgMove);
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
    MsgMove msg = new() {
      X = 20,
      Y = -80,
      Z = 0
    };
    NetManager.Send(msg);
  }

  private void OnMsgMove(MsgBase msg) {
    MsgMove msgMove = (MsgMove)msg;
    Debug.Log("Call OnMsgMove " + msgMove.X + "," + msgMove.Y + "," + msgMove.Z);
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
