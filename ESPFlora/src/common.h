#ifndef COMMON_H
#define COMMON_H
#include <WString.h>
#include <stdint.h>

enum Status {Success, NotFound};

static const String ESPFloraVersion = "1.0";

typedef struct {
    String address;
    String name;
    int rssi;
} Sensor;

typedef struct {
    Status status;
    String version;
    int battery;
    int rssi;
} SensorVersionAndBattery;

typedef struct {
    Status status;
    int moisture;
    int brightness;
    float temperature;
    int conductivity;
    int rssi;
} SensorValues;

typedef struct {
    String uptime;
    String address;

    //Internal RAM
    uint32_t heapSize; //total heap size
    uint32_t freeHeap; //available heap
    uint32_t minFreeHeap; //lowest level of free heap since boot
    uint32_t maxAllocHeap; //largest block of heap that can be allocated at once

    //SPI RAM
    uint32_t psramSize;
    uint32_t freePsram;
    uint32_t minFreePsram;
    uint32_t maxAllocPsram;

    //Chip
    uint8_t chipRevision;
    uint8_t cpuFreqMHz;
    uint32_t cycleCount;
    String sdkVersion;

    //Sketch
    uint32_t sketchSize;
    String sketchMD5;
    uint32_t freeSketchSpace;
} DeviceInformation;

#endif