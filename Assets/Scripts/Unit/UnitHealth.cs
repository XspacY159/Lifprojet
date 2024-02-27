using System.Collections;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] private UnitGeneral unit;
    [SerializeField] private ParticleSystem damagedPs;
    [SerializeField] private MeshRenderer unitRenderer;
    [SerializeField] private Material damagedMaterial;
    private Material unitMaterial;

    [SerializeField] private int health;
    [SerializeField] private float iFrames = 0.1f;

    private bool invincible;

    private void OnEnable()
    {
        health = 1;
        unitMaterial = unitRenderer.material;
        StartCoroutine(OnEnableDelay());
    }

    private IEnumerator OnEnableDelay()
    {
        yield return new WaitUntil(() => UnitSelectionController.Instance != null);
        health = unit.GetStats().maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (invincible) return;

        health -= damage;
        damagedPs.Play();

        if (health > 0)
            StartCoroutine(InvincibleFrames());
    }

    private void Update()
    {
        if (health <= 0)
            Death();
    }

    public void Regenerate(int healAmount)
    {
        health += healAmount;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    private IEnumerator InvincibleFrames()
    {
        unitRenderer.material = damagedMaterial;
        invincible = true;
        yield return new WaitForSeconds(iFrames);
        invincible = false;
        unitRenderer.material = unitMaterial;
    }
}
