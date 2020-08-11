#ifndef UPTIME_H
#define UPTIME_H

#include <common.h>
#include <Arduino.h>

class Uptime
{
    ulong last;
    uint miliseconds;
    byte seconds;
    byte minutes;
    byte hours;
    uint days;
    public:
        void handle();
        String toString();
};
#endif
