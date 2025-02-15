using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BrawlMain : MonoBehaviour {
  private readonly Dictionary<string, BaseHuman> otherPlayers = new();
  public GameObject human;

  void Start() {
    BrawlNetManager.AddHandler("Enter", OnEnter);
    BrawlNetManager.AddHandler("List", OnList);
    BrawlNetManager.AddHandler("Move", OnMove);
    BrawlNetManager.AddHandler("Leave", OnLeave);
    BrawlNetManager.AddHandler("Attack", OnAttack);
    BrawlNetManager.AddHandler("Die", OnDie);
    BrawlNetManager.Connect("127.0.0.1", 8888);

    float x = 500 + Random.Range(-10, 10);
    float y = 0;
    float z = 500 + Random.Range(0, 10);
    float v = Random.Range(0, 360);
    CreateHuman(BrawlNetManager.GetDesc(), x, y, z, v, true);
    BrawlNetManager.Send("Enter", new string[] {
      BrawlNetManager.GetDesc(),
      x.ToString(),
      y.ToString(),
      z.ToString(),
      v.ToString(),
    });

    StartCoroutine(GetList());
  }

  private IEnumerator GetList() {
    yield return new WaitForSeconds(0.1f);
    BrawlNetManager.Send("List", new string[0]);
  }

  void Update() {
    BrawlNetManager.Update();
  }

  private void CreateHuman(string desc, float x, float y, float z, float v, bool isCtrl) {
    GameObject player = Instantiate(human);
    player.transform.position = new Vector3(x, y, z);
    player.transform.eulerAngles = new Vector3(0, v, 0);
    BaseHuman baseHuman;
    if (isCtrl) {
      baseHuman = player.AddComponent<ControlHuman>();
    } else {
      baseHuman = player.AddComponent<SyncHuman>();
      otherPlayers.Add(desc, baseHuman);
    }
    baseHuman.desc = desc;
  }

  private void OnEnter(string[] args) {
    CreateHuman(args[0], float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]), false);
  }

  private void OnList(string[] args) {
    for (int i = 0; i + 5 < args.Length; i += 6) {
      if (args[i] == BrawlNetManager.GetDesc()) {
        continue;
      }
      CreateHuman(args[i], float.Parse(args[i + 1]), float.Parse(args[i + 2]), float.Parse(args[i + 3]), float.Parse(args[i + 4]), false);
    }
  }

  private void OnMove(string[] args) {
    if (!otherPlayers.ContainsKey(args[0])) {
      return;
    }
    float x = float.Parse(args[1]);
    float y = float.Parse(args[2]);
    float z = float.Parse(args[3]);
    otherPlayers[args[0]].MoveTo(new Vector3(x, y, z));
  }

  private void OnLeave(string[] args) {
    if (!otherPlayers.ContainsKey(args[0])) {
      return;
    }
    Destroy(otherPlayers[args[0]].gameObject);
    otherPlayers.Remove(args[0]);
  }

  private void OnAttack(string[] args) {
    if (!otherPlayers.ContainsKey(args[0])) {
      return;
    }
    ((SyncHuman)otherPlayers[args[0]]).Attack(float.Parse(args[1]));
  }

  private void OnDie(string[] args) {
    if (args[0] == BrawlNetManager.GetDesc()) {
      Debug.Log("Game Over!");
      return;
    }
    if (!otherPlayers.ContainsKey(args[0])) {
      return;
    }
    otherPlayers[args[0]].gameObject.SetActive(false);
  }
}
