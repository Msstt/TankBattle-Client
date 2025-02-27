using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BattleManager : MonoBehaviour {
  private static readonly Dictionary<string, BaseTank> tanks = new();
  private static BaseTank selfTank = null;
  public static readonly List<GameObject> effects = new();
  public static BattlePanel panel;

  public static void Init() {
    NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
    NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
    NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);

    NetManager.AddMsgListener("MsgSyncTank", OnMsgSyncTank);
    NetManager.AddMsgListener("MsgFire", OnMsgFire);
    NetManager.AddMsgListener("MsgHit", OnMsgHit);
  }

  private static void AddTank(string id, BaseTank tank) {
    if (tanks.ContainsKey(id)) {
      return;
    }
    tanks[id] = tank;
  }

  private static void RemoveTank(string id) {
    if (!tanks.ContainsKey(id)) {
      return;
    }
    tanks.Remove(id);
  }

  private static BaseTank GetTank(string id) {
    if (!tanks.ContainsKey(id)) {
      return null;
    }
    return tanks[id];
  }

  private static void Reset() {
    foreach (BaseTank tank in tanks.Values) {
      Destroy(tank.gameObject);
    }
    tanks.Clear();
    foreach (GameObject effect in effects) {
      Destroy(effect);
    }
    tanks.Clear();
    selfTank = null;
  }

  private static void OnMsgEnterBattle(MsgBase msgBase) {
    if (msgBase is not MsgEnterBattle msg) {
      return;
    }
    Reset();
    PanelManager.Close("RoomPanel");
    PanelManager.Close("ResultPanel");
    panel = PanelManager.Open<BattlePanel>() as BattlePanel;
    foreach (TankInfo tank in msg.Tanks) {
      GenerateTank(tank);
    }
  }

  private static void GenerateTank(TankInfo tankInfo) {
    if (tankInfo == null) {
      return;
    }
    GameObject gameObject = new();
    BaseTank tank;
    if (tankInfo.ID == GameMain.ID) {
      tank = gameObject.AddComponent<ControlTank>();
      selfTank = tank;
    } else {
      tank = gameObject.AddComponent<SyncTank>();
    }
    tank.id = tankInfo.ID;
    tank.camp = tankInfo.Camp;
    tank.transform.position = new Vector3(tankInfo.X, tankInfo.Y, tankInfo.Z);
    tank.transform.eulerAngles = new Vector3(tankInfo.EX, tankInfo.EY, tankInfo.EZ);
    tank.Init();
    AddTank(tankInfo.ID, tank);
  }

  private static void OnMsgBattleResult(MsgBase msgBase) {
    if (msgBase is not MsgBattleResult msg) {
      return;
    }
    if (selfTank == null || msg.Result != selfTank.camp) {
      PanelManager.Open<ResultPanel>(false);
    } else {
      PanelManager.Open<ResultPanel>(true);
    }
  }

  private static void OnMsgLeaveBattle(MsgBase msgBase) {
    if (msgBase is not MsgLeaveBattle msg) {
      return;
    }
    BaseTank tank = GetTank(msg.ID);
    if (tank == null) {
      return;
    }
    RemoveTank(msg.ID);
    Destroy(tank.gameObject);
  }

  private static void OnMsgSyncTank(MsgBase msgBase) {
    if (msgBase is not MsgSyncTank msg) {
      return;
    }
    SyncTank tank = GetTank(msg.ID) as SyncTank;
    if (tank == null) {
      return;
    }
    tank.SyncPosition(msg);
  }

  private static void OnMsgFire(MsgBase msgBase) {
    if (msgBase is not MsgFire msg) {
      return;
    }
    SyncTank tank = GetTank(msg.ID) as SyncTank;
    if (tank == null) {
      return;
    }
    tank.SyncFire(msg);
  }

  private static void OnMsgHit(MsgBase msgBase) {
    if (msgBase is not MsgHit msg) {
      return;
    }
    BaseTank tank = GetTank(msg.ID);
    if (tank == null) {
      return;
    }
    tank.Attack(msg.Health);

    if (tank.id == GameMain.ID) {
      panel.UpdateHealth(tank.Health / 100);
    }
  }
}
