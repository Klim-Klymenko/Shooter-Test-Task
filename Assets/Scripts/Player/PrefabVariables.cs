using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefabVariables : MonoBehaviour
{
    //variables for EnemyController access
    public Slider _healthSlider;
    public TextMeshProUGUI _healthText;
    public TextMeshProUGUI _scoreText;
    public GameObject _dropdownMenu;

    public Transform _player;

    public Animator _bloodEffectAnim;

    public AudioClip _gameOverClip;
    public AudioClip _zombieDeathClip;
    public AudioClip _playerDamageClip;
    public AudioClip _zombieDamageClip;

    //If we restart the game, we need to continue setting the scale on 1 unit
    private void Start()
    {
        Time.timeScale = 1.0f;
        FindObjectOfType<FirstPersonController>().gameObject.GetComponent<FirstPersonController>().enabled = true;
    }

}
