using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour {
  public TMP_InputField inputField;
  public TMP_Text text;

  private Socket socket;
  private readonly byte[] buffer = new byte[1024];

  private void Update() {
    if (socket == null) {
      return;
    }
    if (socket.Poll(0, SelectMode.SelectRead)) {
      int bytesCount = socket.Receive(buffer);
      Debug.Log("S" + bytesCount);
      text.text = text.text + "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, bytesCount);
    }
  }

  public void Connection() {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect("127.0.0.1", 8888);
    Debug.Log("Socket connect success.");
  }

  public void Send() {
    byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(inputField.text);
    socket.Send(sendBytes);
  }
}
