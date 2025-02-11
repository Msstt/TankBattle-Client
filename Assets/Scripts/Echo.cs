using System;
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
  private readonly byte[] buffer = new byte[1024];
  private string message = string.Empty;

  private void Update() {
    text.text = message;
  }

  public void Connection() {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.BeginConnect("127.0.0.1", 8888, OnConnect, null);
  }

  private void OnConnect(IAsyncResult ar) {
    try {
      socket.EndConnect(ar);
      socket.BeginReceive(buffer, 0, 1024, 0, OnReceive, null);
      Debug.Log("Socket connection success.");
    } catch (SocketException ex) {
      Debug.Log("Socket connection failed!" + ex);
    }
  }

  private void OnReceive(IAsyncResult ar) {
    try {
      int bytesCount = socket.EndReceive(ar);
      message += "\n" + System.Text.Encoding.UTF8.GetString(buffer, 0, bytesCount);
      socket.BeginReceive(buffer, 0, 1024, 0, OnReceive, null);
    } catch (SocketException ex) {
      Debug.Log("Socket receive failed!" + ex);
    }
  }

  public void Send() {
    byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(inputField.text);
    socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, OnSend, null);
  }

  private void OnSend(IAsyncResult ar) {
    try {
      int bytesCount = socket.EndSend(ar);
      Debug.Log("Socket send " + bytesCount + " bytes success.");
    } catch (SocketException ex) {
      Debug.Log("Socket send failed!" + ex);
    }
  }
}
