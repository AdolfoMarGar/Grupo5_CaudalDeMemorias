using UnityEngine;

public enum AljibeZoneType
{
    CleanCistern,
    TrashChute
}

public class AljibeZoneTrigger : MonoBehaviour
{
    [SerializeField] private AljibeZoneType zoneType = AljibeZoneType.CleanCistern;
    [SerializeField] private AljibeGameController gameController;

    public AljibeZoneType ZoneType
    {
        get => zoneType;
        set => zoneType = value;
    }

    public AljibeGameController GameController
    {
        get => gameController;
        set => gameController = value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AljibeParticle particle = other.GetComponent<AljibeParticle>();
        if (particle == null) return;

        if (gameController != null)
        {
            gameController.OnParticleEnteredZone(particle, zoneType);
        }

        Destroy(particle.gameObject);
    }
}
