using System;

public class MsgGetScore : MsgBase {
  public MsgGetScore() { ProtoName = "MsgGetScore"; }

  public int Win = 0;
  public int Lose = 0;
}

[Serializable]
public class RoomInfo {
  public int ID = 0;
  public int Count = 0;
  public int Status = 0;
}

public class MsgGetRoomList : MsgBase {
  public MsgGetRoomList() { ProtoName = "MsgGetRoomList"; }

  public RoomInfo[] Rooms;
}

public class MsgCreateRoom : MsgBase {
  public MsgCreateRoom() { ProtoName = "MsgCreateRoom"; }

  public int ID = 0;
  public int Result = 0;
}

public class MsgEnterRoom : MsgBase {
  public MsgEnterRoom() { ProtoName = "MsgEnterRoom"; }

  public int ID = 0;
  public int Result = 0;
}

public class MsgLeaveRoom : MsgBase {
  public MsgLeaveRoom() { ProtoName = "MsgLeaveRoom"; }

  public int Result = 0;
}

[Serializable]
public class PlayerInfo {
  public string ID = "";
  public int Win = 0;
  public int Lose = 0;
  public int Camp = 0;
  public int isOwner = 0;
}

public class MsgGetRoomInfo : MsgBase {
  public MsgGetRoomInfo() { ProtoName = "MsgGetRoomInfo"; }

  public int RoomID;
  public PlayerInfo[] Players;
}

public class MsgStartGame : MsgBase {
  public MsgStartGame() { ProtoName = "MsgStartGame"; }

  public int Result = 0;
}
