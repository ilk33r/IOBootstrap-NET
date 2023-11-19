enum BOPagePropertyType {

    Int = 0,
    String = 1,
    Double = 2,
    Float = 3,
    DateTimeOffset = 4,
    Enum = 4
}

namespace BOPagePropertyType {

    export function getBOPagePropertyTypeName(type: BOPagePropertyType): string {
        if (type === BOPagePropertyType.Int) {
            return "Int";
        }

        if (type === BOPagePropertyType.String) {
            return "String";
        }

        if (type === BOPagePropertyType.Double) {
            return "Double";
        }

        if (type === BOPagePropertyType.Float) {
            return "Float";
        }

        if (type === BOPagePropertyType.DateTimeOffset) {
            return "DateTimeOffset";
        }

        if (type === BOPagePropertyType.Enum) {
            return "Enum";
        }

        return "";
    }

}

export default BOPagePropertyType;