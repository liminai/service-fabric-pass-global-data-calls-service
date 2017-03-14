using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Service.Common
{
    public class CustomServiceContext
    {
        public const string ServiceContext = "_ServiceContext_";

        static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public static void SetContext(CustomContextDataDto contextDto)
        {
            var old = GetValue(ServiceContext);
            locker.EnterWriteLock();

            try
            {
                SetValue(ServiceContext, contextDto);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public static CustomContextDataDto GetContext()
        {
            locker.EnterReadLock();

            try
            {
                return GetValue(ServiceContext) as CustomContextDataDto;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        #region
        ///
        /// Gets a value indicating if the current application domain is running under ASP.NET.
        ///
        public static bool IsInHttpContext
        {
            get
            {
                return (object)HttpContext.Current != null;
            }
        }

        #endregion

        #region Methods/Operators

        private static object AspNetGetValue(string key)
        {
            return HttpContext.Current.Items[key];
        }

        private static void AspNetRemoveValue(string key)
        {
            HttpContext.Current.Items.Remove(key);
        }

        private static void AspNetSetValue(string key, object value)
        {
            HttpContext.Current.Items[key] = value;
        }

        private static object CallCtxGetValue(string key)
        {
            return CallContext.GetData(key);
        }

        private static void CallCtxRemoveValue(string key)
        {
            CallContext.FreeNamedDataSlot(key);
        }

        private static void CallCtxSetValue(string key, object value)
        {
            CallContext.SetData(key, value);
        }

        public static object GetValue(string key)
        {
            if (IsInHttpContext)
            {
                return AspNetGetValue(key);
            }
            else
            {
                return CallCtxGetValue(key);
            }
        }

        public static void RemoveValue(string key)
        {
            if (IsInHttpContext)
            {
                AspNetRemoveValue(key);
            }
            else
            {
                CallCtxRemoveValue(key);
            }
        }

        public static void SetValue(string key, object value)
        {
            if (IsInHttpContext)
            {
                AspNetSetValue(key, value);
            }
            else
            {
                CallCtxSetValue(key, value);
            }
        }

        #endregion
    }
}
