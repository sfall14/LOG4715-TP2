using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
{

    public bool isDamaged;
    //Mana Variables
    public Slider ManaBarSlider;
    public Text outOfManaText;
    private bool isOutOFMana = false;
    private bool isRegenMana = false;
    
    //Health Variables
    public Slider HealthBarSlider;
    public Text outOfHealthText;
    private bool isRegenHealth = false;
    public float Healthregen = .010f;

    public GameObject gameOverPanel;
    public Text gameOverText;

    public bool stompAbility = false;

    // Déclaration des constantes
    private Vector3 offset;

    // Déclaration des variables
    bool _Grounded { get; set; }
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        isDamaged = false;
        gameOverPanel.SetActive(false);
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        if (!isOutOFMana)
            transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 10f, 0, 0);
        _Grounded = false;
        offset = transform.position - _Rb.transform.position; 
    }


    void Update()
    {
        if (HealthBarSlider.value <= 0)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = " Game Over";
            Destroy(_Anim);
            Destroy(_Rb);
            Application.Quit();
        }
        else if (_Rb != null)
        { 
            if (isDamaged)
            {
                isDamaged = false;
                Damage();
            }
        
            if (ManaBarSlider.value < 1f && !isRegenMana)
            {
                StartCoroutine(RegainManaOverTime());
            }
            if (HealthBarSlider.value < 1f && !isRegenHealth)
            {
                StartCoroutine(RegainHealthOverTime()); 
            }  
            transform.position = _Rb.transform.position + offset;
            var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
            HorizontalMove(horizontal);
            CheckJump();
        }
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, horizontal);
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(horizontal));
    }

    

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (ManaBarSlider.value > 0)
                {
                    ManaBarSlider.value -= .011f;  //reduce mana
                    _Rb.AddForce(new Vector3(0, -JumpForce, 0), ForceMode.Impulse);
                    stompAbility = true;
                }
                else
                {
                    isOutOFMana = true;    
                    outOfManaText.enabled = true; 
                }
            }
            
        }

    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll) 
    {        
        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
            _Anim.SetBool("Grounded", _Grounded);
        }
    }

    //Regenartion du Mana
    public void Manaregen()
    {
        ManaBarSlider.value += .010f;
    }


    private IEnumerator RegainManaOverTime()
    {
        isRegenMana = true;
        isOutOFMana = false;
        while (ManaBarSlider.value < 1)
        {
            Manaregen();
            yield return new WaitForSeconds(3);
        }
        isRegenMana = false;
    }

    //Regenartion de la vie
    public void HealthRegen()
    {
        HealthBarSlider.value += Healthregen;
    }


    private IEnumerator RegainHealthOverTime()
    {
        isRegenHealth = true;
        while (HealthBarSlider.value < 1)
        {
            HealthRegen();
            yield return new WaitForSeconds(5);
        }
        isRegenHealth = false;
    }

    public void Damage()
    {
        HealthBarSlider.value -= .050f;
    }
}
