#ifndef UDPMANAGER_H
#define UDPMANAGER_H

#include <common.h>
#include <WiFiUdp.h>

class UDPManager
{
    WiFiUDP udp;
    char packetBuffer[255];
    public:
        UDPManager();
        void handleRequests();
};
#endif
