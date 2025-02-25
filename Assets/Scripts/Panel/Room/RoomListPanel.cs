using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel {
  private TMP_Text score;
  private GameObject list;
  private GameObject roomPrefabs;

  public override void OnInit() {
    path = "RoomListPanel";
    layer = PanelManager.Layer.Panel;
    roomPrefabs = ResourceManager.Load("Room");
  }

  public override void OnShow(params object[] param) {
    panel.transform.Find("InfoPanel/ID").GetComponent<TMP_Text>().text = "账号：" + GameMain.ID;
    score = panel.transform.Find("InfoPanel/Score").GetComponent<TMP_Text>();
    list = panel.transform.Find("ListPanel/Scroll View/Viewport/Content").gameObject;
    panel.transform.Find("ControlPanel/CreateRoom").GetComponent<Button>().onClick.AddListener(CreateRoom);
    panel.transform.Find("ControlPanel/ReflashList").GetComponent<Button>().onClick.AddListener(ReflashList);

    NetManager.AddMsgListener("MsgGetScore", OnMsgGetScore);
    NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
    NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
    NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

    NetManager.Send(new MsgGetScore());
    NetManager.Send(new MsgGetRoomList());
  }

  public override void OnClose() {
    NetManager.RemoveMsgListener("MsgGetScore", OnMsgGetScore);
    NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
    NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
    NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
  }

  private void OnMsgGetScore(MsgBase msgBase) {
    if (msgBase is not MsgGetScore msg) {
      return;
    }
    score.text = "战绩：" + msg.Win + " - " + msg.Lose;
  }

  private void OnMsgGetRoomList(MsgBase msgBase) {
    if (msgBase is not MsgGetRoomList msg) {
      return;
    }
    foreach (Transform child in list.transform) {
      Destroy(child.gameObject);
    }
    if (msg.Rooms == null) {
      return;
    }
    foreach (RoomInfo room in msg.Rooms) {
      CreateRoomInfo(room);
    }
  }

  private void OnMsgCreateRoom(MsgBase msgBase) {
    if (msgBase is not MsgCreateRoom msg) {
      return;
    }
    if (msg.Result == 0) {
      PanelManager.Open<RoomPanel>();
      PanelManager.Tip("创建房间成功!");
      Close();
    } else {
      PanelManager.Tip("创建房间失败!");
    }
  }

  private void OnMsgEnterRoom(MsgBase msgBase) {
    if (msgBase is not MsgEnterRoom msg) {
      return;
    }
    if (msg.Result == 0) {
      PanelManager.Open<RoomPanel>();
      Close();
    } else {
      PanelManager.Tip("进入房间失败!");
    }
  }

  private void CreateRoomInfo(RoomInfo info) {
    if (info == null) {
      return;
    }
    GameObject room = Instantiate(roomPrefabs, list.transform);
    room.transform.Find("ID").GetComponent<TMP_Text>().text = "房间号：" + info.ID;
    room.transform.Find("Count").GetComponent<TMP_Text>().text = "人数：" + info.Count;
    room.transform.Find("Status").GetComponent<TMP_Text>().text = "状态：" + (info.Status == 0 ? "准备中" : "战斗中");
    room.transform.Find("Join").GetComponent<Button>().onClick.AddListener(() => {
      NetManager.Send(new MsgEnterRoom() {
        ID = info.ID
      });
    });
  }

  private void CreateRoom() {
    NetManager.Send(new MsgCreateRoom());
  }

  private void ReflashList() {
    NetManager.Send(new MsgGetRoomList());
  }
}
