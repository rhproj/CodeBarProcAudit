using System;
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
