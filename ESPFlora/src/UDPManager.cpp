#include "UDPManager.h"


static void log(String message)
{
    Serial.println("UDPManager: " + message);
}

const int serverPort = 16555;
const int clientPort = 16556;

UDPManager::UDPManager(const char* deviceName)
{
    this->deviceName = deviceName;
    Serial.print("Initializing udp port");
    udp.begin(clientPort);
}

void UDPManager::handleRequests()
{
    int packageSize = udp.parsePacket();
    if (packageSize > 0) {
      log("Received packet of size " + String(packageSize) + " bytes from " + udp.remoteIP().toString() + ":" + String(udp.remotePort()));
      
      int len = udp.read(packetBuffer, 255);
      if (len > 0) {
        packetBuffer[len] = 0;
      }
      String str = "{ \"name\": \"" + String(deviceName) + "\", \"version\": \"" + ESPFloraVersion + "\", \"port\": 80 }";
      log("Received \"" + String(packetBuffer) + "\", sending response \"" + str + "\"");
      //byte charBuf[str.length()];
      //str.getBytes(charBuf, str.length());
 
      
      udp.beginPacket(udp.remoteIP(), serverPort);
      udp.print(str);
      //udp.write("MiFlora-Client-ESP32-");
      //udp.write(ESPFloraVersion);
      //udp.write(charBuf, sizeof(charBuf));
      udp.endPacket();
    }
}