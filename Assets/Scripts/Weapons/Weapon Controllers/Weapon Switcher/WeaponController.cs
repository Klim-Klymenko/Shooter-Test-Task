using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject rifleUI;
    [SerializeField] private GameObject pistolUI;

    [SerializeField] private GameObject assaultRifle;
    [SerializeField] private GameObject pistol;

    private PistolController pistolController;
    private RifleController rifleController;

    public static int weaponIndex;

    private void Start()
    {
        pistolController = pistol.GetComponent<PistolController>();
        rifleController = assaultRifle.GetComponent<RifleController>();

        rifleUI.SetActive(true);
        pistolUI.SetActive(false);

        assaultRifle.SetActive(true);
        pistol.SetActive(false);

        weaponIndex = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !pistolController.isShooting && !pistolController.isReloading)
        {
            assaultRifle.SetActive(true);
            pistol.SetActive(false);

            rifleUI.SetActive(true);
            pistolUI.SetActive(false);

            weaponIndex = 1;
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2) && !rifleController.isShooting && !rifleController.isReloading)
        {
            pistol.SetActive(true);
            assaultRifle.SetActive(false);

            pistolUI.SetActive(true);
            rifleUI.SetActive(false);

            weaponIndex = 2;
        }
    }
}
