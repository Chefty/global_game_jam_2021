﻿using System.Collections;
using UnityEngine;
public enum eState {
    idle,
    jump,
    walk,
    unhappy,
    idea,
    swim,
    death
}

public class PlayerMovement : MonoBehaviour {
    private Animator animator;
    private float timeIdle = 0f;
    public eState currentState;
    public eState secondState;
    private Vector3 targetPosition;
    private Vector3 targetRotation;
    public bool isLerping = false;

    public float timeToMove = 1f;
    public float rotationSmoothFactor = 350f;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        targetPosition = transform.localPosition;
        GameManager.Instance.onDieOnLava += StartActionCO;
    }

    private void FixedUpdate() {
        switch (currentState) {
            case eState.idle:
                timeIdle += Time.deltaTime;
                if (timeIdle > 8f) {
                    currentState = Random.Range(0f, 1f) > .5f ? eState.idea : eState.unhappy;
                    timeIdle = 0f;
                }
                break;
            case eState.jump:
                //nothing yet
                break;
            case eState.unhappy:
            case eState.idea:
                StartCoroutine(ActionCO());
                break;
            case eState.swim:
                //nothing yet
                break;
            default:
                break;
        }
    }

    public void Action(Vector3 direction, eState newState, eState newState2 = eState.idle) {

        if (!isLerping &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(eState.walk.ToString()) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(eState.jump.ToString())) {

            if (newState != eState.swim)
                animator.SetBool(currentState.ToString(), false);

            currentState = newState;
            secondState = newState2;


            targetPosition += direction;
            targetRotation = direction;
            StartCoroutine(ActionCO());
            StartCoroutine(MovePlayerLerpCO(targetPosition, targetRotation, timeToMove));
        }
    }

    private void StartActionCO() {
        StartCoroutine(ActionCO());
    }

    private IEnumerator ActionCO() {

        if (currentState == eState.walk) { //trigger animation
            animator.SetTrigger(currentState.ToString());
            yield return null;
        } else if (currentState == eState.swim && secondState == eState.idle) { //swim only
            animator.SetBool(currentState.ToString(), true);
            yield return null;
        } else if (currentState == eState.jump && secondState == eState.swim) { //swim is a long lasting animation, should be interupt manually
            animator.SetBool(currentState.ToString(), true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(currentState.ToString(), false);

            currentState = secondState;
            secondState = eState.idle;
            animator.SetBool(currentState.ToString(), true);
            Instantiate(Resources.Load("DeathParticles_water"), transform);

            //check if he has the ability
            if (!GameManager.Instance.DoesPlayerPosessAbility(typeof(Swim))) {
                currentState = eState.death;
                animator.SetBool(currentState.ToString(), true);
                StartCoroutine(MovePlayerLerpCO(transform.localPosition + new Vector3(0, -3f, 0), Vector3.zero, 3f));
            }
        } else if (currentState == eState.jump && secondState == eState.unhappy) { //getting out of water
            animator.SetBool(currentState.ToString(), true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(currentState.ToString(), false);

            currentState = secondState;
            animator.SetBool(currentState.ToString(), true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(currentState.ToString(), false);
            currentState = eState.idle;
            timeIdle = 0f;
        } else if (currentState == eState.death && secondState == eState.idle) { //dying on lava
            Instantiate(Resources.Load("DeathParticles_lava"), transform);
            animator.SetBool(currentState.ToString(), true);
            StartCoroutine(MovePlayerLerpCO(transform.localPosition + new Vector3(0, -2f, 0), Vector3.zero, 3f));
        }
        else if (currentState == eState.swim && secondState == eState.death)
        { //dying in water
            Instantiate(Resources.Load("DeathParticles_water"), transform);
            animator.SetBool(currentState.ToString(), true);
            StartCoroutine(MovePlayerLerpCO(transform.localPosition + new Vector3(0, -2f, 0), Vector3.zero, 3f));
        }
        else {
            animator.SetBool(currentState.ToString(), true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(currentState.ToString(), false);

            currentState = eState.idle;
            timeIdle = 0f;
        }
    }

    IEnumerator MovePlayerLerpCO(Vector3 targetPosition, Vector3 targetRotation, float duration) {
        isLerping = true;
        float time = 0;
        Vector3 startPosition = transform.localPosition;

        while (time < duration) {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            if (targetRotation != Vector3.zero)
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.LookRotation(targetRotation, Vector3.up), rotationSmoothFactor * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        if (currentState != eState.swim)
            animator.SetBool(currentState.ToString(), false); //tile reached - end animation
        transform.localPosition = targetPosition;
        isLerping = false;
        timeIdle = 0f;

        if (GameManager.Instance.onMoving != null)
        {
            GameManager.Instance.onMoving();
        }
    }
}
