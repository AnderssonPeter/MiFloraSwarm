#include "BLEManager.h"
#include <BLEDevice.h>
#include <BLEAdvertisedDevice.h>
#include <HardwareSerial.h>

static void log(String message)
{
    Serial.println("BLEManager: " + message);
}

static BLEUUID           serviceUUID("00001204-0000-1000-8000-00805F9B34FB");
//static BLEUUID              nameUUID("00002a00-0000-1000-8000-00805f9b34fb"); // 0x03
static BLEUUID versionAndBatteryUUID("00001a02-0000-1000-8000-00805f9b34fb"); // 0x38
static BLEUUID        sensorDataUUID("00001a01-0000-1000-8000-00805f9b34fb"); // 0x35
static BLEUUID        modeChangeUUID("00001a00-0000-1000-8000-00805f9b34fb"); // 0x33

BLEManager::BLEManager() {
    BLEDevice::init("");    
    pBLEScan = BLEDevice::getScan(); //create new scan
    pBLEScan->setActiveScan(true); //active scan uses more power, but get results faster
    pBLEScan->setInterval(100);
    pBLEScan->setWindow(99);  // less or equal setInterval value
};

static uint8_t magicPacket[2] = {0xA0, 0x1F};

std::vector<Sensor> BLEManager::scan()
{
    log("Listing BLE Sensors");
    BLEScanResults foundSensors = pBLEScan->start(10, false);
    
    std::vector<Sensor> Sensors;
    int count = foundSensors.getCount();
    
    log("Found " + String(count));
    for (int i = 0; i < count; i++) {
        BLEAdvertisedDevice bleSensor = foundSensors.getDevice(i);
        String SensorName = bleSensor.getName().c_str();
        if (!SensorName.equalsIgnoreCase("flower care") && !SensorName.equalsIgnoreCase("flower mate")) {
            continue;
        }
        String address = bleSensor.getAddress().toString().c_str();
        int rssi = bleSensor.getRSSI();
        Sensor Sensor;
        Sensor.address = address;
        Sensor.name = SensorName;
        Sensor.rssi = rssi;
        Sensors.push_back(Sensor);
    }
    pBLEScan->clearResults();
    return Sensors;
};

SensorVersionAndBattery BLEManager::getSensorVersionAndBattery(String address)
{
    log("getSensorVersionAndBattery");
    BLEClient* client = BLEDevice::createClient();
    log("Connecting to " + address);
    BLEAddress bleAddress = BLEAddress(address.c_str());
    bool connected = client->connect(bleAddress);
    SensorVersionAndBattery result;
    if (connected)
    {
        log("Connected");
        log("Getting service");
        BLERemoteService* service = client->getService(serviceUUID);
        if (service == nullptr)
        {
            log("Service not found");
            result.status = NotFound;
        }
        else {
            log("Getting characteristic");
            BLERemoteCharacteristic* characteristic = service->getCharacteristic(versionAndBatteryUUID);            
            if (characteristic == nullptr)
            {
                log("Characteristic not found");
                result.status = NotFound;
            }
            else
            {
                log("Reading characteristic value");
                
                auto value = characteristic->readValue();
                log("Parsing value");
                //todo: get rssi
                result.battery = (int)value[0];
                result.version = value.substr(2).c_str();
                result.status = Success;
                
                log("Value parsed");
            }  
        }     
    }
    else {
        result.status = NotFound;
    }
    /*log("Disconnecting");
    client->disconnect();
    log("Disconnected");*/
    return result;
};

SensorValues BLEManager::getSensorValues(String address)
{
    log("getSensorValues");
    BLEClient* client = BLEDevice::createClient();
    log("Connecting to " + address);
    BLEAddress bleAddress = BLEAddress(address.c_str());
    bool connected = client->connect(bleAddress);
    SensorValues result;
    if (connected)
    {
        log("Connected");
        log("Writing change mode bits");
        
        log("Getting service");
        BLERemoteService* service = client->getService(serviceUUID);
        if (service == nullptr) {
            result.status = NotFound;
        }
        else
        {
            log("Getting characteristics");
            BLERemoteCharacteristic* changeModeCharacteristic = service->getCharacteristic(modeChangeUUID);
            BLERemoteCharacteristic* sensorDataCharacteristic = service->getCharacteristic(sensorDataUUID);
            if (changeModeCharacteristic == nullptr || sensorDataCharacteristic == nullptr)
            {
                result.status = NotFound;
            }
            else
            {
                log("Sending magic packet");
                changeModeCharacteristic->writeValue(magicPacket, 2, true);
                log("Reading data");
                auto values = sensorDataCharacteristic->readValue();
                Serial.print("Hex: ");
                for (int i = 0; i < 16; i++) {
                    Serial.print((int)values[i], HEX);
                    Serial.print(" ");
                }
                
                //todo: get rssi
                Serial.print("");
                result.status = Success;
                result.temperature = (values[0] + values[1] * 256) / ((float)10.0);
                result.moisture = values[7];
                result.brightness = values[3] + values[4] * 256;
                result.conductivity = values[8] + values[9] * 256;
            }
        }
    }
    else
    {
        result.status = NotFound;
    }
    /*log("Disconnecting");
    client->disconnect();
    log("Disconnected");*/
    return result;
};