#ifndef WEBSERVERMANAGER_H
#define WEBSERVERMANAGER_H

#include <functional>
#include <WString.h>
#include <WebServer.h>
#include <vector>
#include <common.h>
//change from device to sensor
class WebServerManager
{
    typedef std::function<void(void)> TOnHandler;
    
    typedef std::function<DeviceInformation(void)> TOnGetDeviceInformationHandlerFunction;
    typedef std::function<std::vector<Sensor>(void)> TOnScanHandlerFunction;
    typedef std::function<SensorVersionAndBattery(String address)> TOnGetSensorVersionAndBatteryHandlerFunction;
    typedef std::function<SensorValues(String address)> TOnGetSensorValuesHandlerFunction;
    
    TOnGetDeviceInformationHandlerFunction onGetDeviceInformationHandler;
    TOnScanHandlerFunction onScanHandler;
    TOnGetSensorVersionAndBatteryHandlerFunction onGetSensorVersionAndBatteryHandler;
    TOnGetSensorValuesHandlerFunction onGetSensorValuesHandler;
    TOnHandler onRestartHandler;
    WebServer webServer;

    void onScan();
    void onGetSensorVersionAndBattery();
    void onGetSensorValues();
    private:
        void getDeviceInformation();
        void scan();
        void getSensorVersionAndBattery();
        void getSensorValues();
        void root();
        void restart();
        void notFound();
    public:
        WebServerManager();
        void start();
        void onGetDeviceInformation(TOnGetDeviceInformationHandlerFunction handler);
        void onScan(TOnScanHandlerFunction handler);
        void onGetSensorVersionAndBattery(TOnGetSensorVersionAndBatteryHandlerFunction handler);
        void onGetSensorValues(TOnGetSensorValuesHandlerFunction handler);
        void onRestart(TOnHandler);
        void handleRequests();
};
#endif