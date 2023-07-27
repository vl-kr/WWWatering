# WWWatering

The general purpose of this project is to allow the communication between a lightweight web server behind a NAT and a client connecting from anywhere on the internet.

The project consists of 2 parts: <br>
a) Desktop part, which is mostly a lightweight web server running on a local device behind NAT. <br>
b) Server part, which is an ASP.NET app hosted on the cloud and accessible from anywhere. <br>

In its current implementation, the whole system is meant to allow remote care for a plant, namely monitoring the soil moisture level and remote watering. <br>
To prevent overwatering by another person, the cloud web server uses cookie-based authentication.

1. The desktop app creates an SSH tunnel with the server, using the SSH key specified in App.config.
	If the server cannot be reached or a previously established connection is lost, the app keeps trying to reconnect periodically.
2. The client uses the cloud server interface to request an action, such as watering the plant or retrieving data.
3. The cloud server creates a HTTP request and tunnels it to the local server behind NAT.
4. Upon receiving a response, the cloud server shows the result to the user.

![Diagram](https://github.com/comrade-napoleon/WWWatering/assets/44015502/6ca5323c-33b4-4863-9250-d07ec9d3b07e)
