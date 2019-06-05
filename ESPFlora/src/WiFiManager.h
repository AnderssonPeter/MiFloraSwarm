#ifndef WIFIMANAGER_H
#define WIFIMANAGER_H

class WiFiManager
{
    const char* ssid;
    const char* passphrase;
    public:
        WiFiManager(const char* ssid, const char* passphrase);
        void connect();
        bool isConnected();
};
#endif