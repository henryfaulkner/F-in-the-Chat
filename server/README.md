### This article helped me implement the server

https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server

- currently running program as root user, using sudo command,
  to connect to ports lower than 1024

#### Handshake

The full explanation of the Server handshake can be found in RFC 6455, section 4.2.2.
Steps:

1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
3. Compute SHA-1 and Base64 hash of the new value
4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
