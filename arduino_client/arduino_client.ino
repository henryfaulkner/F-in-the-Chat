#include <ESP8266WiFi.h>
#include "WebSocketClient.h"

const char *ssid = "Kleins Guys";
const char *password = "Redbirds901FC";
char path[] = "/";
char host[] = "ws://10.0.0.227/";

WebSocketClient webSocketClient;

// Use WiFiClient class to create TCP connections
WiFiClient client;
int buttonState = 0;
bool on = false;
int BUTTON_PIN = 2;

void setup()
{
  Serial.begin(9600);
  pinMode(BUTTON_PIN, INPUT);
  delay(10);

  // We start by connecting to a WiFi network

  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  delay(5000);

  // Connect to the websocket server
  if (client.connect("10.0.0.227", 80))
  {
    Serial.println("Connected");
  }
  else
  {
    Serial.println("Connection failed.");
    while (1)
    {
      // Hang on failure
    }
  }

  // Handshake with the server
  webSocketClient.path = path;
  webSocketClient.host = host;
  if (webSocketClient.handshake(client))
  {
    Serial.println("Handshake successful");
  }
  else
  {
    Serial.println("Handshake failed.");
    while (1)
    {
      // Hang on failure
    }
  }
}

void loop()
{
  String data;

  buttonState = digitalRead(BUTTON_PIN);
  if (buttonState == 0)
  {
    if (!on)
    {
      on = true;
      //webSocketClient.sendData("off");
      Serial.println("down");
    }
  }
  else
  {
    if (on)
    {
      Serial.println("up");
      //webSocketClient.sendData("off");
      on = false;
    }
  }
  delay(50);

  if (client.connected())
  {
    Serial.println("connected");
    webSocketClient.getData(data);
    if (data.length() > 0)
    {
      Serial.print("Received data: ");
      Serial.println(data);
    }
    
    pinMode(1, INPUT);
    data = String(analogRead(1));
    
    webSocketClient.sendData(data);
  }
  else
  {
    Serial.println("Client disconnected.");
    while (1)
    {
      // Hang on disconnect.
    }
  }

  // wait to fully let the client disconnect
  delay(3000);
}
