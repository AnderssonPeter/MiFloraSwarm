#include "Uptime.h"
#include <common.h>
#include <Arduino.h>

static void log(String message)
{
    Serial.println("Uptime: " + message);
}

static String convertToString(byte length, uint value)
{
    String data = String(value);
    while(data.length() < length)
    {
        data = "0" + data;
    }
    return data;
}

static String convertToString(byte length, byte value)
{
    String data = String(value);
    while(data.length() < length)
    {
        data = "0" + data;
    }
    return data;
}

void Uptime::handle()
{
    ulong current = millis();
    ulong delta = current - last;
    while((delta + miliseconds) >= 1000) {
        seconds += 1;
        delta -= 1000;
    }
    miliseconds += delta;
    while(seconds >= 60)
    {
        seconds -= 60;
        minutes += 1;
    }
    while(minutes >= 60)
    {
        minutes -= 60;
        hours += 1;
    }
    while(hours >= 24)
    {
        hours -= 24;
        days += 1;
    }

    last = current;
}

String Uptime::toString()
{
    return convertToString(1, days) + ":" + 
           convertToString(2, hours) + ":" + 
           convertToString(2, minutes) + ":" +
           convertToString(2, seconds) + "." +
           convertToString(3, miliseconds) + "0000";
}