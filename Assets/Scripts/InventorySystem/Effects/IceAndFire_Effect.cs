using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "IceAndFire Effect", menuName = "Data/Item Effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;
    public override void ExecuteEffect(Transform _respawnPosition)
    {
        base.ExecuteEffect(_respawnPosition);
        Transform player = PlayerManager.instance.player.transform;
        bool thirdAttack = player.GetComponent<Player>().primaryAttack.comboCounter == 2;
        if(thirdAttack)
        {
            GameObject iceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position, player.rotation);
            iceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity*player.GetComponent<Player>().facingDirection, 0);
            Destroy(iceAndFire, 10);
        }
        //Destroy(iceAndFire, 1f);
    }
}
