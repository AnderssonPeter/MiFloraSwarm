#ifndef UDPMANAGER_H
#define UDPMANAGER_H

#include <common.h>
#include <WiFiUdp.h>

class UDPManager
{
    WiFiUDP udp;
    char packetBuffer[255];
    const char* deviceName;
    public:
        UDPManager(const char* deviceName);
        void handleRequests();
};
#endif
