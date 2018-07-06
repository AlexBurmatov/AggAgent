using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Collections;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.security
{
    public class Permissions : IEnumerable<Permission>
    {
        public const char PERMISSIONS_SEPARATOR = ',';

        private readonly IList<Permission> permissions = new SynchronizedList<Permission>();

        public Permissions()
        {
        }

        public Permissions(String data, PermissionChecker checker)
        {
            if (data == null)
            {
                data = "";
            }

            var pd = StringUtils.split(data, PERMISSIONS_SEPARATOR);

            foreach (var pde in pd)
            {
                var permSrc = pde.Trim();

                if (permSrc.Length > 0)
                {
                    permissions.Add(new Permission(permSrc, checker));
                }
            }
        }

        public Permissions(String entity, String type)
        {
            permissions.Add(new Permission(entity, type));
        }

        public Permissions(String data) : this(data, (PermissionChecker) null)
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public String encode()
        {
            var enc = new StringBuilder();
            var i = 0;

            foreach (var perm in permissions)
            {
                if (i > 0)
                {
                    enc.Append(PERMISSIONS_SEPARATOR);
                }

                enc.Append(perm.encode());

                i++;
            }

            return enc.ToString();
        }

        public override String ToString()
        {
            return encode();
        }

        public IList<Permission> list()
        {
            return permissions;
        }

        public Permissions add(Permission permission)
        {
            permissions.Add(permission);
            return this;
        }

        public IEnumerator<Permission> GetEnumerator()
        {
            return permissions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}