using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MsgBaseTest {

  [Test]
  public void MsgBaseTestMsg() {
    MsgTest msg = new() {
      X = 20,
      Y = -80
    };
    byte[] bytes = MsgBase.Encode(msg);
    msg = MsgBase.Decode("MsgTest", bytes, 0, bytes.Length) as MsgTest;
    Assert.NotNull(msg);
    Assert.AreEqual("MsgTest", msg.ProtoName);
    Assert.AreEqual(20, msg.X);
    Assert.AreEqual(-80, msg.Y);
    Assert.AreEqual(0, msg.Z);
  }

  [Test]
  public void MsgBaseTestMsgName() {
    MsgTest MsgTest = new();
    byte[] bs = MsgBase.EncodeName(MsgTest);
    string name = MsgBase.DecodeName(bs, 0, out int count);
    Assert.AreEqual("MsgTest", name);
    Assert.AreEqual(9, count);
  }
}
