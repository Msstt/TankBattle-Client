using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class PanelManager : MonoBehaviour {
  public enum Layer {
    Panel,
    Tip
  };

  private static readonly Dictionary<Layer, Transform> layers = new();
  private static readonly Dictionary<string, BasePanel> panels = new();
  private static readonly List<BasePanel> tips = new();
  private static GameObject root;

  public static void Init() {
    root = GameObject.Find("Root");
    Transform canvas = root.transform.Find("Canvas");
    layers.Add(Layer.Panel, canvas.Find("Panel"));
    layers.Add(Layer.Tip, canvas.Find("Tip"));
  }

  public static BasePanel Open<T>(params object[] param) where T : BasePanel {
    string name = typeof(T).ToString();
    if (panels.ContainsKey(name)) {
      return null;
    }
    T panel = root.AddComponent<T>();
    panel.OnInit();
    panel.Init();
    panel.panel.transform.SetParent(layers[panel.layer], false);
    if (panel.layer == Layer.Panel) {
      panels.Add(name, panel);
    } else {
      tips.Add(panel);
    }
    panel.OnShow(param);
    return panel;
  }

  public static void Close(string name) {
    BasePanel panel;
    if (panels.ContainsKey(name)) {
      panel = panels[name];
    } else if (tips.Count > 0) {
      panel = tips[0];
    } else {
      return;
    }
    panel.OnClose();
    Destroy(panel.panel);
    Destroy(panel);
    if (panel.layer == Layer.Panel) {
      panels.Remove(name);
    } else {
      tips.RemoveAt(0);
    }
  }

  public static void Tip(string text) {
    Open<TipPanel>(text);
  }
}
