using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace BiermanTech.CriticalDog
{
    public static class Extensions
    {
        public static IActionResult SetModelStateErrorMessage(this PageModel pageModel)
        {
            var firstError = pageModel.ModelState
                .Where(m => m.Value.Errors.Any())
                .Select(m => new { Property = m.Key, Error = m.Value.Errors.FirstOrDefault()?.ErrorMessage })
                .FirstOrDefault();

            if (firstError != null)
            {
                pageModel.TempData[Constants.AlertWarning] = $"Validation failed for {firstError.Property}: {firstError.Error}";
            }
            else
            {
                pageModel.TempData[Constants.AlertWarning] = "Please correct the errors in the form.";
            }

            return pageModel.Page();
        }

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
