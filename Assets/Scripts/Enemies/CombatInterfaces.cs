using UnityEngine;

// ITF objetos que reciben daño (Mascara Roja / Azul)
public interface IDamageable
{
    void TakeDamage(int amount, string sourceName);
}

// ITF objetos que pueden ser empujados (Mascara Verde)
public interface IPushable
{
    // direction: vector hacia donde va el empuje
    // force: potencia del viento
    void Push(Vector2 direction, float force);
}