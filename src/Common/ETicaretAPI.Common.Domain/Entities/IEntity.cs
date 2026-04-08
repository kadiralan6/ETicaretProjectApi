namespace ETicaretAPI.Common.Domain.Entities;

public interface IEntity<TPriamryKey>
{
    TPriamryKey Id { get; set; }

    bool IsDeleted { get; set; }

    DateTime CreatedAt { get; set; }

    int? CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }

    int? ModifiedBy { get; set; }

    DateTime? DeletedAt { get; set; }
    int? DeletedBy { get; set; }
}
