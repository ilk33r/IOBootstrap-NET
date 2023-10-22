enum DeviceTypes {

    Android = 0,
    iOS = 1,
    Generic = 2,
    Unkown = 999
}

namespace DeviceTypes {

    export function getDeviceName(type: DeviceTypes): string {
        if (type === DeviceTypes.Android) {
            return "Android";
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
