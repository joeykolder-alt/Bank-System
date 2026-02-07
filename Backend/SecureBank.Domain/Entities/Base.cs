namespace SecureBank.Domain.Entities;

/// <summary>
/// Base entity with common properties for all entities
/// </summary>
public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Soft delete entity - extends BaseEntity with soft delete support
/// </summary>
public class SoftDeleteEntity : BaseEntity
{
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
