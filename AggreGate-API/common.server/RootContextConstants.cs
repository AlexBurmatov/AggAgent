using System;
using com.tibbo.aggregate.common.datatable;

namespace com.tibbo.aggregate.common.server
{
    public class RootContextConstants
    {
        public const String V_VERSION = "version";

        public const String F_REGISTER = "register";
        public const String F_LOGIN = "login";
        public const String F_LOGOUT = "logout";
        public const String E_FEEDBACK = "feedback";

        public const String VF_VERSION_VERSION = "version";

        public const String FIF_REGISTER_USERNAME = "username";
        public const String FIF_REGISTER_PASSWORD = "password";
        public const String FIF_REGISTER_PASSWORDRE = "passwordre";

        public const String FIF_LOGIN_USERNAME = "username";
        public const String FIF_LOGIN_PASSWORD = "password";

        public const String EF_FEEDBACK_MESSAGE = "message";

        public static readonly TableFormat FIFT_LOGIN = new TableFormat(1, 1);

        static RootContextConstants()
        {
            FIFT_LOGIN.addField("<" + FIF_LOGIN_USERNAME + "><S>");
            FIFT_LOGIN.addField("<" + FIF_LOGIN_PASSWORD + "><S>");
        }
    }
}