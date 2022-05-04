using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	public AK.Wwise.Event pjump;

	// grounded fix: change the bottom circle collider and only change isGrounded on collison with only that collider
	[SerializeField] public float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = 0.1f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	public Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	[Space]
	[SerializeField]
	Animator PlayerAnim;

	bool jumped = false;
    private bool canJump = false;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		//OnLandEvent.AddListener(OnLandFunc);
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapBoxAll(m_GroundCheck.position, new Vector2(0.43f, k_GroundedRadius), 0f, m_WhatIsGround);
		
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != this.gameObject)
			{
				m_Grounded = true;
				canJump = true;

				if (!wasGrounded)
				{
					OnLandEvent.Invoke();
                    if (jumped)
                    {
						jumped = false;
						PlayerAnim.SetTrigger("Land");
						GameObject.FindObjectOfType<CameraShake>()?.shakeCam(1.1f, 0.1f, true);
					}
				}
			}
		}

        if (!m_Grounded && wasGrounded && !jumped)
        {
			StartCoroutine(EdgeJump());
        }

		Debug.Log(PlayerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash);
		if(m_Grounded && PlayerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == 788460410)
        {
			PlayerAnim.SetTrigger("Land");
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (jump && canJump)
		{
			// Add a vertical force to the player.
			pjump.Post(gameObject);
			m_Grounded = false;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			PlayerAnim.SetTrigger("Jump");
			jumped = true;
			canJump = false;
		}
	}

	void OnLandFunc()
    {
		PlayerAnim.SetTrigger("Land");
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FloorBoost"))
        {
			Debug.Log("boostio");
			m_JumpForce = 2000f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("FloorBoost"))
        {
			m_JumpForce = 700f;
        }
    }

	IEnumerator EdgeJump()
    {
		yield return new WaitForSeconds(0.1f);
		canJump = false;
    }

    /*private void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
		Gizmos.DrawCube(m_GroundCheck.position, new Vector3(0.55f, k_GroundedRadius, 1));
	}*/
}
