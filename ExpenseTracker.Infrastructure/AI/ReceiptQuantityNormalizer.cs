using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Infrastructure.AI
{
    /// <summary>
    /// Turns the quantity and unit the AI model read off a receipt into values the Expense entity accepts.
    /// </summary>
    public static class ReceiptQuantityNormalizer
    {
        /// <summary>The unit named by the model, or Item when it is missing or unknown.</summary>
        public static QuantityUnit ParseUnit(string? raw)
        {
            return Enum.TryParse<QuantityUnit>(raw?.Trim(), ignoreCase: true, out var unit) && Enum.IsDefined(unit)
                ? unit
                : QuantityUnit.Item;
        }

        /// <summary>A whole number of at least 1 for items; above 0 with at most 3 decimal places for weights and volumes.</summary>
        public static decimal Normalize(decimal raw, QuantityUnit unit)
        {
            if (unit == QuantityUnit.Item)
                return Math.Max(1, decimal.Round(raw, MidpointRounding.AwayFromZero));

            var rounded = decimal.Round(raw, 3, MidpointRounding.AwayFromZero);
            return rounded > 0 ? rounded : 1;
        }
    }
}
