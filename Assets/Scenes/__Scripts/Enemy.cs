using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 0.1f;

    [Header("Set Dynamically")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDistruction = false;

    protected BoundsCheck bndCheck;
    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        } 
    }
    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }
    void Update()
    {       
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            print("suceess");
            UnShowDamage();
        }
        if (bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
            //if (pos.y < bndCheck.camHeigth - bndCheck.radius)
            //{
            // Destroy(gameObject);
            //}
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    /*void OnCollisionEnter(Collision coll)
    {
        GameObject otherGo = coll.gameObject;
        if(otherGo.tag == "ProjectileHero")
        {
            Destroy(otherGo);
            Destroy(gameObject);
        } else
        {
           // print("loh!!");
        }
    }*/
    void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        switch(otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>(); 
                if(!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                ShowDamage();
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;               
                if (health  <=0)
                {
                    if(!notifiedOfDistruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDistruction = true;
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                
                break;
            default:
                print("Enemy hited by unknown object: " + otherGO);
                break;
        }
    }
    void ShowDamage()
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
        Invoke("UnShowDamage", showDamageDuration);//костыль
    }
    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
