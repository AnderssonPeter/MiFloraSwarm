#ifndef BLEMANAGER_H
#define BLEMANAGER_H

#include <common.h>
#include <WString.h>
#include <vector>
#include <BLEScan.h>

class BLEManager
{
    const char* deviceName;
    BLEScan* pBLEScan;
    public:
        BLEManager(const char* deviceName);
        std::vector<Sensor> scan();
        SensorVersionAndBattery getSensorVersionAndBattery(String address);
        SensorValues getSensorValues(String address);
};
#endif
