#include <Arduino.h>
#include "WiFiManager.h"
#include "BLEManager.h"
#include "WebServerManager.h"
#include "UDPManager.h"
#include "Uptime.h"
#include "config.h"
#include "esp_system.h"

//todo:
//* Watchdog
//* Uptime in DeviceInformation

static void log(String message)
{
    Serial.println("main: " + message);
}
const int wdtTimeout = 30000;  //time in ms to trigger the watchdog
hw_timer_t *timer = NULL;

void IRAM_ATTR resetModule(){
    ets_printf("Watchdog reboot\n");
    esp_restart();
}

WiFiManager* wifiManager;
BLEManager* bleManager;
WebServerManager* webServerManager;
UDPManager* updManager;
Uptime* uptime;

std::vector<Sensor> onScan()
{
    return bleManager->scan();
}

SensorVersionAndBattery onGetSensorVersionAndBattery(String address)
{
    log("onGetSensorVersionAndBattery");
    auto result = bleManager->getSensorVersionAndBattery(address);
    log("after onGetSensorVersionAndBattery");
    return result;
}

SensorValues onGetSensorValues(String address)
{
    log("onGetSensorValues");
    return bleManager->getSensorValues(address);
}

void onRestart()
{
    log("onRestart");
    delay(1000);
    ESP.restart();
}

DeviceInformation onGetDeviceInformation()
{
    log("onGetDeviceInformation");
    DeviceInformation result;
    
    result.uptime = uptime->toString();
    result.address = wifiManager->getMACAddress();

    log("Internal RAM");
    result.heapSize = ESP.getHeapSize();
    result.freeHeap = ESP.getFreeHeap();
    result.minFreeHeap = ESP.getMinFreeHeap(); 
    result.maxAllocHeap = ESP.getMaxAllocHeap();
    
    log("SPI RAM");
    result.psramSize = ESP.getPsramSize();
    result.freePsram = ESP.getFreePsram();
    result.minFreePsram = ESP.getMinFreePsram();
    result.maxAllocPsram = ESP.getMaxAllocPsram();
    
    log("Chip");
    result.chipRevision = ESP.getChipRevision();
    result.cpuFreqMHz = ESP.getCpuFreqMHz();
    result.cycleCount = ESP.getCycleCount();
    result.sdkVersion = ESP.getSdkVersion();
    
    log("Sketch");
    result.sketchSize = ESP.getSketchSize();
    result.sketchMD5 = ESP.getSketchMD5();
    result.freeSketchSpace = ESP.getFreeSketchSpace();
    return result;
}

void setup() {
    Serial.begin(115200);
    log("Creating Uptime");
    uptime = new Uptime();
    log("Creating WiFiManager");
    wifiManager = new WiFiManager(ssid, password);
    log("Connecting to wifi");
    wifiManager->connect();
    log("Creating BLEManager");
    bleManager = new BLEManager();
    log("Creating WebServerManager");
    webServerManager = new WebServerManager();
    log("Configuring WebServerManager");
    webServerManager->onGetDeviceInformation(onGetDeviceInformation);
    webServerManager->onRestart(onRestart);
    webServerManager->onScan(onScan);
    webServerManager->onGetSensorVersionAndBattery(onGetSensorVersionAndBattery);
    webServerManager->onGetSensorValues(onGetSensorValues);
    log("Starting webServer");
    webServerManager->start();

    updManager = new UDPManager();    
    log("starting watchdog");
    timer = timerBegin(0, 80, true);                  //timer 0, div 80
    timerAttachInterrupt(timer, &resetModule, true);  //attach callback
    timerAlarmWrite(timer, wdtTimeout * 1000, false); //set time in us
    timerAlarmEnable(timer);                          //enable interrupt
    log("Setup done!");  
}

void loop() {
    timerWrite(timer, 0); //reset timer (feed watchdog)
    if (!wifiManager->isConnected()) {
        log("Lost connection to wifi, rebooting in 5 seconds");
        delay(5000);
        ESP.restart();
    }
    uptime->handle();
    updManager->handleRequests();
    webServerManager->handleRequests();
}