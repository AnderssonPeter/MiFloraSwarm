#include "WebServerManager.h"
#include <ArduinoJson.h>

static void log(String message)
{
    Serial.println("WebServerManager: " + message);
}

WebServerManager::WebServerManager()
: webServer(80)
{    
    log("Creating webServer path handlers");
    webServer.on("/", std::bind(&WebServerManager::root, this));
    webServer.on("/device", std::bind(&WebServerManager::getDeviceInformation, this));
    webServer.on("/sensors/scan", std::bind(&WebServerManager::scan, this));
    webServer.on("/sensors/{}/info", std::bind(&WebServerManager::getSensorVersionAndBattery, this));
    webServer.on("/sensors/{}/values", std::bind(&WebServerManager::getSensorValues, this));
    webServer.on("/device/restart", HTTP_POST, std::bind(&WebServerManager::restart, this));
}

void WebServerManager::start()
{
    log("start");
    webServer.begin();
}

void WebServerManager::root()
{
    log("root");
    webServer.send(200, "text/plain", "ESPFlora " + ESPFloraVersion);
}

void WebServerManager::notFound() {
    log("notFound");
    String message = "File Not Found\n\n";
    message += "URI: ";
    message += webServer.uri();
    message += "\nMethod: ";
    message += (webServer.method() == HTTP_GET) ? "GET" : "POST";
    message += "\nArguments: ";
    message += webServer.args();
    message += "\n";
    for (uint8_t i = 0; i < webServer.args(); i++) {
        message += " " + webServer.argName(i) + ": " + webServer.arg(i) + "\n";
    }
    webServer.send(404, "text/plain", message);
}

void WebServerManager::onGetDeviceInformation(TOnGetDeviceInformationHandlerFunction handler)
{
    this->onGetDeviceInformationHandler = handler;
}

void WebServerManager::onScan(TOnScanHandlerFunction handler)
{
    this->onScanHandler = handler;
}

void WebServerManager::onGetSensorVersionAndBattery(TOnGetSensorVersionAndBatteryHandlerFunction handler)
{
    this->onGetSensorVersionAndBatteryHandler = handler;
}

void WebServerManager::onGetSensorValues(TOnGetSensorValuesHandlerFunction handler)
{
    this->onGetSensorValuesHandler = handler;
}

void WebServerManager::onRestart(TOnHandler handler)
{
    this->onRestartHandler = handler;
}

void WebServerManager::getDeviceInformation()
{
    log("getDeviceInformation");
    auto result = this->onGetDeviceInformationHandler();
    DynamicJsonDocument doc(2048);
    doc["version"] = ESPFloraVersion;
    doc["macAddress"] = result.address;
    doc["uptime"] = result.uptime;
    JsonObject internalRam = doc.createNestedObject("internalRam");
    internalRam["heapSize"] = result.heapSize;
    internalRam["freeSize"] = result.freeHeap;
    internalRam["minFreeHeap"] = result.minFreeHeap;
    internalRam["maxAllocHeap"] = result.maxAllocHeap;
    JsonObject spiRam = doc.createNestedObject("spiRam");
    spiRam["psramSize"] = result.psramSize;
    spiRam["freePsram"] = result.freePsram;
    spiRam["minFreePsram"] = result.minFreePsram;
    spiRam["maxAllocPsram"] = result.maxAllocPsram;
    JsonObject chip = doc.createNestedObject("chip");
    chip["revision"] = result.chipRevision;
    chip["cpuFreqMHz"] = result.cpuFreqMHz;
    chip["cycleCount"] = result.cycleCount;
    chip["sdkVersion"] = result.sdkVersion;
    JsonObject sketch = doc.createNestedObject("sketch");
    sketch["sketchSize"] = result.sketchSize;
    sketch["sketchMD5"] = result.sketchMD5;
    sketch["freeSketchSpace"] = result.freeSketchSpace;
    String output;
    serializeJson(doc, output);
    webServer.send(200, "application/json", output);
}

void WebServerManager::restart()
{
    log("restart");
    webServer.send(404, "text/plain", "Restarting");
    this->onRestartHandler();
}

void WebServerManager::scan()
{
    log("scan");
    //handle on scan here!
    auto result = this->onScanHandler();
    DynamicJsonDocument doc(2048);
    JsonArray Sensors = doc.createNestedArray("Sensors");
    
    for (int i = 0; i < result.size(); i++) {
        JsonObject jsonSensor = Sensors.createNestedObject();

        jsonSensor["name"] = result[i].name;
        jsonSensor["macAddress"] = result[i].address;
        jsonSensor["rssi"] = result[i].rssi;
    }
    String output;
    serializeJson(doc, output);
    webServer.send(200, "application/json", output);
}

void WebServerManager::getSensorVersionAndBattery()
{
    log("getSensorVersionAndBattery");
    String SensorAddress = webServer.pathArg(0);
    log("Sensor: " + SensorAddress);
    auto result = this->onGetSensorVersionAndBatteryHandler(SensorAddress);
    log("Generating JSON");
    switch (result.status)
    {
        case Success:
        {
            DynamicJsonDocument doc(512);
            doc["battery"] = result.battery;
            doc["version"] = result.version;
            doc["rssi"] = result.rssi;
            String output;
            serializeJson(doc, output);
            webServer.send(200, "application/json", output);
            break;
        }
        case NotFound:
            webServer.send(404, "application/json", "{ \"message\": \"Sensor not found!\" }");
            break;
    }
}

void WebServerManager::getSensorValues()
{
    log("getSensorValues");
    String SensorAddress = webServer.pathArg(0);
    log("Sensor: " + SensorAddress);
    auto result = this->onGetSensorValuesHandler(SensorAddress);
       switch (result.status)
    {
        case Success:
        {
            DynamicJsonDocument doc(512);
            doc["temperature"] = result.temperature;
            doc["brightness"] = result.brightness;
            doc["moisture"] = result.moisture;
            doc["conductivity"] = result.conductivity;
            doc["rssi"] = result.rssi;
            String output;
            serializeJson(doc, output);
            webServer.send(200, "application/json", output);
            break;
        }
        case NotFound:
            webServer.send(404, "application/json", "{ \"message\": \"Sensor not found!\" }");
            break;
    }
}

void WebServerManager::handleRequests()
{
    webServer.handleClient();    
}