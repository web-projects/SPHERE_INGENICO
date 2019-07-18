using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.DAL.Helpers
{
    public class StatusCode
    {
        public static string errorMessage = "Cannot get message for status code: ";
        public static List<Data.Entity.StatusCode> StatusCodes = new List<Data.Entity.StatusCode>();

        public static string GetDisplayMessage(Enum message)
        {
            try
            {
                var statusCode = GetStatusCode(message);
                return string.IsNullOrEmpty(statusCode?.DisplayMessage) ? message.Description() : statusCode.DisplayMessage;
            }
            catch
            {
                return $"{errorMessage} {Convert.ToInt32(message)}";
            }
        }

        private static Data.Entity.StatusCode CreateNewStatusCode(int statusCodeID, string code, string displayMessage)
        {
            return new Data.Entity.StatusCode()
            {
                StatusCodeID = statusCodeID,
                Active = true,
                Code = code,
                DisplayMessage = displayMessage
            };
        }

        public static Data.Entity.StatusCode GetStatusCode(Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            var item = Enum.Parse(enumType, enumValue.ToString());
            var code = new Data.Entity.StatusCode();

            if (StatusCodes == null || StatusCodes.Count <= 0)
            {
                code = CreateNewStatusCode(Convert.ToInt32(enumValue), enumValue.ToString(), enumValue.Description());
            }
            else
            {
                code = StatusCodes.Where(c => c.StatusCodeID == Convert.ToInt32(enumValue)).FirstOrDefault();
            }
            
            return code;
        }

    }
}
