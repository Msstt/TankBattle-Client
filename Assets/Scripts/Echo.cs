using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour {
  public TMP_InputField inputField;
  public TMP_Text text;

  private Socket socket;

  public void Connection() {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect("127.0.0.1", 8888);
    Debug.Log("Socket connection success.");
  }

  public void Send() {
    byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(inputField.text);
    socket.Send(sendBytes);
    byte[] buffer = new byte[1024];
    int bytesCount = socket.Receive(buffer);
    text.text = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesCount);
    socket.Close();
  }
}
