using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class RoomPanel : BasePanel {
  private TMP_Text ID;
  private TMP_Text owner;
  private GameObject list;
  private GameObject playerPrefabs;

  public override void OnInit() {
    path = "RoomPanel";
    layer = PanelManager.Layer.Panel;
    playerPrefabs = ResourceManager.Load("Player");
  }

  public override void OnShow(params object[] param) {
    ID = panel.transform.Find("InfoPanel/ID").GetComponent<TMP_Text>();
    owner = panel.transform.Find("InfoPanel/Owner").GetComponent<TMP_Text>();
    list = panel.transform.Find("ListPanel/Scroll View/Viewport/Content").gameObject;
    panel.transform.Find("ControlPanel/StartGame").GetComponent<Button>().onClick.AddListener(StartGame);
    panel.transform.Find("ControlPanel/Leave").GetComponent<Button>().onClick.AddListener(Leave);

    NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
    NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
    NetManager.AddMsgListener("MsgStartGame", OnMsgStartGame);

    NetManager.Send(new MsgGetRoomInfo());
  }

  public override void OnClose() {
    NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
    NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
    NetManager.RemoveMsgListener("MsgStartGame", OnMsgStartGame);
  }

  private void OnMsgGetRoomInfo(MsgBase msgBase) {
    if (msgBase is not MsgGetRoomInfo msg) {
      return;
    }
    ID.text = "房间号：" + msg.RoomID;
    foreach (Transform child in list.transform) {
      Destroy(child.gameObject);
    }
    if (msg.Players == null) {
      return;
    }
    foreach (PlayerInfo player in msg.Players) {
      CreatePlayerInfo(player);
      if (player.isOwner == 1) {
        owner.text = "房主：" + player.ID;
      }
    }
  }

  private void OnMsgLeaveRoom(MsgBase msgBase) {
    if (msgBase is not MsgLeaveRoom msg) {
      return;
    }
    if (msg.Result == 0) {
      PanelManager.Open<RoomListPanel>();
      Close();
    } else {
      PanelManager.Tip("离开房间失败!");
    }
  }

  private void OnMsgStartGame(MsgBase msgBase) {
    if (msgBase is not MsgStartGame msg) {
      return;
    }
    if (msg.Result == 0) {
      Close();
      PanelManager.Tip("开始战斗!");
    } else {
      PanelManager.Tip("开始战斗失败!");
    }
  }

  private void CreatePlayerInfo(PlayerInfo info) {
    if (info == null) {
      return;
    }
    GameObject room = Instantiate(playerPrefabs, list.transform);
    room.transform.Find("ID").GetComponent<TMP_Text>().text = "账号：" + info.ID;
    room.transform.Find("Camp").GetComponent<TMP_Text>().text = "阵营：" + info.Camp;
    room.transform.Find("Score").GetComponent<TMP_Text>().text = "战绩：" + info.Win + " - " + info.Lose;
  }

  private void StartGame() {
    NetManager.Send(new MsgStartGame());
  }

  private void Leave() {
    NetManager.Send(new MsgLeaveRoom());
  }
}
