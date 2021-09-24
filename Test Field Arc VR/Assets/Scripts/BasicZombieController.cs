using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombieController : MonoBehaviour
{

    public float WalkingSpeed = 2f;
    public float RunningSpeed = 5f;

    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    bool isGrounded;
    Vector3 velocity;

    public SelectableAreaBox DesPicker;

    public Transform Target;

    public float AggressionRange = 5f;

    public float attackRange = 0.5f;

    public ObjectHealth objHealth;

    public Animator anim;
    CharacterController controller;

    public float attackTime = 0.7f;

    float t = 0f;

    bool IsAttacking = false;

    Vector3 GoTo;

    public Transform DeathPos;

    bool Stop = true;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //anim = GetComponent<Animator>();

        GoTo = DesPicker.GetRandomPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (Stop)
            return;
        if (objHealth.HP <= 0)
        {
            anim.SetBool("isDead", true);
            DeathState();
        }
        else
        {
            Vector3 MyPos = new Vector3(transform.position.x, GoTo.y, transform.position.z);
            //*
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -1f;
            }

            if (!isGrounded)
                velocity.y += gravity * Time.deltaTime;//*/
            controller.Move(velocity * Time.deltaTime);

            StateControl(MyPos);
        }

    }

    void passiveState(Vector3 MyPos)
    {
        if (GoTo == null)
        {
            GoTo = DesPicker.GetRandomPoint();
        }

        if (Vector3.Distance(MyPos, GoTo) < 0.2)
            GoTo = DesPicker.GetRandomPoint();

        bool run = false;

        Vector3 move = GoTo - MyPos;
        move.Normalize();

        if (!run)
            controller.Move(move * WalkingSpeed * Time.deltaTime);
        else
            controller.Move(move * RunningSpeed * Time.deltaTime);

        transform.rotation = Quaternion.LookRotation(move, Vector3.up);
        //controller.Move(velocity * Time.deltaTime);
    }

    void AggressiveState(Vector3 MyPos)
    {
        MyPos.y = Target.position.y;

        if (Vector3.Distance(MyPos, Target.position) <= attackRange || IsAttacking)
        {
            AttackState();
        }
        else
        {

            bool run = false;

            Vector3 move = Target.position - MyPos;
            move.Normalize();

            if (!run)
                controller.Move(move * WalkingSpeed * Time.deltaTime);
            else
                controller.Move(move * RunningSpeed * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(move, Vector3.up);
        }

    }

    void StateControl(Vector3 MyPos)
    {
        MyPos.y = Target.position.y;

        if (Vector3.Distance(MyPos, Target.position) <= AggressionRange)
        {
            AggressiveState(MyPos);
        }
        else
        {
            MyPos.y = GoTo.y;
            passiveState(MyPos);
        }

    }

    void DeathState()
    {
        t += Time.deltaTime;

        if (t > 5f)
        {
            Stop = true;

            transform.position = DeathPos.position;
            gameObject.SetActive(false);
        }
    }

    void AttackState()
    {
        if (!IsAttacking)
        {
            IsAttacking = true;
            anim.SetTrigger("Attack");
        }

        t += Time.deltaTime;

        if (t >= attackTime)
        {
            IsAttacking = false;
            t = 0f;
        }
    }

    public void Spawn(Vector3 pos)
    {
        objHealth.ResetHealth(5);
        //move Zombie object to pos
        transform.position = pos;
        //activate zombie
        Stop = false;
        t = 0f;
        anim.SetBool("isDead", false);
    }

    public void Spawn()
    {

        objHealth.ResetHealth(5);
        //Get pos from DesPicker
        Vector3 newPosition = DesPicker.GetRandomPoint();

        //Debug.Log(newPosition.x + " " + newPosition.z);
        //move Zombie object to pos
        transform.position = newPosition;

        while (transform.position != newPosition)
        {
            transform.position = newPosition;
        }
        //activate zombie
        Stop = false;
        t = 0f;
        anim.SetBool("isDead", false);
    }

    public void ZomStart()
    {
        Stop = false;
    }
}
