using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Common
{
    public class CustomServiceContext
    {
        public const string ServiceContext = "_ServiceContext_";

        static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public static void SetContext(CustomContextDataDto contextDto)
        {
            var old = CallContext.LogicalGetData(ServiceContext);
            locker.EnterWriteLock();

            try
            {
                CallContext.LogicalSetData(ServiceContext, contextDto);
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
                return CallContext.LogicalGetData(ServiceContext) as CustomContextDataDto;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }
    }
}
