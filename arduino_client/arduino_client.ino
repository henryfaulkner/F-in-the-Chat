#include <ESP8266WiFi.h>=

const char *ssid = "Kleins Guys";
const char *password = "Redbirds901FC";
char path[] = "/";
char host[] = "ws://10.0.0.227/";

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
