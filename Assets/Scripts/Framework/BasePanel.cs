using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour {
  protected string path;
  public GameObject panel;
  public PanelManager.Layer layer;

  public void Init() {
    GameObject prefabs = ResourceManager.Load(path);
    panel = Instantiate(prefabs);
  }

  public void Close() {
    string name = GetType().ToString();
    PanelManager.Close(name);
  }

  protected void SetClose() {
    panel.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() => Close());
  }

  public abstract void OnInit();
  public abstract void OnShow(params object[] param);
  public abstract void OnClose();
}
