using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : BasePanel {
  public override void OnInit() {
    path = "ResultPanel";
    layer = PanelManager.Layer.Panel;
  }

  public override void OnShow(params object[] param) {
    panel.transform.Find("Close").GetComponent<Button>().onClick.AddListener(Return);
    if (param.Length < 1 || param[0] is not bool result) {
      return;
    }
    panel.transform.Find("Text").GetComponent<TMP_Text>().text = result ? "胜利！" : "失败！";
  }

  public override void OnClose() {
  }

  private void Return() {
    PanelManager.Open<RoomPanel>();
    Close();
    PanelManager.Close("BattlePanel");
  }
}
