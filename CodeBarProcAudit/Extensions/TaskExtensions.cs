using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBarProcAudit.Extensions
{
    internal static class TaskExtensions
    {
        public async static void Await(this Task tsk, Action<Exception> errorCB)
        {
            try
            {
                await tsk;
            }
            catch (Exception ex)
            {
                errorCB?.Invoke(ex);
            }
        }
    }
}
