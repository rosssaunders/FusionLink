using System;

namespace RxdSolutions.FusionLink
{
    public class UserRight
    {
        private const string RightName = "FusionLink";

        public static bool CanOpen()
        {
            using (var userRights = LoadUserRights())
            {
                if(IsUserManager(userRights))
                {
                    return true;
                }

                if (IsRightEnabled(userRights))
                {
                    return true;
                }

                if (userRights.GetUserDefRight(RightName) == eMRightStatusType.M_rsSameAsParent)
                {
                    using (var groupRights = LoadUserRights(userRights.GetParentID()))
                    {
                        return IsRightEnabled(groupRights);
                    }
                }

                return false;
            }
        }

        private static CSMUserRights LoadUserRights(int? groupId = null)
        {
            CSMUserRights userRights;
            if (!groupId.HasValue)
                userRights = new CSMUserRights();
            else
                userRights = new CSMUserRights((uint)groupId.Value);
            
            userRights.LoadDetails();
            return userRights;
        }

        private static bool IsRightEnabled(CSMUserRights userRights)
        {
            if (userRights.GetUserDefRight(RightName) == eMRightStatusType.M_rsEnable)
            {
                return true;
            }

            return false;
        }

        private static bool IsUserManager(CSMUserRights userRights)
        {
            using (var name = userRights.GetName())
            {
                if(name.StringValue.Equals("MANAGER", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
