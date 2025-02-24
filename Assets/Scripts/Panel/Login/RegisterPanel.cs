using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel {
  private TMP_InputField ID;
  private TMP_InputField password;
  private TMP_InputField repeatPassword;

  public override void OnInit() {
    path = "RegisterPanel";
    layer = PanelManager.Layer.Panel;
  }

  public override void OnShow(params object[] param) {
    ID = panel.transform.Find("IDInput").GetComponent<TMP_InputField>();
    password = panel.transform.Find("PasswordInput").GetComponent<TMP_InputField>();
    repeatPassword = panel.transform.Find("RepeatPasswordInput").GetComponent<TMP_InputField>();
    SetClose();
    panel.transform.Find("Register").GetComponent<Button>().onClick.AddListener(OnRegister);

    NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
  }

  public override void OnClose() {
  }

  private void OnRegister() {
    if (ID.text == "" || password.text == "") {
      PanelManager.Tip("账号或密码不能为空!");
      return;
    }
    if (password.text != repeatPassword.text) {
      PanelManager.Tip("重复密码不一致!");
      return;
    }
    NetManager.Send(new MsgRegister() {
      Id = ID.text,
      Password = password.text
    });
  }

  private void OnMsgRegister(MsgBase msgBase) {
    if (msgBase is not MsgRegister msg) {
      return;
    }
    if (msg.Result == 0) {
      Close();
      PanelManager.Tip("注册成功!");
    } else {
      PanelManager.Tip("注册失败!");
    }
  }
}
