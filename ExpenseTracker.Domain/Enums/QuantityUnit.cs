namespace ExpenseTracker.Domain.Enums
{
    /// <summary>
    /// What an expense's quantity counts. Item means whole units; the rest are measured amounts
    /// (e.g. 0.612 Kg at £5.00 per kg).
    /// </summary>
    public enum QuantityUnit
    {
        Item,
        Kg,
        G,
        Lb,
        L
    }
}
