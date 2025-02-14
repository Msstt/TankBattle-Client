using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ByteArrayTest {
  [Test]
  public void ByteArrayTestSimplePasses() {
    ByteArray array = new(8);
    Debug.Log(array.Debug());
    Debug.Log(array.ToString());
    byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
    array.Write(bytes, 0, 5);
    Assert.AreEqual(5, array.Length);
    Debug.Log(array.Debug());
    Debug.Log(array.ToString());
    bytes = new byte[4];
    array.Read(bytes, 0, 2);
    Assert.AreEqual(3, array.Length);
    Debug.Log(array.Debug());
    Debug.Log(array.ToString());
    Debug.Log(BitConverter.ToString(bytes));
    bytes = new byte[] { 6, 7, 8, 9, 10, 11 };
    array.Write(bytes, 0, 6);
    Assert.AreEqual(9, array.Length);
    Assert.AreEqual(16, array.buffer.Length);
    Debug.Log(array.Debug());
    Debug.Log(array.ToString());
  }
}
