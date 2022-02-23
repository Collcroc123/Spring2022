using System.Collections;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public SpellData spell;
    public SpriteRenderer spellSprite;
    public bool isCasting, isEnemy;
    public bool canCast;
    //public ArrayData gunArray;

    void Start()
    {
        SetGun(spell);
    }

    public void SetGun(SpellData newSpell) //If crate touched
    {
        spell = newSpell;
        if (isEnemy)
        {
            //gun = gunArray.guns[Random.Range(0, 5)];
        }
        //gunSprite.material.color = spell.gunColor;
    }

    public void Shoot()
    {
        if (canCast) StartCoroutine(ShootCoro());
    }

    IEnumerator ShootCoro()
    {
        isCasting = true;
        if (isEnemy) yield return new WaitForSeconds(1f);
        Spellcast cast = Instantiate(spell.cast, gameObject.transform.position, gameObject.transform.rotation).GetComponent<Spellcast>();
        cast.spell = spell;
        //cast.castSpawn = gameObject;
        if (isEnemy) cast.tag = "EnemySpell";
        yield return new WaitForSeconds(spell.rate);
        isCasting = false;
    }
}