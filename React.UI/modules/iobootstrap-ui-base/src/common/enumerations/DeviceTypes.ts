enum DeviceTypes {

    AndroidGoogle = 0,
    AndroidHuawei = 1,
    iOS = 2,
    Generic = 3,
    Unkown = 999
}

namespace DeviceTypes {

    export function getDeviceName(type: DeviceTypes): string {
        if (type === DeviceTypes.AndroidGoogle) {
            return "Android Google";
        }

        if (type === DeviceTypes.AndroidHuawei) {
            return "Android Huawei";
        }

        if (type === DeviceTypes.iOS) {
            return "iOS";
        }

        if (type === DeviceTypes.Generic) {
            return "Generic";
        }

        return "Unkown";
    }

}

export default DeviceTypes;
