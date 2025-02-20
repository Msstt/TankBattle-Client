using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Echo : MonoBehaviour {
  public TMP_InputField inputField;
  public TMP_Text text;
  private string textString = string.Empty;

  private Socket socket;
  private bool isClose = false;

  private readonly ByteArray readBuffer = new();
  private readonly Queue<ByteArray> writeQueue = new();

  void Update() {
    text.text = textString;
  }

  public void Connection() {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect("127.0.0.1", 8888);
    Debug.Log("Socket connect success.");
    socket.BeginReceive(readBuffer.buffer, readBuffer.writeIndex, readBuffer.Remain, 0, OnReceive, null);
  }

  private void OnReceive(IAsyncResult ar) {
    try {
      int bytesCount = socket.EndReceive(ar);
      readBuffer.writeIndex += bytesCount;
      OnReceiveDate();
      if (readBuffer.Remain < 8) {
        readBuffer.Resize(2 * readBuffer.Length);
      }
      socket.BeginReceive(readBuffer.buffer, readBuffer.writeIndex, readBuffer.Remain, 0, OnReceive, null);
    } catch (SocketException ex) {
      Debug.Log("Socket receive failed: " + ex);
    }
  }

  private void OnReceiveDate() {
    if (readBuffer.Length <= 2) {
      return;
    }
    Int16 length = readBuffer.ReadInt16();
    if (readBuffer.Length < 2 + length) {
      return;
    }
    textString = textString + "\n" +
      System.Text.Encoding.UTF8.GetString(readBuffer.buffer, readBuffer.readIndex + 2, length);
    readBuffer.readIndex += 2 + length;
    readBuffer.CheckAndMove();
    OnReceiveDate();
  }

  public void Send() {
    if (isClose) {
      return;
    }
    byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(inputField.text);
    Int16 length = (Int16)bodyBytes.Length;
    byte[] lengthBytes = BitConverter.GetBytes(length);
    if (!BitConverter.IsLittleEndian) {
      lengthBytes.Reverse();
    }
    ByteArray sendArray = new(lengthBytes.Concat(bodyBytes).ToArray());
    int queueCount = 0;
    lock (writeQueue) {
      writeQueue.Enqueue(sendArray);
      queueCount = writeQueue.Count;
    }
    if (queueCount == 1) {
      socket.BeginSend(sendArray.buffer, sendArray.readIndex, sendArray.Length, 0, OnSend, null);
    }
  }

  private void OnSend(IAsyncResult ar) {
    try {
      int byteCount = socket.EndSend(ar);
      ByteArray sendArray;
      lock (writeQueue) {
        sendArray = writeQueue.First();
      }
      sendArray.readIndex += byteCount;
      if (sendArray.Length == 0) {
        lock (writeQueue) {
          writeQueue.Dequeue();
          sendArray = writeQueue.First();
        }
      }
      if (sendArray != null) {
        socket.BeginSend(sendArray.buffer, sendArray.readIndex, sendArray.Length, 0, OnSend, null);
      } else if (isClose) {
        socket.Close();
      }
    } catch (SocketException ex) {
      Debug.Log("Socket send failed: " + ex);
    }
  }

  private void Close() {
    isClose = true;
    if (writeQueue.Count > 0) {
      socket.Close();
    }
  }
}
