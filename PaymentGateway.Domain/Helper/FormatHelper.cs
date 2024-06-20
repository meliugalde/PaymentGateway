using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Helper
{
    public static class FormatHelper
    {
        public static string GetLastFourDigits(String cardNumber)
        {
            int startIndex = cardNumber.Length - 4;
            return cardNumber.Substring(startIndex, 4);
        }

        public static string FormatDate(string month, string year)
        {
            if (string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
            {
                throw new ArgumentException("Month and year must be provided.");
            }

            if (!int.TryParse(month, out int monthInt) || monthInt < 1 || monthInt > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be an integer between 1 and 12.");
            }

            if (!int.TryParse(year, out int yearInt) || yearInt < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Year must be a valid positive integer.");
            }

            // Format month to two digits
            string formattedMonth = monthInt.ToString("D2");

            return $"{formattedMonth}/{year}";
        }

        public static bool ValidateMonth(string month, out int expireMonthInt)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(month))
            {
                isValid = false;
            }

            if (!int.TryParse(month, out expireMonthInt) || expireMonthInt < 1 || expireMonthInt > 12)
            {
                isValid = false;
            }

            return isValid;
        }

        public static bool ValidateYear(string year, out int expireYearhInt)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(year) || year.Length != 4)
            {
                isValid = false;
            }

            if (!int.TryParse(year, out expireYearhInt))
            {
                isValid = false;
            }

            return isValid;
        }

        public static bool ValidateExpiryDate(int expiryMonth, int expiryYear)
        {
            var expiryDate = new DateTime(expiryYear, expiryMonth, DateTime.DaysInMonth(expiryYear, expiryMonth));
            return expiryDate >= DateTime.Now;
        }
    }
}
