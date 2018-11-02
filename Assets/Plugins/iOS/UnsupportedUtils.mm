#import <Foundation/Foundation.h>
#import <AssetsLibrary/AssetsLibrary.h>
#import <AVFoundation/AVFoundation.h>
#include <sys/types.h>
#include <sys/sysctl.h>

extern "C" void _SaveJpegToCameraRoll(unsigned char *jpegData, int32_t jpegDataLength)
{
    NSData *data = [NSData dataWithBytesNoCopy:jpegData length:jpegDataLength freeWhenDone:NO];
    if (data) {
        UIImage *image = [UIImage imageWithData:data];
        if (image) {
            UIImageWriteToSavedPhotosAlbum(image, nil, nil, NULL);
        }
    }
}

// ---------------
enum MachineIdx{
    MACHINE_IDX_IPHONE_3GS = 0, //iPhone3GS
    MACHINE_IDX_IPHONE_4,       //iPhone4
    MACHINE_IDX_IPHONE_4S,      //iPhone4S
    MACHINE_IDX_IPHONE_5,       //iPhone5
    MACHINE_IDX_IPHONE_5C,      //iPhone5c
    MACHINE_IDX_IPHONE_5S,      //iPhone5s
    MACHINE_IDX_IPHONE_6,       //iPhone6
    MACHINE_IDX_IPHONE_6P,      //iPhone6Plus
    MACHINE_IDX_IPOD_TOUCH_4,   //iPodTouch4
    MACHINE_IDX_IPOD_TOUCH_5,   //iPodTouch5
    MACHINE_IDX_IPAD_2,         //iPad2
    MACHINE_IDX_IPAD_3,         //iPad3
    MACHINE_IDX_IPAD_4,         //iPad4
    MACHINE_IDX_IPAD_AIR,       //iPadAir
    MACHINE_IDX_IPAD_MINI,      //iPadMini
    MACHINE_IDX_IPHONE_6S,      //iPhone6s
    MACHINE_IDX_IPHONE_6SP,     //iPhone6sPlus
    MACHINE_IDX_IPHONE_SE,      //iPhoneSE
    MACHINE_IDX_IPHONE_7,       //iPhone7
    MACHINE_IDX_IPHONE_7P,      //iPhone7Plus
    MACHINE_IDX_IPHONE_8,       //iPhone8
    MACHINE_IDX_IPHONE_8P,      //iPhone8Plus
    MACHINE_IDX_IPHONE_X,       //iPhoneX
    MACHINE_IDX_IPOD_TOUCH_6,   //iPodTouch6
    MACHINE_IDX_IPAD_AIR_2,     //iPadAir2
    MACHINE_IDX_IPAD_5,         //iPad5
    MACHINE_IDX_IPAD_PRO_9_7,   //iPadPro
    MACHINE_IDX_IPAD_PRO_10_5,  //iPadPro
    MACHINE_IDX_IPAD_PRO_12_9,  //iPadPro
    MACHINE_IDX_IPAD_PRO_12_9_2,//iPadPro
    MACHINE_IDX_IPAD_MINI_2,    //iPadMini2
    MACHINE_IDX_IPAD_MINI_3,    //iPadMini3
    MACHINE_IDX_IPAD_MINI_4,    //iPadMini4
    MACHINE_IDX_UNKNOWN_IPHONE,
    MACHINE_IDX_UNKNOWN_IPOD,
    MACHINE_IDX_UNKNOWN_IPAD,
    MACHINE_IDX_UNKNOWN,
};

typedef struct  {
    const MachineIdx machineIdx_;
    const float focalLengthMillis_;
    const float imageSensorHeightMillis_;
} focalLengthAndSensorHeight;

