using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public void Damage(GameObject bloodParticle, GameObject bulletHole)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            //If we shoot in zombie
            if (hitInfo.collider.TryGetComponent(out EnemyController enemyController))
            {
                //Subtract zombie health and spawn particle of blood splash if zombie is alive
                if (enemyController.zombieHealthBar > EnemyController.zombieDamage)
                {
                    enemyController.onZombieDamaged?.Invoke();

                    Instantiate(bloodParticle, hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));
                }

                //If zombie is killed, calls the event with death mechanic
                else
                {
                    enemyController.onZombieKilled?.Invoke(hitInfo.collider.gameObject);

                    Instantiate(bloodParticle, hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));
                }
                    
            }

            //If we shot in hitable object, we spawn a bullet trace (hole)
            else if (hitInfo.collider.CompareTag("Hitable"))
                Instantiate(bulletHole, hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));
        }
    }

    public void Clip(ref int loadedBullets, ref int unloadedBullets, int totalBullets, int clipBulletsAmount)
    {
        int spentBullets;

        if (totalBullets > clipBulletsAmount)
        {
            spentBullets = clipBulletsAmount - loadedBullets;

            loadedBullets = clipBulletsAmount;
            unloadedBullets -= spentBullets;
        }

        else
        {
            loadedBullets += unloadedBullets;
            unloadedBullets = 0;
        }
    }

    public void CameraShaking(ref float recoilTime, float cameraRecoilForce, Quaternion initialRotation)
    {
        if (recoilTime > 0)
        {
            initialRotation = Camera.main.transform.localRotation;
            Camera.main.transform.localRotation = Quaternion.Lerp(initialRotation,
            Quaternion.Euler(initialRotation.eulerAngles + new Vector3(cameraRecoilForce, 0f, 0f)), 0.5f);
            recoilTime -= Time.deltaTime;
        }

        else
            Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation,
            initialRotation, 0.5f);
    }

    public void Misfire(int loadedBullets, AudioSource audioSource, AudioClip misfireClip)
    {
        if (Input.GetMouseButtonDown(0) && loadedBullets == 0)
            audioSource.PlayOneShot(misfireClip);
    }

    public void AmmoTextDisplaying(int loadedBullets, int unloadedBullets, Text loadedText, Text unloadedText)
    {
        loadedText.text = loadedBullets.ToString();
        unloadedText.text = unloadedBullets.ToString();
    }
}
