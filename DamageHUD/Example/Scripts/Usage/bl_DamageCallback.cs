using UnityEngine;

public class bl_DamageCallback : MonoBehaviour {

	[SerializeField]private float Health = 100;

    private bool isDie = false;

    /// <summary>
    /// This callback for receive damage
    /// If you have your own 'Player Damage' script, you can use this as reference
    /// for implement.
    /// </summary>
    /// <param name="info"></param>
    public void OnDamage(bl_DamageInfo info)
    {
        if (isDie)
            return;

        Health -= info.Damage;
        //If you not receive a 'bl_DamageInfo' in your own function of your script ->
        //you can create in the same function like this ->
        //--------bl_DamageInfo info = new bl_DamageInfo((float)DamageReceive); //'DamageReceive' is the damage that player receive.
        //--------info.Sender = EnemyAttaker; //send sender (this need receive in your funtion).
        //them just send.
        bl_DamageDelegate.OnDamageEvent(info);

        if (Health <= 0)
        {
            //If not more health = player die
            Die();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Die()
    {
        isDie = true;
        Health = 0;
        //When player die, send the event for all listeners.
        //with this sure to show the death hud.
        bl_DamageDelegate.OnDie();
    }
}