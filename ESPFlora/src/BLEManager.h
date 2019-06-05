#ifndef BLEMANAGER_H
#define BLEMANAGER_H

#include <common.h>
#include <WString.h>
#include <vector>
#include <BLEScan.h>

class BLEManager
{
    BLEScan* pBLEScan;
    public:
        BLEManager();
        std::vector<Sensor> scan();
        SensorVersionAndBattery getSensorVersionAndBattery(String address);
        SensorValues getSensorValues(String address);
};
#endif
