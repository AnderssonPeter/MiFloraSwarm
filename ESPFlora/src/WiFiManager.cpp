#include "WiFiManager.h"
#include <WiFi.h>
#include <ESPmDNS.h>

static void log(String message)
{
    Serial.println("WiFiManager: " + message);
}

WiFiManager::WiFiManager(const char* ssid, const char* passphrase)
{
    this->ssid = ssid;
    this-> passphrase = passphrase;
}

void WiFiManager::connect()
{
    log("Initializing WiFi");  
    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, passphrase);
    Serial.print("Connecting to "); log(ssid);
    while (!isConnected()) {
        delay(200);
        Serial.print('.');
    }
    
    Serial.println("");
    Serial.print("Connected. IP address is: ");
    log(WiFi.localIP());

    if (MDNS.begin("esp32")) {
        log("MDNS responder started");
    }
}

bool WiFiManager::isConnected()
{
    return WiFi.status() == WL_CONNECTED;
}