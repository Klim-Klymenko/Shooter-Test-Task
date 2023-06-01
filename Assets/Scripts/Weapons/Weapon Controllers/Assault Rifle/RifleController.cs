using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RifleController : Weapon
{
    [SerializeField] private Text loadedBulletsText;
    [SerializeField] private Text unloadedBulletsText;

    [SerializeField] private GameObject bulletHole;
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject assaultRifle;

    [SerializeField] private AudioSource shootingAudioSource;
    [SerializeField] private AudioClip shootingClip;
    [SerializeField] private AudioClip misfireClip;
    [SerializeField] private AudioClip reloadClip;

    private Animator assaultRifleAnimator;

    private Quaternion initialRotation;

    private Vector3 initialRiflePosition;

    private int reloadingTime = 1162;
    private int muzzleFlashLifeTime = 150;
    private int loadedBullets = 30;
    private int clipBulletsAmount = 30;
    private int unloadedBullets = 120;
    private int totalBullets = 150;
    private int spentBullets;

    private float shootingTime = 0.35f;
    private float shootingState = 0.35f;

    private float recoilTime = 0.15f;
    private float fireHalfRate = 0.15f;
    private float rifleRecoilForce = -0.2f;
    private float cameraRecoilForce = -5f;

    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool isReloading;

    private void Start()
    {
        //switch animator off
        assaultRifleAnimator = assaultRifle.GetComponent<Animator>();
        assaultRifleAnimator.enabled = false;

        //remember initial rifle's position for moving in back on the condition of a recoil
        initialRiflePosition = assaultRifle.transform.localPosition;

        //display bullets amount
        AmmoTextDisplaying(loadedBullets, unloadedBullets, loadedBulletsText, unloadedBulletsText);
    }

    private void Update()
    {
        Shooting();     
        
        Reloading();

        EndShooting();

        Misfire(loadedBullets, shootingAudioSource, misfireClip);

        //if we do shoot, we call a recoil methods
        if (isShooting && !isReloading)
        {
            CameraShaking(ref recoilTime, cameraRecoilForce, initialRotation);
            AssaultRifleRecoil();
        }

        //if we stopped shooting, we need to finish playing weapon recoil
        else if (!isShooting && !isReloading)
            EndRecoil();
    }

    private async void Shooting()
    {
        //if we don't reload, we start shooting
        if (Input.GetMouseButton(0) && shootingState >= shootingTime && !isReloading && loadedBullets > 0)
        {
            isShooting = true;

            //Damage method which damages zombie or leaving the bullet trace
            Damage(bloodParticle, bulletHole);

            //Turn on muzzle falsh effect
            muzzleFlash.SetActive(true);

            //turn on shooting sound
            shootingAudioSource.PlayOneShot(shootingClip);

            //subtract loaded bullets amount and display it
            loadedBullets--;
            totalBullets = loadedBullets + unloadedBullets;
            AmmoTextDisplaying(loadedBullets, unloadedBullets, loadedBulletsText, unloadedBulletsText);

            //make delay between shoots
            shootingState = 0;
            Invoke("ShootingDelay", shootingTime);

            //turn off muzzle flash effect once it has played
            await Task.Delay(muzzleFlashLifeTime);
            muzzleFlash.SetActive(false);
        }
    }

    //making bool variable false once we stopped shooting
    private void EndShooting()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
    }

    private async void Reloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isShooting && loadedBullets < clipBulletsAmount && unloadedBullets > 0)
        {
            //counting bullets to substitute
            Clip(ref loadedBullets, ref unloadedBullets, totalBullets, clipBulletsAmount);

            //start playing reload animation
            isReloading = true;
            assaultRifleAnimator.enabled = true;

            //playing reload sound
            shootingAudioSource.PlayOneShot(reloadClip);

            //wait until reload animation finishes
            await Task.Delay(reloadingTime);

            //stop playing reload animation
            isReloading = false;
            assaultRifleAnimator.enabled = false;

            //display new reloaded values
            AmmoTextDisplaying(loadedBullets, unloadedBullets, loadedBulletsText, unloadedBulletsText);
        }
    }

    private void AssaultRifleRecoil()
    {
        //start playing recoil animation
        if (recoilTime > 0)
        {
            assaultRifle.transform.localPosition = Vector3.Lerp(initialRiflePosition,
            initialRiflePosition + new Vector3(0f, 0f, rifleRecoilForce), 0.5f);
        }

        //return assault rifle to initial position before animation
        else
            EndRecoil();    
    }

    //returns weapon to initial position stopping playing recoil animation
    private void EndRecoil()
    {
        assaultRifle.transform.localPosition = Vector3.Lerp(assaultRifle.transform.localPosition,
        initialRiflePosition, 0.5f);
    }

    //renew values to start a new cycle
    private void ShootingDelay()
    {
        shootingState = shootingTime;
        recoilTime = fireHalfRate;
    }
}
