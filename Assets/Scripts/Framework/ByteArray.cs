using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ByteArray {
  const int DEFAULT_SIZE = 1024;

  public byte[] buffer;
  public int readIndex;
  public int writeIndex;
  public int Length { get => writeIndex - readIndex; }
  public int Remain { get => buffer.Length - writeIndex; }

  public ByteArray(byte[] buffer) {
    this.buffer = buffer;
    readIndex = 0;
    writeIndex = buffer.Length;
  }

  public ByteArray(int bufferSize = DEFAULT_SIZE) {
    buffer = new byte[bufferSize];
    readIndex = 0;
    writeIndex = 0;
  }

  public override string ToString() {
    return BitConverter.ToString(buffer, readIndex, Length);
  }

  public string Debug() {
    return string.Format("readIndex({0}) writeIndex({1} buffer({2}))",
      readIndex, writeIndex, BitConverter.ToString(buffer, 0, buffer.Length));
  }

  public void Resize(int size) {
    if (size < Length) {
      return;
    }
    int newSize = 1;
    while (newSize < size) {
      newSize <<= 1;
    }
    byte[] newBuffer = new byte[newSize];
    Array.Copy(buffer, readIndex, newBuffer, 0, Length);
    buffer = newBuffer;
    writeIndex = Length;
    readIndex = 0;
  }

  public void CheckAndMove() {
    if (Length < 8) {
      Array.Copy(buffer, readIndex, buffer, 0, Length);
      writeIndex = Length;
      readIndex = 0;
    }
  }

  public void Write(byte[] bytes, int offset, int count) {
    if (Remain < count) {
      Resize(Length + count);
    }
    Array.Copy(bytes, offset, buffer, writeIndex, count);
    writeIndex += count;
  }

  public int Read(byte[] bytes, int offset, int count) {
    count = Math.Min(count, Length);
    Array.Copy(buffer, readIndex, bytes, offset, count);
    readIndex += count;
    CheckAndMove();
    return count;
  }

  public Int16 ReadInt16() {
    if (Length < 2) {
      return 0;
    }
    Int16 res = (Int16)((buffer[readIndex + 1] << 8) | buffer[readIndex]);
    CheckAndMove();
    return res;
  }

  public Int32 ReadInt32() {
    if (Length < 4) {
      return 0;
    }
    Int32 res = (Int32)((buffer[readIndex + 3] << 24) |
                        (buffer[readIndex + 2] << 16) |
                        (buffer[readIndex + 1] << 8) |
                         buffer[readIndex]);
    CheckAndMove();
    return res;
  }
}
