using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;

public class Notepad : MonoBehaviour {
  public TMP_InputField id;
  public TMP_InputField password;
  public TMP_InputField text;

  void Start() {
    NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    NetManager.AddMsgListener("MsgKick", OnMsgKick);
    NetManager.AddMsgListener("MsgGetText", OnMsgGetText);
    NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
  }

  void Update() {
    NetManager.Update();
  }

  public void Connect() {
    NetManager.Connect("127.0.0.1", 8888);
  }

  public void Disconnect() {
    NetManager.Close();
  }

  public void Register() {
    MsgRegister msg = new() {
      Id = id.text,
      Password = password.text
    };
    NetManager.Send(msg);
  }

  public void Login() {
    MsgLogin msg = new() {
      Id = id.text,
      Password = password.text
    };
    NetManager.Send(msg);
  }

  public void Save() {
    MsgSaveText msg = new() {
      Text = text.text
    };
    NetManager.Send(msg);
  }

  private void OnMsgRegister(MsgBase msgBase) {
    MsgRegister msg = (MsgRegister)msgBase;
    if (msg.Result == 0) {
      Debug.Log("注册成功");
    } else {
      Debug.Log("注册失败");
    }
  }

  private void OnMsgLogin(MsgBase msgBase) {
    MsgLogin msg = (MsgLogin)msgBase;
    if (msg.Result == 0) {
      Debug.Log("登录成功");
      NetManager.Send(new MsgGetText());
    } else {
      Debug.Log("登录失败");
    }
  }

  private void OnMsgKick(MsgBase msgBase) {
    MsgKick msg = (MsgKick)msgBase;
    Debug.Log("强制下线");
  }

  private void OnMsgGetText(MsgBase msgBase) {
    MsgGetText msg = (MsgGetText)msgBase;
    text.text = msg.Text;
  }

  private void OnMsgSaveText(MsgBase msgBase) {
    MsgSaveText msg = (MsgSaveText)msgBase;
    if (msg.Result == 0) {
      Debug.Log("保存成功");
    } else {
      Debug.Log("保存失败");
    }
  }
}
