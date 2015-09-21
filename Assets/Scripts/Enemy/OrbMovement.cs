using UnityEngine;
using System.Collections;

public class OrbMovement : MonoBehaviour
{

    public int startCorner;//must be value from 0 - 3
    public bool moveClockwise;
    Vector3[] patrolPositions;

    bool roomSet;
    bool patrolling;

    float speed = 10;
    int nextPatrolPos;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(patrolling)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, patrolPositions[nextPatrolPos], step);

            if (transform.position == patrolPositions[nextPatrolPos])
            {

                if(moveClockwise)
                {
                    if (nextPatrolPos == 3)
                        nextPatrolPos = 0;
                    else
                        nextPatrolPos++;
                }

                else
                {
                    if (nextPatrolPos == 0)
                        nextPatrolPos = 3;
                    else
                        nextPatrolPos--;
                }
               
            }
        }     
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(!roomSet)
        {
            if (collider.tag == "RoomArea")
            {
                roomSet = true;
                SetPatrolPositions(collider);
            }
        }   
    }

    void SetPatrolPositions(Collider2D room)
    {

        float offset = 2;
        patrolPositions = new Vector3[4];

        patrolPositions[0] = new Vector3(room.bounds.min.x + offset, room.bounds.max.y - offset,0);
        patrolPositions[1] = new Vector3(room.bounds.max.x - offset, room.bounds.max.y - offset,0);
        patrolPositions[2] = new Vector3(room.bounds.max.x - offset, room.bounds.min.y + offset,0);
        patrolPositions[3] = new Vector3(room.bounds.min.x + offset, room.bounds.min.y + offset,0);

        transform.position = patrolPositions[startCorner];
        
        if(moveClockwise)
        {
            if (startCorner == 3)
                nextPatrolPos = 0;

            else
                nextPatrolPos = startCorner + 1;
        }  

        else
        {
            if (startCorner == 0)
                nextPatrolPos = 3;

            else
                nextPatrolPos = startCorner - 1;
        }

        patrolling = true;
    }
}
