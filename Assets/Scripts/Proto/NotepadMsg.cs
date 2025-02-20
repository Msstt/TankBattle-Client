public class MsgGetText : MsgBase {
  public MsgGetText() { ProtoName = "MsgGetText"; }

  public string Text = "";
}

public class MsgSaveText : MsgBase {
  public MsgSaveText() { ProtoName = "MsgSaveText"; }

  public string Text = "";
  public int Result = 0;
}
