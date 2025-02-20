using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgMove : MsgBase {
  public MsgMove() { ProtoName = "MsgMove"; }

  public int X = 0;
  public int Y = 0;
  public int Z = 0;
}
