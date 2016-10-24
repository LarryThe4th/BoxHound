using UnityEngine;
using System.Collections;

public class bl_Attaker : MonoBehaviour {

    [HideInInspector]public GameObject Sender = null;
	[SerializeField]private Vector2 DamageRange = new Vector2(2,7);
    [SerializeField]private AudioClip BounceAudio;
    [SerializeField]private bool EvenSound = false;
    /// <summary>
    /// This example use collider as detector of hit / impact.
    /// is same process if you use raycast or other method.
    /// the important is that you send the variables.
    /// </summary>
    /// <param name="c"></param>
    void OnCollisionEnter(Collision c)
    {
       
        //If impact with the player
       if (c.transform.tag == "Player")
        {
            //Damage caused to the player
            float damage = Random.Range(DamageRange.x, DamageRange.y);

            //This is the important that you impliment in your own scripts.
            //No need use the struct 'bl_DamageInfo', just sure of send this two variables:
            //---GameObject Sender---- = the enemy that inflict the damage, 
            //in this case the turrent that shoot this ball.

            //This is the method that use for this example, you can use your own for notify the player
            //that have received damage
            bl_DamageInfo info = new bl_DamageInfo(damage);
            //Send the sender (enemy) that inflict this damage.
            info.Sender = Sender;
            c.transform.GetComponent<bl_DamageCallback>().OnDamage(info);
            //And the other important variable is the position of enemy.
            //for this is we need to have a reference of enemy to do the following:
            Sender.SetIndicator();
            //When do this we send the position of sender reference.
            //if you need send a custom position, you can do like this
            //-----Sender.SetIndicator((Vector3)CustomPosition);

            AudioSource.PlayClipAtPoint(BounceAudio, c.transform.position, 1.0f);
        }
        else
        {
            int posibilty = Random.Range(0, 3);
            if(posibilty == 1 || EvenSound)
            {
                AudioSource.PlayClipAtPoint(BounceAudio, c.transform.position, 1.0f);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerEnter(Collider c)
    {
        if(c.transform.tag == "Player")
        {
            Sender.SetIndicator(Color.white);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Start()
    {
        yield return new WaitForSeconds(8);
        GetComponent<Rigidbody>().AddForce(Vector3.up * 100, ForceMode.Impulse);
    }
}