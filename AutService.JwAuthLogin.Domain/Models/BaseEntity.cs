namespace AutService.JwAuthLogin.Domain.Models
{
    public abstract class BaseEntity<TKey>
    {
        public virtual TKey Id { get; set; }
    }
}
