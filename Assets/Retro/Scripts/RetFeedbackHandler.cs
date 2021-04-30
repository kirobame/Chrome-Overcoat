using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetFeedbackHandler : MonoBehaviour
{
    [SerializeField] GameObject bloodFX;
    [SerializeField] GameObject explosionFX;
    [SerializeField] GameObject spawnFX;

    // damagePosition is the position of the object responsable for player death
    // exemple : bullet, knife, ennemy, etc...
    public void BloodFX(Vector3 characterPosition, Vector3 damagePosition, Transform parent)
    {
        // bloodFX is Z forward

        Vector3 direction = characterPosition - damagePosition;
        Vector3 lookAtTarget = Vector3Mul(characterPosition, direction.normalized);

        Instantiate(bloodFX, characterPosition, Quaternion.identity, parent);
        bloodFX.transform.LookAt(lookAtTarget, Vector3.up);
    }

    public void ExplosionFX(Vector3 characterPosition,  Transform parent)
    {
        Instantiate(explosionFX, characterPosition, Quaternion.identity, parent);
    }

    public void SpawnFX(Vector3 characterPosition, Transform parent)
    {
        Instantiate(spawnFX, characterPosition, Quaternion.identity, parent);
    }

    Vector3 Vector3Mul(Vector3 a, Vector3 b)
    {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;
        return a;
    }

}
