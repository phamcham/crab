public interface IDamagable {
    public Team Team { get; }
    public void TakeDamage(int damage);
}