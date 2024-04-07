using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class scp096 : MonoBehaviourPun
{
    public Transform faceTransform;
    public float rageThreshold = 0.5f;
    public float moveSpeed = 5f;
    public NavMeshAgent Navmeshagent;
    public AudioSource ScreamSound;
    public float Screamlength;
    private bool isRageMode = false;
    private PhotonView photonView;
    public Transform playerToChase;
    public string chasetag;
    public float chaseRange; 
    public bool yes = true;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Screamlength = ScreamSound.clip.length;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (isRageMode)
            {
                findplayer();
                StartCoroutine(Rage());
            }
        }
    }

    public void PlayerLookingAtFace()
    {
        if (!isRageMode)
        {
            photonView.RPC("EnterRageMode", RpcTarget.All);
        }
    }

    public void PlayerNotLookingAtFace()
    {
        if (isRageMode)
        {
            photonView.RPC("ExitRageMode", RpcTarget.All);
        }
    }

    void ChasePlayer()
    {
        if (yes)
        {
            Navmeshagent.SetDestination(playerToChase.position);
        }
    }

    [PunRPC]
    void EnterRageMode()
    {
        isRageMode = true;
    }

    [PunRPC]
    void ExitRageMode()
    {
        isRageMode = false;
        playerToChase = null;
    }

    IEnumerator Rage()
    {
        findplayer();
        ScreamSound.Play();
        yield return new WaitForSeconds(Screamlength);
        yes = true;
        ChasePlayer();
    }

    public void findplayer()
    {
        if (photonView.IsMine)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(chasetag);
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;

            foreach (GameObject player in players)
            {
                float distanceToTarget = Vector3.Distance(gameObject.transform.position, player.transform.position);
                distanceToTarget = Mathf.Abs(distanceToTarget);

                if (distanceToTarget <= chaseRange && distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = player.transform;
                }
            }
        }
    }
}

