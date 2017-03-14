using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
    public sealed class ServiceRequestContext
    {
        private static readonly string ContextKey = Guid.NewGuid().ToString();

        public ServiceRequestContext(CustomContextDataDto contextDto)
        {
            this.ContextDto = contextDto;
        }

        public CustomContextDataDto ContextDto { get; private set; }

        public static ServiceRequestContext Current
        {
            get { return (ServiceRequestContext)CallContext.LogicalGetData(ContextKey); }
            internal set
            {
                if (value == null)
                {
                    CallContext.FreeNamedDataSlot(ContextKey);
                }
                else
                {
                    CallContext.LogicalSetData(ContextKey, value);
                }
            }
        }

        public static Task RunInRequestContext(Func<Task> action, CustomContextDataDto contextDto)
        {
            Task<Task> task = null;
            task = new Task<Task>(async () =>
            {
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(contextDto);
                try
                {
                    await action();
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                }
            });
            task.Start();
            return task.Unwrap();
        }

        public static Task<TResult> RunInRequestContext<TResult>(Func<Task<TResult>> action, CustomContextDataDto contextDto)
        {
            Task<Task<TResult>> task = null;
            task = new Task<Task<TResult>>(async () =>
            {
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(contextDto);
                try
                {
                    return await action();
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                }
            });
            task.Start();
            return task.Unwrap<TResult>();
        }
    }
}
