//==================================================================================================
// Configuration for marketing campaigns with date ranges and pricing.
// Demonstrates generic validation with decimal, DateTime, and TimeSpan types.
//==================================================================================================

namespace Fox.ConfigKit.Samples.WebApi.Configuration;

public sealed class CampaignConfig
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MinimumPurchaseAmount { get; set; }
    public decimal MaximumDiscountPercentage { get; set; }
    public TimeSpan EmailReminderInterval { get; set; }
    public TimeSpan CacheDuration { get; set; }
}
