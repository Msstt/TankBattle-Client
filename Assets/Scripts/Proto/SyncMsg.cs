public class MsgSyncTank : MsgBase {
  public MsgSyncTank() { ProtoName = "MsgSyncTank"; }

  public float X = 0;
  public float Y = 0;
  public float Z = 0;
  public float EX = 0;
  public float EY = 0;
  public float EZ = 0;
  public float TurretY = 0;
  public float Time = 0;
  public string ID = "";
}

public class MsgFire : MsgBase {
  public MsgFire() { ProtoName = "MsgFire"; }

  public float X = 0;
  public float Y = 0;
  public float Z = 0;
  public float EX = 0;
  public float EY = 0;
  public float EZ = 0;
  public string ID = "";
}

public class MsgHit : MsgBase {
  public MsgHit() { ProtoName = "MsgHit"; }

  public string ID = "";
  public float Health = 0;
}
