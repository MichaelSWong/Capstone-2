namespace Capstone.Class
{
    /// <summary>
    /// A class of general helper methods.
    /// </summary>
    public static class GeneralAssistance
    {
        /// <summary>
        /// Checks if the given char is a valid integer value.
        /// </summary>
        /// <param name="num">The string to check.</param>
        /// <returns>True if it is a valid integer.</returns>
        public static bool CheckIfIntNumber(char num)
        {
            if (num == '-')
            {
                return false;
            }

            return CheckIfIntNumber(num.ToString());
        }
        /// <summary>
        /// Checks if the given string is a valid integer value.
        /// </summary>
        /// <param name="num">The string to check.</param>
        /// <returns>True if it is a valid integer.</returns>
        public static bool CheckIfIntNumber(string num)
        {
            if (num == null || num == "")
            {
                return false;
            }

            foreach (char digit in num)
            {
                if (digit == '-' && num.IndexOf(digit) != 0)
                {
                    return false;
                }
                else if (digit == '-' && num.Length == 1)
                {
                    return false;
                }
                else if (digit != '-' && digit != '0' && digit != '1' && digit != '2' && digit != '3' && digit != '4' && digit != '5' && digit != '6' && digit != '7' && digit != '8' && digit != '9')
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Checks if the given string is a valid floating point value.
        /// </summary>
        /// <param name="num">The string to check.</param>
        /// <returns>True if it is a valid floating point value.</returns>
        public static bool CheckIfFloatingPointNumber(string num)
        {
            if (num == null || num == "")
            {
                return false;
            }

            bool hasADecimal = false;
            foreach (char digit in num)
            {
                if (digit == '.' && num.Length == 1)
                {
                    return false;
                }
                else if (digit == '.' && !hasADecimal)
                {
                    hasADecimal = true;
                }
                else if (digit == '.')
                {
                    return false;
                }
                else if (digit == '-' && num.IndexOf(digit) != 0)
                {
                    return false;
                }
                else if (digit == '-' && num.Length == 1)
                {
                    return false;
                }
                else if (digit != '-' && digit != '0' && digit != '1' && digit != '2' && digit != '3' && digit != '4' && digit != '5' && digit != '6' && digit != '7' && digit != '8' && digit != '9')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
