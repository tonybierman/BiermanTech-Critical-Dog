using System.Text;

namespace BiermanTech.CriticalDog
{
    public static class Extensions
    {
        public static string GetAllExceptionMessagesWithStackTrace(this Exception ex)
        {
            var messages = new StringBuilder();
            messages.AppendLine(GetAllExceptionMessages(ex));
            messages.AppendLine(ex.StackTrace);

            return messages.ToString();
        }

        public static string GetAllExceptionMessages(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            var messages = new List<string>();
            var currentException = ex;

            while (currentException != null)
            {
                messages.Add(currentException.Message);
                currentException = currentException.InnerException;
            }

            return string.Join(" -> ", messages);
        }
    }
}