static focalLengthAndSensorHeight focalLengthAndSensorHeightListBack[] = {
    {MACHINE_IDX_IPHONE_4S, 4.12, 3.3},
    {MACHINE_IDX_IPHONE_5, 4.12, 3.3},
    {MACHINE_IDX_IPHONE_5C, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_5S, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_6, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_6P, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_6S, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_6SP, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_SE, 4.15, 3.6},
    {MACHINE_IDX_IPHONE_7, 3.99, 4.15},
    {MACHINE_IDX_IPHONE_7P, 3.99, 4.15},
    {MACHINE_IDX_IPHONE_8, 3.99, 4.15},
    {MACHINE_IDX_IPHONE_8P, 3.99, 4.15},
    {MACHINE_IDX_IPHONE_X, 3.99, 4.15},
    {MACHINE_IDX_UNKNOWN_IPHONE, 3.99, 4.15},
    {MACHINE_IDX_IPOD_TOUCH_5, 4.12, 3.3},
    {MACHINE_IDX_IPOD_TOUCH_6, 4.15, 3.6},
    {MACHINE_IDX_UNKNOWN_IPOD, 4.15, 3.6},
    {MACHINE_IDX_IPAD_2, 4.12, 3.3},
    {MACHINE_IDX_IPAD_3, 4.12, 3.3},
    {MACHINE_IDX_IPAD_4, 4.12, 3.3},
    {MACHINE_IDX_IPAD_AIR, 4.12, 3.3},
    {MACHINE_IDX_IPAD_AIR_2, 4.15, 3.6},
    {MACHINE_IDX_IPAD_5, 4.15, 3.6},
    {MACHINE_IDX_IPAD_MINI, 4.12, 3.3},
    {MACHINE_IDX_IPAD_MINI_2, 4.12, 3.3},
    {MACHINE_IDX_IPAD_MINI_3, 4.12, 3.3},
    {MACHINE_IDX_IPAD_MINI_4, 4.15, 3.6},
    {MACHINE_IDX_IPAD_PRO_9_7, 4.15, 3.6},
    {MACHINE_IDX_IPAD_PRO_10_5, 4.15, 3.6},
    {MACHINE_IDX_IPAD_PRO_12_9, 4.15, 3.6},
    {MACHINE_IDX_IPAD_PRO_12_9_2, 4.15, 3.6},
    {MACHINE_IDX_UNKNOWN_IPAD, 4.15, 3.6},
    {MACHINE_IDX_UNKNOWN, 4.12, 3.3},
};

static focalLengthAndSensorHeight focalLengthAndSensorHeightListFront[] = {
    {MACHINE_IDX_IPHONE_4S, 2.18, 1.5},
    {MACHINE_IDX_IPHONE_5, 2.18, 1.5},
    {MACHINE_IDX_IPHONE_5C, 2.15, 1.5},
    {MACHINE_IDX_IPHONE_5S, 2.15, 1.5},
    {MACHINE_IDX_IPHONE_6, 2.65, 2.0},
    {MACHINE_IDX_IPHONE_6P, 2.65, 2.0},
    {MACHINE_IDX_IPHONE_6S, 2.65, 2.0},
    {MACHINE_IDX_IPHONE_6SP, 2.65, 2.0},
    {MACHINE_IDX_IPHONE_SE, 2.15, 1.5},
    {MACHINE_IDX_IPHONE_7, 2.87, 3.3},
    {MACHINE_IDX_IPHONE_7P, 2.87, 3.3},
    {MACHINE_IDX_IPHONE_8, 2.87, 3.3},      // 1/3.2
    {MACHINE_IDX_IPHONE_8P, 2.87, 3.3},     // 1/3.2
    {MACHINE_IDX_IPHONE_X, 2.87, 3.3},     // 1/3.2
    {MACHINE_IDX_UNKNOWN_IPHONE, 2.87, 3.3},
    {MACHINE_IDX_IPOD_TOUCH_5, 2.15, 1.5},
    {MACHINE_IDX_IPOD_TOUCH_6, 2.15, 1.5},
    {MACHINE_IDX_UNKNOWN_IPOD, 2.15, 1.5},
    {MACHINE_IDX_IPAD_2, 2.15, 1.5},
    {MACHINE_IDX_IPAD_3, 2.15, 1.5},
    {MACHINE_IDX_IPAD_4, 2.15, 1.5},
    {MACHINE_IDX_IPAD_AIR, 2.15, 1.5},
    {MACHINE_IDX_IPAD_AIR_2, 2.15, 1.5},
    {MACHINE_IDX_IPAD_5, 2.15, 1.5},
    {MACHINE_IDX_IPAD_MINI, 2.18, 1.5},
    {MACHINE_IDX_IPAD_MINI_2, 2.18, 1.5},
    {MACHINE_IDX_IPAD_MINI_3, 2.18, 1.5},
    {MACHINE_IDX_IPAD_MINI_4, 2.18, 1.5},
    {MACHINE_IDX_IPAD_PRO_9_7, 2.65, 2.0},
    {MACHINE_IDX_IPAD_PRO_10_5, 2.65, 2.0},
    {MACHINE_IDX_IPAD_PRO_12_9, 2.65, 2.0},
    {MACHINE_IDX_IPAD_PRO_12_9_2, 2.65, 2.0},
    {MACHINE_IDX_UNKNOWN_IPAD, 2.65, 2.0},
    {MACHINE_IDX_UNKNOWN, 2.18, 1.5},
};

