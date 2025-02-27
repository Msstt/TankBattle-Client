using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgPing : MsgBase {
  public MsgPing() { ProtoName = "MsgPing"; }
}

public class MsgPong : MsgBase {
  public MsgPong() { ProtoName = "MsgPong"; }
}

public class MsgTest : MsgBase {
  public MsgTest() { ProtoName = "MsgTest"; }

  public float X = 0;
  public float Y = 0;
  public float Z = 0;
}
