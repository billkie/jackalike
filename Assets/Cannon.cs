using UnityEngine;
using XInputDotNetPure;

public class Cannon : MonoBehaviour {

    public GameObject cannon;

    PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    
    public Transform rocketSpawn;
    //public Transform muzzle;
    public GameObject rocketPrefab;
    public float fireRate = 0.5f;
    //public ParticleSystem muzzleFlash;

    private float nextTimeToFire = 0f;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {

        prevState = state;
        state = GamePad.GetState(playerIndex);

        if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            GameObject rocket = Instantiate(rocketPrefab, rocketSpawn.position, rocketSpawn.rotation);
            //Destroy(rocket, 5f);
        }
    }

}
