using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MsgBaseTest {

  [Test]
  public void MsgBaseTestMsg() {
    MsgMove msg = new() {
      X = 20,
      Y = -80
    };
    byte[] bytes = MsgBase.Encode(msg);
    msg = MsgBase.Decode("MsgMove", bytes, 0, bytes.Length) as MsgMove;
    Assert.NotNull(msg);
    Assert.AreEqual("MsgMove", msg.ProtoName);
    Assert.AreEqual(20, msg.X);
    Assert.AreEqual(-80, msg.Y);
    Assert.AreEqual(0, msg.Z);
  }

  [Test]
  public void MsgBaseTestMsgName() {
    MsgMove msgMove = new();
    byte[] bs = MsgBase.EncodeName(msgMove);
    string name = MsgBase.DecodeName(bs, 0, out int count);
    Assert.AreEqual("MsgMove", name);
    Assert.AreEqual(9, count);
  }
}
