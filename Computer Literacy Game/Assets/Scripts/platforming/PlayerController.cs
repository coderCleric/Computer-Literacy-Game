using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float horizSpeed;
    public float slowdownSpeed;
    public LayerMask groundLayer;
    public float terminalV;
    [SerializeField]
    private AudioSource walkAudio;
    [SerializeField]
    private AudioSource jumpAudio;
    [SerializeField]
    private AudioSource impactAudio;
    public static PlayerController Instance { get; private set; }

    //Saved for performance
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private SpriteRenderer rend;

    private bool wasGrounded = true;

    //Animation enum
    private enum AnimationState {IDLE, WALKING, JUMPING}

    // Awake is called before the first frame update
    void Awake()
    {
        //Get the different components of the objects
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        //Set the instance to this
        Instance = this;
    }

    /**
     * Need to apply a terminal V
     */
    private void FixedUpdate()
    {
        if (rb.velocity.y < -terminalV)
            rb.velocity = new Vector2(rb.velocity.x, -terminalV);
    }

    // Update is called once per frame
    void Update()
    {
        //Only allow input when in game mode
        bool walking = false;
        if (InputStateTracker.CheckGameState(InputStateTracker.GameState.PLAY))
        {
            //Jump when the jump key is pressed
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpAudio.Play();
            }

            //Get the horizontal direction
            float direction = Input.GetAxisRaw("Horizontal");

            //Move right when d is pressed
            if (direction == 1)
            {
                rb.velocity = new Vector2(horizSpeed, rb.velocity.y);
                walking = true;
            }

            //Move left when a is pressed
            else if (direction == -1)
            {
                rb.velocity = new Vector2(-1 * horizSpeed, rb.velocity.y);
                walking = true;
            }
        }

        //Auto stop when no movement is happening
        if(!walking)
        {
            float actualSlowdown = slowdownSpeed * Time.deltaTime;

            if (Mathf.Abs(rb.velocity.x) < actualSlowdown)
                rb.velocity = new Vector2(0, rb.velocity.y);
            else if (rb.velocity.x > 0)
                rb.velocity = new Vector2(rb.velocity.x - actualSlowdown, rb.velocity.y);
            else
                rb.velocity = new Vector2(rb.velocity.x + actualSlowdown, rb.velocity.y);
        }

        //Determine what animation to play
        DetermineAnim();

        //Figure out if we need to play the impact sound
        if(IsGrounded() && !wasGrounded)
            impactAudio.Play();
        wasGrounded = IsGrounded();
    }

    //Determines what animation should be played
    private void DetermineAnim()
    {
        float direction = 0;
        if (InputStateTracker.CheckGameState(InputStateTracker.GameState.PLAY))
        {
            direction = Input.GetAxisRaw("Horizontal");

            //Turn right if moving right
            if (direction == 1)
                rend.flipX = false;

            //Turn left if moving left
            if (direction == -1)
                rend.flipX = true;
        }

        //Update the animation state
        AnimationState state = AnimationState.IDLE;

        //If jumping, set the state to jumping
        if (Input.GetButtonDown("Jump") && IsGrounded() && InputStateTracker.CheckGameState(InputStateTracker.GameState.PLAY))
            state = AnimationState.JUMPING;

        //If moving at all, set the state to walking
        else if (direction != 0)
            state = AnimationState.WALKING;

        //Assign the new state
        anim.SetInteger("state", (int)state);

        //Update the vertspeed of the animator if we're not grounded (or set to 0 if we are)
        if (!IsGrounded())
            anim.SetFloat("vertSpeed", rb.velocity.y);
        else
            anim.SetFloat("vertSpeed", 0);

        //Determine what audio to play
        //If grounded and walking, play the walking loop
        if (IsGrounded() && state == AnimationState.WALKING && !walkAudio.isPlaying)
            walkAudio.Play();
        else if((!IsGrounded() || state != AnimationState.WALKING) && walkAudio.isPlaying)
        {
            walkAudio.Stop();
            Debug.Log("Stopping walk audio\nGrounded: " + IsGrounded() + "\nState: " + state + "\nAudio playing: " + walkAudio.isPlaying);
        }
    }

    //Determine whether or not the player is touching the ground
    private bool IsGrounded()
    {
        //Cast a box just below the player, see if it hits the ground
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
    }
}
