import { FormDataOptionModel, UserRoles } from "iobootstrap-bo-base";
import { DIHooks } from "iobootstrap-ui-base";

class DIUserRoleHooks {

    static setup() {
        DIHooks.Instance.setHookForKey("userRoleName", function (param: any | null): any | null {
            if (param == null) {
                return null;
            }

            const numberParam = Number(param);
            if (numberParam === undefined) {
                return null;
            }


            const role: UserRoles = UserRoles.fromRawValue(numberParam);
            return UserRoles.getRoleName(role);
        });
        
        DIHooks.Instance.setHookForKey("userRoleFormDataOptions", function (param: any | null): any | null {
            const options: FormDataOptionModel[] = [
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.SuperAdmin), UserRoles.SuperAdmin.toString()),
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.Admin), UserRoles.Admin.toString()),
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.User), UserRoles.User.toString()),
            ];

            return options;
        });
    }
}

export default DIUserRoleHooks;
