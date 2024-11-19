using UnityEngine;
using System.Collections.Generic;

public class CollisionResolver : MonoBehaviour
{
    const float collisionTolerance = .001f;
    public void FixedUpdate()
    {

        // New plan: collide all players first and form all "OneThings" using conservation of momentum
        // Then, collide all the things into the walls
        // This makes more sense than colliding into wallks first, because a player could push another player into a wall,
        // so we'd have to check wall collisions again anyway.

        // ah but what if you have a chain of players, which after getting corrected out of the wall, hit a player that they did not originally hit...

        DynamicObject[] allObjects = FindObjectsByType<DynamicObject>(FindObjectsSortMode.None);

        // move all objects vertically
        foreach (DynamicObject obj in allObjects)
        {
            if (obj.velocity.y != 0.0f)
            {
                obj.bounds.center += Vector3.up * obj.velocity.y;
            }
        }

        // check each pair of dynamic objects and find the collision that occured first
        // since we already moved the objects, we want to find the longestTimeSinceCollision
        DynamicObject firstCollisionObj1 = null;
        DynamicObject firstCollisionObj2 = null;
        float longestTimeSinceCollision = 0.0f;
        for (int i = 0; i < allObjects.Length; i++)
        {
            for (int j = i + 1; j < allObjects.Length; j++)
            {
                DynamicObject obj1 = allObjects[i];
                DynamicObject obj2 = allObjects[j];
                if(obj1.CollidesWith(obj2))
                {
                    // find collision time
                    float obj1EndPos;
                    float obj2EndPos;

                    float obj1Max = obj1.bounds.max.y;
                    float obj1Min = obj1.bounds.min.y;
                    float obj2Max = obj2.bounds.max.y;
                    float obj2Min = obj2.bounds.min.y;
                    if (obj1Max - obj2Min < obj2Max - obj1Min)
                    {
                        obj1EndPos = obj1Max;
                        obj2EndPos = obj2Min;
                    }
                    else
                    {
                        obj1EndPos = obj1Min;
                        obj2EndPos = obj2Max;
                    }

                    float timeSinceCollision = (obj1EndPos - obj2EndPos) / (obj1.velocity.y - obj2.velocity.y);
                    // we know the objects intersected because we did an intersection check,
                    // so t must be between 0 and 1 already.
                    if (timeSinceCollision > longestTimeSinceCollision)
                    {
                        firstCollisionObj1 = obj1;
                        firstCollisionObj2 = obj2;
                        longestTimeSinceCollision = timeSinceCollision;
                        Debug.Log("Vertical overlap: " + (obj1EndPos - obj2EndPos));
                    }
                }
            }
        }

        if (longestTimeSinceCollision > 0.0f)
        {
            // resolve collision by applying time backwards
            // go to time slightly before collision to avoid floating point issues
            // TODO: figure out if above condition needs to be changed also to avoid floating point issues.
            // > collisionTolerance? probably not because then we might still have overlap when resolving future collisions
            float obj1VelocityAfterCollision = firstCollisionObj1.velocity.y * (longestTimeSinceCollision + collisionTolerance);
            float obj2VelocityAfterCollision = firstCollisionObj2.velocity.y * (longestTimeSinceCollision + collisionTolerance);
            firstCollisionObj1.bounds.center -= Vector3.up * obj1VelocityAfterCollision;
            firstCollisionObj2.bounds.center -= Vector3.up * obj2VelocityAfterCollision;

            Debug.Log("obj1 center: " + firstCollisionObj1.bounds.center.y);
            Debug.Log("obj2 center: " + firstCollisionObj2.bounds.center.y);

            // figure out how much to move objects based on conservation of momentum
            // TODO: apply left/right/up/down adjacencies to get correct mass and move all objects together
            // currently, just average velocities
            float conservedVelocity = (obj1VelocityAfterCollision + obj2VelocityAfterCollision) / 2.0f;

            // apply velocity to both objects
            firstCollisionObj1.bounds.center += Vector3.up * conservedVelocity;
            firstCollisionObj2.bounds.center += Vector3.up * conservedVelocity;
        }

        Debug.Log("Vertical collision time: " + longestTimeSinceCollision);

        // TODO: repeat horizontally
        // move all objects horizontally
        foreach (DynamicObject obj in allObjects)
        {
            if (obj.velocity.x != 0.0f)
            {
                obj.bounds.center += Vector3.right * obj.velocity.x;
            }
        }

        // check each pair of dynamic objects and find the collision that occured first
        // since we already moved the objects, we want to find the longestTimeSinceCollision
        firstCollisionObj1 = null;
        firstCollisionObj2 = null;
        longestTimeSinceCollision = 0.0f;
        for (int i = 0; i < allObjects.Length; i++)
        {
            for (int j = i + 1; j < allObjects.Length; j++)
            {
                DynamicObject obj1 = allObjects[i];
                DynamicObject obj2 = allObjects[j];
                if (obj1.CollidesWith(obj2))
                {
                    // find collision time
                    float obj1EndPos;
                    float obj2EndPos;

                    float obj1Max = obj1.bounds.max.x;
                    float obj1Min = obj1.bounds.min.x;
                    float obj2Max = obj2.bounds.max.x;
                    float obj2Min = obj2.bounds.min.x;
                    if (obj1Max - obj2Min < obj2Max - obj1Min)
                    {
                        obj1EndPos = obj1Max;
                        obj2EndPos = obj2Min;
                    }
                    else
                    {
                        obj1EndPos = obj1Min;
                        obj2EndPos = obj2Max;
                    }

                    // TODO: ideally, overlap would be 0 if we are just moving vertically, but because of floating point precision,
                    // we can end up still thinking that we overlap after resolving a vertical collision,
                    // so then we get timeSinceCollision = infinity.
                    // How can we avoid this? use skin width?
                    float timeSinceCollision = (obj1EndPos - obj2EndPos) / (obj1.velocity.x - obj2.velocity.x);
                    // we know the objects intersected because we did an intersection check,
                    // so t must be between 0 and 1 already.
                    if (timeSinceCollision > longestTimeSinceCollision)
                    {
                        firstCollisionObj1 = obj1;
                        firstCollisionObj2 = obj2;
                        longestTimeSinceCollision = timeSinceCollision;
                        Debug.Log("Horizontal overlap: " + (obj1EndPos - obj2EndPos));
                    }
                }
            }
        }

        if (longestTimeSinceCollision > 0.0f)
        {
            // resolve collision by applying time backwards
            float obj1VelocityAfterCollision = firstCollisionObj1.velocity.x * (longestTimeSinceCollision + collisionTolerance);
            float obj2VelocityAfterCollision = firstCollisionObj2.velocity.x * (longestTimeSinceCollision + collisionTolerance);
            firstCollisionObj1.bounds.center -= Vector3.right * obj1VelocityAfterCollision;
            firstCollisionObj2.bounds.center -= Vector3.right * obj2VelocityAfterCollision;

            Debug.Log("horizontal obj1 center: " + firstCollisionObj1.bounds.center.x);
            Debug.Log("horizontal obj2 center: " + firstCollisionObj2.bounds.center.x);

            // figure out how much to move objects based on conservation of momentum
            // TODO: apply left/right/up/down adjacencies to get correct mass and move all objects together
            // currently, just average velocities
            float conservedVelocity = (obj1VelocityAfterCollision + obj2VelocityAfterCollision) / 2.0f;

            // apply velocity to both objects
            firstCollisionObj1.bounds.center += Vector3.right * conservedVelocity;
            firstCollisionObj2.bounds.center += Vector3.right * conservedVelocity;
        }

        Debug.Log("Horizontal collision time: " + longestTimeSinceCollision);

        // move all the objects based on their bounds
        foreach (DynamicObject obj in allObjects)
        {
            obj.transform.position = obj.bounds.center;
        }
    }


}