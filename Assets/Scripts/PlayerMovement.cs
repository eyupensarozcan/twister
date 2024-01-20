using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    public float moveSpeed;
    private bool onWater = false;
    public GameObject miniMapCamera;
    public TextMeshProUGUI warningText;
    private float waterTimer = 10;
    public GameObject sceneTransition;
    private GameObject selectedRuin;
    public GameObject levelController;
    private float speedMultiplier = 1;
    private Vector3 checkpoint = new Vector3(100,27.55f,-0);
    private int deathCount;
    private bool isSpawned = false;
    public TextMeshProUGUI livesText;
    public GameObject checkpointText;
    public GameObject deadScene;
    public GameObject finishScene;
    public GameObject quest;
    public GameObject respawnEffectPrefab;
    
    //Combat
    private bool _isOnDash;
    private bool _canDash;
    public float health;
    public float hitTimer;
    public float canHitTimer;
    public GameObject canDashIndicator;
    public TextMeshProUGUI healthText;
    public float dashSpeed;
    private float _defaultSpeedMultiplier;
    public GameObject dashVfx;
    public Vector3 maxVelocity = new Vector3(6, 6, 6);
    private float _maxVelocity = 6;
    private GameObject _explosionVfx;
    public GameObject explosionVfxPrefab;

    void Start()
    {
        rigidbody = GetComponentInChildren<Rigidbody>();
        _canDash = true;
        _defaultSpeedMultiplier = speedMultiplier;
    }

    private void Update()
    {
        if(miniMapCamera != null) miniMapCamera.transform.position = new Vector3(transform.position.x, 150, transform.position.z);

        if (onWater && waterTimer >= 0)
        {
            waterTimer -= Time.deltaTime;
            if(warningText != null) warningText.text = "Get out of water before " + (int)waterTimer + "!";
        }

        if (!onWater)
        {
            waterTimer = 10;
        }

        if (onWater && waterTimer < 0)
        {
            waterTimer = -0.25f;
            StartCoroutine(LoadAgain());
        }

        if (selectedRuin != null && Input.GetKeyDown(KeyCode.E))
        {
            selectedRuin.transform.GetChild(0).GetComponent<ParticleSystem>().loop = false;
            selectedRuin.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().loop = false;
            selectedRuin.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().loop = false;
            selectedRuin.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>().loop = false;
            selectedRuin.transform.GetChild(1).gameObject.SetActive(true);
            selectedRuin.transform.GetChild(2).gameObject.SetActive(true);
            selectedRuin.transform.tag = null;
            selectedRuin = null;
            levelController.GetComponent<LevelController>().cleanedRuinCount =
                levelController.GetComponent<LevelController>().cleanedRuinCount + 1;
        }

        dashVfx.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            isSpawned = false;
        }

        if (isSpawned)
        {
            rigidbody.velocity = Vector3.zero;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rigidbody.AddForce(movement * moveSpeed * speedMultiplier);
        
        //Dash to attack
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            _canDash = false;
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 6);
            _isOnDash = true;
            canDashIndicator.SetActive(false);
            speedMultiplier = dashSpeed;
            dashVfx.GetComponent<ParticleSystem>().Play();
            StartCoroutine(Attack());
        }

        if(_canDash)
        {
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 4);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            onWater = true;
            if(warningText != null) warningText.gameObject.SetActive(true);
        }
        
        if (other.CompareTag("Ruin"))
        {
            selectedRuin = other.gameObject;
        }
        
        if (other.CompareTag("Boost"))
        {
            speedMultiplier = 4;
        }
        
        if (other.CompareTag("Boost1"))
        {
            speedMultiplier = 6;
        }
        
        if (other.CompareTag("Checkpoint"))
        {
            checkpoint = transform.position;
            Destroy(other.GameObject());
            StartCoroutine(Checkpoint());
        }
        
        if (other.CompareTag("Dead"))
        {
            if(deathCount < 3)
            {
                deathCount = deathCount + 1;
                livesText.text = "Remaining Lives: " + (3 - deathCount).ToString() + "/3";
                rigidbody.velocity = Vector3.zero;
                transform.position = checkpoint;
                isSpawned = true;
                GameObject respawnEffect;
                respawnEffect = Instantiate(respawnEffectPrefab, checkpoint, Quaternion.identity);
            }
            else
            {
                speedMultiplier = 0;
                quest.SetActive(false);
                deadScene.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
        
        if (other.CompareTag("Finish"))
        {
            speedMultiplier = 0;
            quest.SetActive(false);
            finishScene.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        if (other.CompareTag("Health"))
        {
            if(health < 100)
            {
                health += 40;
                if (health > 100)
                {
                    health = 100;
                }
                healthText.text = "Health: " + health.ToString();
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            switch (_isOnDash)
            {
                case false:
                    StartCoroutine(GetDamage());
                    rigidbody.AddExplosionForce(1000, other.transform.position, 4);
                    _explosionVfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
                    _explosionVfx.transform.localScale = _explosionVfx.transform.localScale / 2;
                    break;
                case true:
                    _explosionVfx = Instantiate(explosionVfxPrefab, other.transform.position, Quaternion.identity);
                    Destroy(other.gameObject);
                    break;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            onWater = false;
            if(warningText != null) warningText.gameObject.SetActive(false);
        }
        
        if (other.CompareTag("Ruin") && selectedRuin != null)
        {
            selectedRuin = null;
        }
        
        if (other.CompareTag("Boost") || other.CompareTag("Boost1"))
        {
            speedMultiplier = 1;
        }
    }

    public IEnumerator GetDamage()
    {
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 6);
        health -= 20;
        healthText.text = "Health: " + health.ToString();

        if (health <= 0)
        {
            StartCoroutine(LoadAgain());
        }

        yield return new WaitForSeconds(0.5f);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 4);
    }

    public IEnumerator Attack()
    {
        yield return new WaitForSeconds(hitTimer);
        _isOnDash = false;
        speedMultiplier = _defaultSpeedMultiplier;
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 4);
        yield return new WaitForSeconds(canHitTimer);
        _canDash = true;
        canDashIndicator.SetActive(true);
    }

    public IEnumerator LoadAgain()
    {
        sceneTransition.GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(1);
    }

    public IEnumerator Checkpoint()
    {
        checkpointText.SetActive(true);
        yield return new WaitForSeconds(2);
        checkpointText.SetActive(false);
    }
}
