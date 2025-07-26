enum UserRoles {

    SuperAdmin = 0,
    Admin = 1,
    User = 2,
    AnonmyMouse = 999
}

namespace UserRoles {

    export function getRoleName(role: UserRoles): string {
        if (role === UserRoles.SuperAdmin) {
            return "Owner";
        }

        if (role === UserRoles.Admin) {
            return "Admin";
        }

        if (role === UserRoles.User) {
            return "User";
        }

        return "Undefined";
    }

    export function fromRawValue(rawValue: number): UserRoles {
        if (rawValue === 0) {
            return UserRoles.SuperAdmin;
        }

        if (rawValue === UserRoles.Admin) {
            return UserRoles.Admin;
        }

        if (rawValue === UserRoles.User) {
            return UserRoles.User;
        }

        return UserRoles.AnonmyMouse;
    }
}

export default UserRoles;
