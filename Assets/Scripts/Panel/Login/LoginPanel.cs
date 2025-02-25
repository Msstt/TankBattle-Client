using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel {
  private TMP_InputField ID;
  private TMP_InputField password;

  public override void OnInit() {
    path = "LoginPanel";
    layer = PanelManager.Layer.Panel;
  }

  public override void OnShow(params object[] param) {
    ID = panel.transform.Find("IDInput").GetComponent<TMP_InputField>();
    password = panel.transform.Find("PasswordInput").GetComponent<TMP_InputField>();
    panel.transform.Find("Login").GetComponent<Button>().onClick.AddListener(OnLogin);
    panel.transform.Find("Register").GetComponent<Button>().onClick.AddListener(OnRegister);

    NetManager.AddEventListener(NetEvent.ConnectSuccess, OnConnectSuccess);
    NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);

    NetManager.AddMsgListener("MsgLogin", OnMsgLogin);

    NetManager.Connect("127.0.0.1", 8888);
  }

  public override void OnClose() {
    NetManager.RemoveEventListener(NetEvent.ConnectSuccess, OnConnectSuccess);
    NetManager.RemoveEventListener(NetEvent.ConnectFail, OnConnectFail);

    NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
  }

  private void OnLogin() {
    if (ID.text == "" || password.text == "") {
      PanelManager.Tip("账号或密码不能为空!");
      return;
    }
    NetManager.Send(new MsgLogin() {
      Id = ID.text,
      Password = password.text
    });
  }

  private void OnRegister() {
    PanelManager.Open<RegisterPanel>();
  }

  private void OnConnectSuccess(string error) {
  }

  private void OnConnectFail(string error) {
    PanelManager.Tip("连接服务器失败!");
  }

  private void OnMsgLogin(MsgBase msgBase) {
    if (msgBase is not MsgLogin msg) {
      return;
    }
    if (msg.Result == 0) {
      PanelManager.Open<RoomListPanel>();
      GameMain.ID = ID.text;
      Close();
    } else {
      PanelManager.Tip("登录失败!");
    }
  }
}
