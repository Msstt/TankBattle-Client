using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel {
  private Slider health;
  public override void OnInit() {
    path = "BattlePanel";
    layer = PanelManager.Layer.Panel;
  }

  public override void OnShow(params object[] param) {
    health = panel.transform.Find("Health").GetComponent<Slider>();
  }

  public override void OnClose() {
  }

  public void UpdateHealth(float value) {
    health.value = value;
  }
}
