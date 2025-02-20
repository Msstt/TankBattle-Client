public class MsgRegister : MsgBase {
  public MsgRegister() { ProtoName = "MsgRegister"; }

  public string Id = "";
  public string Password = "";
  public int Result = 0;
}

public class MsgLogin : MsgBase {
  public MsgLogin() { ProtoName = "MsgLogin"; }

  public string Id = "";
  public string Password = "";
  public int Result = 0;
}

public class MsgKick : MsgBase {
  public MsgKick() { ProtoName = "MsgKick"; }

  public int Reason = 0;
}