static size_t getDeviceName(char* name, size_t maxLength)
{
    size_t length = maxLength;
    sysctlbyname("hw.machine", name, &length, NULL, 0);
    return length;
}

static MachineIdx getDeviceLabel()
{
    char deviceName[128];
    getDeviceName(deviceName, 128);
    
    if( memcmp(deviceName, "iPhone2,", 8) == 0 ){
        return MACHINE_IDX_IPHONE_3GS;
    }else if( memcmp(deviceName, "iPhone3,", 8) == 0){
        return MACHINE_IDX_IPHONE_4;
    }else if( memcmp(deviceName, "iPhone4,", 8) == 0){
        return MACHINE_IDX_IPHONE_4S;
    }else if( memcmp(deviceName, "iPhone5,1", 9) == 0 ||
             memcmp(deviceName, "iPhone5,2", 9) == 0){
        return MACHINE_IDX_IPHONE_5;
    }else if( memcmp(deviceName, "iPhone5,3", 9) == 0 ||
             memcmp(deviceName, "iPhone5,4", 9) == 0){
        return MACHINE_IDX_IPHONE_5C;
    }else if( memcmp(deviceName, "iPhone6,", 8) == 0){
        return MACHINE_IDX_IPHONE_5S;
    }else if( memcmp(deviceName, "iPhone7,2", 9) == 0){
        return MACHINE_IDX_IPHONE_6;
    }else if( memcmp(deviceName, "iPhone7,1", 9) == 0){
        return MACHINE_IDX_IPHONE_6P;
    }else if( memcmp(deviceName, "iPhone8,2", 9) == 0){
        return MACHINE_IDX_IPHONE_6SP;
    }else if( memcmp(deviceName, "iPhone8,1", 9) == 0){
        return MACHINE_IDX_IPHONE_6S;
    }else if( memcmp(deviceName, "iPhone8,4", 9) == 0){
        return MACHINE_IDX_IPHONE_SE;
    }else if( memcmp(deviceName, "iPhone9,2", 9) == 0 ||
             memcmp(deviceName, "iPhone9,4", 9) == 0){
        return MACHINE_IDX_IPHONE_7P;
    }else if( memcmp(deviceName, "iPhone9,1", 9) == 0 ||
             memcmp(deviceName, "iPhone9,3", 9) == 0){
        return MACHINE_IDX_IPHONE_7;
    }else if( memcmp(deviceName, "iPhone10,2", 10) == 0 ||
             memcmp(deviceName, "iPhone10,5", 10) == 0){
        return MACHINE_IDX_IPHONE_8P;
    }else if( memcmp(deviceName, "iPhone10,1", 10) == 0 ||
             memcmp(deviceName, "iPhone10,4", 10) == 0){
        return MACHINE_IDX_IPHONE_8;
    }else if( memcmp(deviceName, "iPhone10,3", 10) == 0 ||
             memcmp(deviceName, "iPhone10,6", 10) == 0){
        return MACHINE_IDX_IPHONE_X;
    }
    
    else if( memcmp(deviceName, "iPod4,", 6) == 0){
        return MACHINE_IDX_IPOD_TOUCH_4;
    }else if( memcmp(deviceName, "iPod5,", 6) == 0){
        return MACHINE_IDX_IPOD_TOUCH_5;
    }else if( memcmp(deviceName, "iPod7,", 6) == 0){
        return MACHINE_IDX_IPOD_TOUCH_6;
    }
    
    else if( memcmp(deviceName, "iPad2,1", 7) == 0 ||
            memcmp(deviceName, "iPad2,2", 7) == 0 ||
            memcmp(deviceName, "iPad2,3", 7) == 0 ||
            memcmp(deviceName, "iPad2,4", 7) == 0 ){
        return MACHINE_IDX_IPAD_2;
    }else if( memcmp(deviceName, "iPad2,5", 7) == 0 ||
             memcmp(deviceName, "iPad2,6", 7) == 0 ||
             memcmp(deviceName, "iPad2,7", 7) == 0 ){
        return MACHINE_IDX_IPAD_MINI;
    }else if( memcmp(deviceName, "iPad3,1", 7) == 0 ||
             memcmp(deviceName, "iPad3,2", 7) == 0 ||
             memcmp(deviceName, "iPad3,3", 7) == 0 ){
        return MACHINE_IDX_IPAD_3;
    }else if( memcmp(deviceName, "iPad3,4", 7) == 0 ||
             memcmp(deviceName, "iPad3,5", 7) == 0 ||
             memcmp(deviceName, "iPad3,6", 7) == 0 ){
        return MACHINE_IDX_IPAD_4;
    }else if( memcmp(deviceName, "iPad4,1", 7) == 0 ||
             memcmp(deviceName, "iPad4,2", 7) == 0 ||
             memcmp(deviceName, "iPad4,3", 7) == 0 ){
        return MACHINE_IDX_IPAD_AIR;
    }else if( memcmp(deviceName, "iPad4,4", 7) == 0 ||
             memcmp(deviceName, "iPad4,5", 7) == 0 ||
             memcmp(deviceName, "iPad4,6", 7) == 0 ){
        return MACHINE_IDX_IPAD_MINI_2;
    }else if( memcmp(deviceName, "iPad4,7", 7) == 0 ||
             memcmp(deviceName, "iPad4,8", 7) == 0 ||
             memcmp(deviceName, "iPad4,9", 7) == 0 ){
        return MACHINE_IDX_IPAD_MINI_3;
    }else if( memcmp(deviceName, "iPad5,1", 7) == 0 ||
             memcmp(deviceName, "iPad5,2", 7) == 0 ){
        return MACHINE_IDX_IPAD_MINI_4;
    }else if( memcmp(deviceName, "iPad5,3", 7) == 0 ||
             memcmp(deviceName, "iPad5,4", 7) == 0 ){
        return MACHINE_IDX_IPAD_AIR_2;
    }else if( memcmp(deviceName, "iPad6,7", 7) == 0 ||
             memcmp(deviceName, "iPad6,8", 7) == 0){
        return MACHINE_IDX_IPAD_PRO_12_9;
    }else if( memcmp(deviceName, "iPad6,3", 7) == 0 ||
             memcmp(deviceName, "iPad6,4", 7) == 0){
        return MACHINE_IDX_IPAD_PRO_9_7;
    }else if( memcmp(deviceName, "iPad6,11", 8) == 0 ||
             memcmp(deviceName, "iPad6,12", 8) == 0){
        return MACHINE_IDX_IPAD_5;
    }else if( memcmp(deviceName, "iPad7,1", 8) == 0 ||
             memcmp(deviceName, "iPad7,2", 8) == 0){
        return MACHINE_IDX_IPAD_PRO_12_9_2;
    }else if( memcmp(deviceName, "iPad7,3", 8) == 0 ||
             memcmp(deviceName, "iPad7,4", 8) == 0){
        return MACHINE_IDX_IPAD_PRO_10_5;
    }
    else if( memcmp(deviceName, "iPhone", 6) == 0){
        return MACHINE_IDX_UNKNOWN_IPHONE;
    } else if( memcmp(deviceName, "iPod", 4) == 0){
        return MACHINE_IDX_UNKNOWN_IPOD;
    } else if( memcmp(deviceName, "iPad", 4) == 0){
        return MACHINE_IDX_UNKNOWN_IPAD;
    } else {
        return MACHINE_IDX_UNKNOWN;
    }
}

