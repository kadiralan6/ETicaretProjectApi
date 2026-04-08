
namespace ETicaretAPI.Common.Domain.Entities
{

    [Serializable]
    public class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {

        public TPrimaryKey Id { get; set; }


        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public int? DeletedBy { get; set; }


        public Entity()
        {

            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
        }

        public Entity(TPrimaryKey id)
        {
            Id = id;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
        }
    }
}