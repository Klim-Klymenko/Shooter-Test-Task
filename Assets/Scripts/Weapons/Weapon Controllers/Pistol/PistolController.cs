using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PistolController : Weapon
{
    [SerializeField] private Text loadedBulletsText;
    [SerializeField] private Text unloadedBulletsText;

    [SerializeField] private GameObject bulletHole;
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject assaultRifle;
    [SerializeField] private GameObject clip;

    [SerializeField] private AudioSource shootingAudioSource;
    [SerializeField] private AudioClip shootingClip;
    [SerializeField] private AudioClip misfireClip;
    [SerializeField] private AudioClip reloadClip;

    private Animator pistolAnimator;

    private Quaternion initialRotation;

    private Vector3 initialPosition;

    private int muzzleFlashLifeTime = 150;
    private int reloadTime = 1100;
    private int recoilMillisecondsTime = 500;

    private int loadedBullets = 7;
    private int clipBulletsAmount = 7;
    private int unloadedBullets = 30;
    private int totalBullets = 37;

    private float shootingTime = 1.1f;
    private float shootingState = 1.1f;

    private float recoilTime = 0.45f;
    private float fireHalfRate = 0.45f;
    private float cameraRecoilForce = -10f;

    private float halfReloadingTime = 0.55f;
    private float halfReloadingTimeValue = 0.55f;

    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool isReloading;

    private void Start()
    {
        pistolAnimator = GetComponent<Animator>();

        initialPosition = clip.transform.localPosition;

        //display bullets amount
        AmmoTextDisplaying(loadedBullets, unloadedBullets, loadedBulletsText, unloadedBulletsText);
    }

    private void Update()
    {
        //if we do shoot, we call a recoil methods
        if (isShooting && !isReloading)
        {
            CameraShaking(ref recoilTime, cameraRecoilForce, initialRotation);
            PistolRecoil();
        }

        Shooting();

        Reloading();

        Misfire(loadedBullets, shootingAudioSource, misfireClip);


        //if we reload, we start animation of reload
        if (!isShooting && isReloading)
        {
            ReloadingAnim();
        }
    }

    private async void Shooting()
    {
        //if we don't reload, we start shooting
        if (Input.GetMouseButtonDown(0) && shootingState >= shootingTime && !isReloading && loadedBullets > 0)
        {
            isShooting = true;

            //Damage method which damages zombie or leaving the bullet trace
            Damage(bloodParticle, bulletHole);

            //Turn on reload animation
            PistolRecoil();

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

    private async void Reloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isShooting && loadedBullets < clipBulletsAmount && unloadedBullets > 0)
        {
            //counting bullets to substitute
            Clip(ref loadedBullets, ref unloadedBullets, totalBullets, clipBulletsAmount);
            isReloading = true;

            //producing reload sound
            shootingAudioSource.PlayOneShot(reloadClip);

            //make a delay
            await Task.Delay(reloadTime);

            //display new clip values
            AmmoTextDisplaying(loadedBullets, unloadedBullets, loadedBulletsText, unloadedBulletsText);
            isReloading = false;
        }
    }

    private void ReloadingAnim()
    {
        //if we didn't finish moving clip aside, we do it
        if (halfReloadingTime > 0)
        {
            clip.transform.localPosition = Vector3.Lerp(initialPosition,
            initialPosition + new Vector3(-0.02f, 0, 0), 1f);
            halfReloadingTime -= Time.deltaTime;
        }

        //if we did move clip, we start moving it back to initial position
        else
        {
            clip.transform.localPosition = Vector3.Lerp(clip.transform.localPosition,
            initialPosition, 1f);
        }
    }

    private async void PistolRecoil()
    {
        //start playing recoil animation
        pistolAnimator.enabled = true;
        pistolAnimator.Play("Pistol Recoil");

        //time of recoil animation
        await Task.Delay(recoilMillisecondsTime);

        //start playing empty animation and making delay to avoid second cycle of recoil animation
        pistolAnimator.Play("New State");

        await Task.Delay(30);

        //stop playnig animation
        pistolAnimator.enabled = false;

        isShooting = false;
    }

    private void ShootingDelay()
    {
        //renew values to start new cycle
        shootingState = shootingTime;
        recoilTime = fireHalfRate;
        halfReloadingTime = halfReloadingTimeValue;
    }
}
