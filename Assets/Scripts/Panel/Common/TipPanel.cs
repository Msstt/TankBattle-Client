using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel {
  private Image background;
  private TMP_Text text;
  private static readonly float fadeTime = 2f;

  public override void OnInit() {
    path = "TipPanel";
    layer = PanelManager.Layer.Tip;
  }

  public override void OnShow(params object[] param) {
    if (param.Length < 1) {
      return;
    }
    background = panel.GetComponent<Image>();
    text = panel.transform.Find("Text").GetComponent<TMP_Text>();
    text.text = param[0] as string;

    StartCoroutine(Fade());
  }

  private IEnumerator Fade() {
    yield return new WaitForSeconds(fadeTime - 1);
    float a = 1;
    while (a >= 0) {
      a -= 0.01f;
      Color color = background.color;
      color.a = a;
      background.color = color;
      color = text.color;
      color.a = a;
      text.color = color;
      yield return new WaitForSeconds(0.01f);
    }
    Close();
  }

  public override void OnClose() {
  }
}
