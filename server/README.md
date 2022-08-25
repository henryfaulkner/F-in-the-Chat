### This article helped me implement the server

https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server

- currently running program as root user, using sudo command,
  to connect to ports lower than 1024

$dotnet build
$sudo dotnet run --project WebSocketServer/WebSocketServer.csproj

#### Handshake

The full explanation of the Server handshake can be found in RFC 6455, section 4.2.2.
Steps:

1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
3. Compute SHA-1 and Base64 hash of the new value
4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response

### Script to install dotnet to Raspberry Pi

https://www.petecodes.co.uk/install-and-use-microsoft-dot-net-6-with-the-raspberry-pi/

### .NET Core version

Raspberry Pi needs netcoreapp6.0
Mac needs netcoreapp3.1

### Install Iot.Device.Binding to WebSocketServer

$dotnet add package Iot.Device.Bindings --version 2.1.0
