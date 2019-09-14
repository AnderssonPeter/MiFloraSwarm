#ifndef WIFIMANAGER_H
#define WIFIMANAGER_H

#include <Arduino.h>

class WiFiManager
{
    const char* ssid;
    const char* passphrase;
    public:
        WiFiManager(const char* ssid, const char* passphrase);
        String getMACAddress();
        void connect();
        bool isConnected();
};
#endif