static focalLengthAndSensorHeight getFocalLengthAndSensorHeight(MachineIdx idx, bool isFront)
{
    if (isFront) {
        int listSize = sizeof(focalLengthAndSensorHeightListFront) / sizeof(focalLengthAndSensorHeightListFront[0]);
        for (int i = 0; i < listSize; i++) {
            if (focalLengthAndSensorHeightListFront[i].machineIdx_ == idx) {
                return focalLengthAndSensorHeightListFront[i];
            }
        }
    }
    else {
        int listSize = sizeof(focalLengthAndSensorHeightListBack) / sizeof(focalLengthAndSensorHeightListBack[0]);
        for (int i = 0; i < listSize; i++) {
            if (focalLengthAndSensorHeightListBack[i].machineIdx_ == idx) {
                return focalLengthAndSensorHeightListBack[i];
            }
        }
    }
    // error
    return {MACHINE_IDX_UNKNOWN, 0.0f, 0.0f};
}

extern "C" float _GetFocalLength(bool isFront)
{
    focalLengthAndSensorHeight flsh = getFocalLengthAndSensorHeight(getDeviceLabel(), isFront);
    return flsh.focalLengthMillis_;
}

extern "C" float _GetImageSensorHeightMillis(bool isFront)
{
    focalLengthAndSensorHeight flsh = getFocalLengthAndSensorHeight(getDeviceLabel(), isFront);
    return flsh.imageSensorHeightMillis_;
}
