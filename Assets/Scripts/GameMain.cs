using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
  public static string ID;

  void Start() {
    NetManager.AddEventListener(NetEvent.Close, OnClose);
    NetManager.AddMsgListener("MsgKick", OnMsgKick);

    PanelManager.Init();
    BattleManager.Init();
    PanelManager.Open<LoginPanel>();
  }

  void Update() {
    NetManager.Update();
  }

  private void OnClose(string error) {
    PanelManager.Open<LoginPanel>();
  }

  private void OnMsgKick(MsgBase msg) {
    PanelManager.Tip("您的账号在其他设备登录!");
  }
}
