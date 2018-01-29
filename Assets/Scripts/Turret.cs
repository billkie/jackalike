using UnityEngine;
using XInputDotNetPure;

public class Turret : MonoBehaviour {

    public GameObject turret;

    PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    private float angle;
    public float bulletSpeed = 10f;
    public Transform bulletSpawn;
    public Transform muzzle;
    public GameObject bulletPrefab;
    public float fireRate = 15f;
    public ParticleSystem muzzleFlash;

    private float nextTimeToFire = 0f;
    private float x, y;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        prevState = state;
        state = GamePad.GetState(playerIndex);

        x = state.ThumbSticks.Right.X;
        y = state.ThumbSticks.Right.Y;
        if ((Mathf.Abs(x) + Mathf.Abs(y) > 0.5f) && Time.time >= nextTimeToFire)
        {
            muzzleFlash.Play();
            nextTimeToFire = Time.time + 1f / fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(turret.transform.forward * bulletSpeed);
        }
	}

    private void Update()
    {
        if (x != 0.0f || y != 0.0f)
        {
            angle = (Mathf.Atan2(x, y) * Mathf.Rad2Deg) - transform.eulerAngles.y - 45; // - transform.eulerAngles.y;
            //Debug.Log(transform.eulerAngles.y);
            
        }
        //Vector3 v = turret.transform.localEulerAngles;
        turret.transform.localEulerAngles = new Vector3(0, angle, 0);
        //turret.transform.eulerAngles = new Vector3(turret.transform.rotation.x, angle, turret.transform.rotation.z);
    }
}
