#include "WiFiManager.h"
#include <WiFi.h>
#include <ESPmDNS.h>

static void log(String message)
{
    Serial.println("WiFiManager: " + message);
}

WiFiManager::WiFiManager(const char* deviceName, const char* ssid, const char* passphrase)
{
    this->deviceName = deviceName;
    this->ssid = ssid;
    this->passphrase = passphrase;
}

void WiFiManager::connect()
{
    log("Initializing WiFi");  
    WiFi.mode(WIFI_STA);
    WiFi.setHostname(deviceName);
    WiFi.begin(ssid, passphrase);
    Serial.print("Connecting to " + String(ssid));
    while (!isConnected()) {
        delay(200);
        Serial.print('.');
    }
    
    Serial.println("");
    log("Connected. IP address is: " + WiFi.localIP().toString() + ", Mac address: " + WiFi.macAddress());

    if (MDNS.begin("esp32")) {
        log("MDNS responder started");
    }
}

String WiFiManager::getMACAddress()
{
    return WiFi.macAddress();
}

bool WiFiManager::isConnected()
{
    return WiFi.status() == WL_CONNECTED;
}