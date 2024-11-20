using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        bool verticalCollisionDetected;
        do
        {
            verticalCollisionDetected = false;
            for (int i = 0; i < allObjects.Length; i++)
            {
                for (int j = i + 1; j < allObjects.Length; j++)
                {
                    DynamicObject obj1 = allObjects[i];
                    DynamicObject obj2 = allObjects[j];
                    if (obj1.CollidesWith(obj2))
                    {
                        verticalCollisionDetected = true;
                        DynamicObject downObject;
                        DynamicObject upObject;
                        if (obj1.bounds.max.y - obj2.bounds.min.y < obj2.bounds.max.y - obj1.bounds.min.y)
                        {
                            downObject = obj1;
                            upObject = obj2;
                        }
                        else
                        {
                            downObject = obj2;
                            upObject = obj1;
                        }

                        float overlap = downObject.bounds.max.y - upObject.bounds.min.y;

                        List<DynamicObject> downObjects = downObject.GetAdjacencyList(DynamicObject.Direction.DOWN);
                        List<DynamicObject> upObjects = upObject.GetAdjacencyList(DynamicObject.Direction.UP);

                        float massDown = downObject.mass + downObjects.Aggregate(0.0f, (acc, obj) => acc + obj.mass);
                        float massUp = upObject.mass + upObjects.Aggregate(0.0f, (acc, obj) => acc + obj.mass);
                        float totalMass = massDown + massUp;

                        downObject.bounds.center -= Vector3.up * (overlap * massUp/totalMass + collisionTolerance);
                        upObject.bounds.center += Vector3.up * (overlap * massDown/totalMass + collisionTolerance);

                        downObjects.ForEach(obj => obj.bounds.center -= Vector3.up * (overlap * massUp / totalMass));
                        upObjects.ForEach(obj => obj.bounds.center += Vector3.up * (overlap * massUp / totalMass));

                        downObject.Adjacencies[DynamicObject.Direction.UP] = upObject;
                        upObject.Adjacencies[DynamicObject.Direction.DOWN] = downObject;
                    }
                }
            }
        } while (verticalCollisionDetected);

        // TODO: repeat horizontally
        // move all objects horizontally
        foreach (DynamicObject obj in allObjects)
        {
            if (obj.velocity.x != 0.0f)
            {
                obj.bounds.center += Vector3.right * obj.velocity.x;
            }
        }

        bool horizontalCollisionDetected;
        do
        {
            horizontalCollisionDetected = false;
            for (int i = 0; i < allObjects.Length; i++)
            {
                for (int j = i + 1; j < allObjects.Length; j++)
                {
                    DynamicObject obj1 = allObjects[i];
                    DynamicObject obj2 = allObjects[j];
                    if (obj1.CollidesWith(obj2))
                    {
                        horizontalCollisionDetected = true;
                        DynamicObject leftObject;
                        DynamicObject rightObject;
                        if (obj1.bounds.max.x - obj2.bounds.min.x < obj2.bounds.max.x - obj1.bounds.min.x)
                        {
                            leftObject = obj1;
                            rightObject = obj2;
                        }
                        else
                        {
                            leftObject = obj2;
                            rightObject = obj1;
                        }

                        float overlap = leftObject.bounds.max.x - rightObject.bounds.min.x;

                        List<DynamicObject> leftObjects = leftObject.GetAdjacencyList(DynamicObject.Direction.LEFT);
                        List<DynamicObject> rightObjects = rightObject.GetAdjacencyList(DynamicObject.Direction.RIGHT);

                        float massLeft = leftObject.mass + leftObjects.Aggregate(0.0f, (acc, obj) => acc + obj.mass);
                        float massRight = rightObject.mass + rightObjects.Aggregate(0.0f, (acc, obj) => acc + obj.mass);
                        float totalMass = massLeft + massRight;

                        leftObject.bounds.center -= Vector3.right * (overlap * massRight / totalMass + collisionTolerance);
                        rightObject.bounds.center += Vector3.right * (overlap * massLeft / totalMass + collisionTolerance);

                        leftObjects.ForEach(obj => obj.bounds.center -= Vector3.right * (overlap * massRight / totalMass));
                        rightObjects.ForEach(obj => obj.bounds.center += Vector3.right * (overlap * massLeft / totalMass));

                        leftObject.Adjacencies[DynamicObject.Direction.RIGHT] = rightObject;
                        rightObject.Adjacencies[DynamicObject.Direction.LEFT] = leftObject;
                    }
                }
            }
        } while (horizontalCollisionDetected);

        // move all the objects based on their bounds
        foreach (DynamicObject obj in allObjects)
        {
            obj.transform.position = obj.bounds.center;
        }
    }


}