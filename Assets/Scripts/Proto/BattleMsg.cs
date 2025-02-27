using System;

[Serializable]
public class TankInfo {
  public string ID = "";
  public float X = 0;
  public float Y = 0;
  public float Z = 0;
  public float EX = 0;
  public float EY = 0;
  public float EZ = 0;
  public int Camp = 0;
}

public class MsgEnterBattle : MsgBase {
  public MsgEnterBattle() { ProtoName = "MsgEnterBattle"; }

  public TankInfo[] Tanks;
}

public class MsgBattleResult : MsgBase {
  public MsgBattleResult() { ProtoName = "MsgBattleResult"; }

  public int Result = 0;
}

public class MsgLeaveBattle : MsgBase {
  public MsgLeaveBattle() { ProtoName = "MsgLeaveBattle"; }

  public string ID = "";
}
