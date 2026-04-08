

namespace ETicaretAPI.Common.Domain.Entities;

public class AuditLog : Entity<int>
{
    /// <summary>
    /// Type of action performed (Insert, Update, Delete)
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Entity name (e.g., "User", "Product", "Order")
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Entity's primary key value
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Old values before change (JSON)
    /// Null for Insert operations
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// New values after change (JSON)
    /// Null for Delete operations
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// List of changed property names (JSON array)
    /// </summary>
    public string? ChangedProperties { get; set; }

    /// <summary>
    /// User who performed the action (from ILogContextProvider)
    /// </summary>
    public int? PerformedByUserId { get; set; }

    /// <summary>
    /// Username of the performer (denormalized for quick access)
    /// </summary>
    public string? PerformedByUserName { get; set; }

    /// <summary>
    /// IP address of the request
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent from HTTP headers
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Platform (Web, Mobile, Admin)
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Additional context data (JSON)
    /// </summary>
    public string? AdditionalInfo { get; set; }
}
