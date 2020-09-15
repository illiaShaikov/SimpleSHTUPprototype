using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    Vector2 rotMinMax = new Vector2(15, 90);
    Vector2 driftMinMax = new Vector2(.25f,2);
    public float lifeTime = 6f;
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;
    // Start is called before the first frame update
    void Awake()
    {
        cube = transform.Find("Cube").gameObject;// находим обьект куб
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();
        Vector3 vel = Random.onUnitSphere;//возвращает вектор, указывающую на случайную точку, которая находиться на сфере радиусом 1м. и с центром в начале координат
        vel.z = 0;
        vel.Normalize();// устанавливает длину равную 1м
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;
        transform.rotation = Quaternion.identity;//отсутствие пово
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if(u >= 1)
        {
            Destroy(this.gameObject);
        }
        if(u > 0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
        if(!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }
    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        //letter.color = def.color;
        letter.text = def.letter;
        type = wt;
    }
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
