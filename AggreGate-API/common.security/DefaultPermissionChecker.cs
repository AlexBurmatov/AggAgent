using System;
using System.Collections.Generic;
using System.Security;
using com.tibbo.aggregate.common.context;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.security
{
    public class DefaultPermissionChecker
    {
        public const String NULL_PERMISSIONS = "";

        protected static PermissionType[] perms;

        static DefaultPermissionChecker()
        {
            var nullType = new PermissionType(0, NULL_PERMISSIONS, Cres.get().getString("secNoPerms"));
            perms = new[] {nullType};
        }

        public static Permissions getNullPermissions()
        {
            return new Permissions(NULL_PERMISSIONS);
        }

        public Boolean have(Permissions has, Permissions need)
        {
            try
            {
                if (has == null)
                {
                    Logger.getLogger(Log.SECURITY).debug("Permission level is 'null' and allow nothing, need " + need);
                    return false;
                }

                if (need == null || need.list().Count == 0)
                {
                    return true;
                }

                needLabel:
                foreach (var needp in need.list())
                {
                    foreach (var hasp in has.list())
                    {
                        if (!entityMatches(hasp, needp)) continue;
                        if (haveType(hasp.getType(), needp.getType()))
                        {
                            goto needLabel;
                        }
                        Logger.getLogger(Log.SECURITY).debug("Permissions '" + has + "' doesn't allow '" + need +
                                                             "' (because '" + hasp + "' doesn't allow '" + needp + "')");
                        return false;
                    }

                    Logger.getLogger(Log.SECURITY).debug("Permissions '" + has + "' does not allow '" + need +
                                                         "': no entity matches for " + needp);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.SECURITY).error("Error checking permissions: ", ex);
                return false;
            }
        }

        public String getType(Permissions permissions, String entity)
        {
            try
            {
                Logger.getLogger(Log.SECURITY).debug("Getting permission type of '" + permissions + "' in '" + entity +
                                                     "'");

                String type = null;

                if (permissions == null)
                {
                    throw new InvalidOperationException("Permissions are null");
                }

                foreach (var perm in permissions)
                {
                    if (perm.getEntity() != null && !ContextUtils.matchesToMask(perm.getEntity(), entity, true, false))
                        continue;
                    type = perm.getType();
                    break;
                }

                if (type == null)
                {
                    throw new InvalidOperationException("No matching entities");
                }

                Logger.getLogger(Log.SECURITY).debug("Permissions type of '" + permissions + "' in '" + entity + "' is " +
                                                     type);
                return type;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error getting permission type of '" + permissions + "' in '" + entity + "': ", ex);
            }
        }

        public Boolean canSee(Permissions permissions, String entity)
        {
            try
            {
                if (permissions == null)
                {
                    return false;
                }

                foreach (var perm in permissions)
                {
                    if (perm.getType().Equals(NULL_PERMISSIONS))
                    {
                        continue;
                    }

                    if (perm.getEntity() == null)
                    {
                        return true;
                    }

                    if (ContextUtils.matchesToMask(perm.getEntity(), entity, false, false))
                    {
                        return false;
                    }

                    if (ContextUtils.matchesToMask(perm.getEntity(), entity, false, true))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.SECURITY).error("Error checking permissions: ", ex);
                return false;
            }
        }

        private static Boolean entityMatches(Permission has, Permission need)
        {
            if (has.getEntity() == null)
            {
                return true;
            }

            return need.getEntity() != null
                       ? ContextUtils.matchesToMask(has.getEntity(), need.getEntity(), true, false)
                       : false;
        }

        private static Boolean haveType(String has, String need)
        {
            var hasPerm = find(has);
            var needPerm = find(need);

            return (needPerm & hasPerm) == needPerm;
        }

        private static int find(String perm)
        {
            for (var i = 0; i < perms.Length; i++)
            {
                if (perms[i].getName().Equals(perm))
                {
                    return perms[i].getPattern();
                }
            }
            throw
                new SecurityException("Permission type '" + perm + "' not found");
        }

        public Boolean isValid(String permType)
        {
            try
            {
                for (var i = 0; i < perms.Length; i++)
                {
                    if (perms[i].getName().Equals(permType))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Dictionary<Object, String> getPermissions()
        {
            var pm = new Dictionary<Object, String>();

            for (var i = 0; i < perms.Length; i++)
            {
                pm.Add(perms[i].getName(), perms[i].getDescription());
            }

            return pm;
        }

        public String canActivate(Permissions has, Permissions need)
        {
            foreach (var needp in need)
            {
                foreach (var hasp in has)
                {
                    if (!ContextUtils.matchesToMask(hasp.getEntity(), needp.getEntity(), true, false)) continue;
                    if (haveType(hasp.getType(), needp.getType())) continue;
                    if (!haveType(getType(has, needp.getEntity()), needp.getType()))
                    {
                        return "Cannot set permissions for '" + needp.getEntity() + "' to '" + needp.getType() +
                               "' because your own permission level for '" + hasp.getEntity() + "' is '" +
                               hasp.getType() + "'";
                    }
                }
            }

            return null;
        }
    }
}