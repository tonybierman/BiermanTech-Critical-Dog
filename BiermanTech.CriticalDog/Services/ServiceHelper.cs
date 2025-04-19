using BiermanTech.CriticalDog;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public static class ServiceHelper
{
    public static async Task<bool> ExecuteAsyncOperation<T>(
        Func<Task<T>> operation,
        ITempDataDictionary tempData, // Change to ITempDataDictionary
        ILogger logger,
        string successMessage = "Record saved.",
        string failureMessage = "Record not saved.",
        string rowsAffectedMessage = "{0} rows affected.")
    {
        try
        {
            var result = await operation();
            int rows = Convert.ToInt32(result);

            if (rows > 0)
            {
                tempData[Constants.AlertSuccess] = $"{successMessage} {string.Format(rowsAffectedMessage, rows)}";
                return true;
            }
            else
            {
                tempData[Constants.AlertWarning] = $"{failureMessage} {string.Format(rowsAffectedMessage, rows)}";
                return false;
            }
        }
        catch (Exception ex)
        {
            tempData[Constants.AlertDanger] = ex.GetAllExceptionMessages();
            logger.LogError(ex, ex.GetAllExceptionMessages());
            return false;
        }
    }
}