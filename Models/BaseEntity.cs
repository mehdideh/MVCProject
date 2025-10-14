namespace MVCProject.Models;

public abstract  class BaseEntity
{
    public Guid Id { get; set; }
    public bool isDeleted { get; set; }
   // public DateTime? CreatedAt { get; set; }
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        isDeleted = false;
        //CreatedAt = DateTime.UtcNow;

    }


}