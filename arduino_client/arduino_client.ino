#include <ESP8266WiFi.h>=

const char *ssid = "";
const char *password = "";
char path[] = "/";
char host[] = "ws:///";

// Use WiFiClient class to create TCP connections
WiFiClient client;
int buttonState = 0;
bool on = false;
int BUTTON_PIN = 3; // Using RX pin for general boolean input

void setup()
{
  Serial.begin(115200,SERIAL_8N1,SERIAL_TX_ONLY); // TX pin taking responsibility of Serial
  pinMode(BUTTON_PIN, INPUT);
  delay(10);

  // We start by connecting to a WiFi network
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
  if (client.connect("", 80))
  {
    Serial.println("Connected");
    buttonState = digitalRead(BUTTON_PIN);
    Serial.println("buttonState");
    Serial.println(buttonState);
    if(buttonState == 0) {
      if(!on) {
        on = true;
        client.println("on");
        Serial.println("down");
      }
    } else {
      if(on) {
        Serial.println("up");
        client.println("off");
        on = false;
      }
    }
    delay(50);
  }
  else
  {
    Serial.println("Connection failed.");
    while (1)
    {
      // Hang on failure
    }
  }
}

void loop()
{
  if (client.connected())
  {
    //Serial.println("connected");
    buttonState = digitalRead(BUTTON_PIN);
    if(buttonState == 0) {
      if(!on) {
        on = true;
        Serial.println("down");
        client.println("on");
      }
    } else {
      if(on) {
        on = false;
        Serial.println("up");
        client.println("off");
      }
    }
    delay(50);
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
