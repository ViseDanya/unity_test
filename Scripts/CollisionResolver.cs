using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CollisionResolver : MonoBehaviour
{
    const float collisionTolerance = .001f;
    public void FixedUpdate()
    {

        DynamicObject[] dynamicObjects = FindObjectsByType<DynamicObject>(FindObjectsSortMode.None);
        StaticObject[] staticObjects = FindObjectsByType<StaticObject>(FindObjectsSortMode.None);

        // TODO: Potentially can just check static collisions after dynamic.
        // would just have to check dynamic/dynamic again, but just adjusting for static collision changes
        // You definitely have to at least check after, because a dynamic object could have been pushed into a static object
        // You might also have to check before in order to apply the platform specific effects, but maybe not?

        // move and collide all objects vertically
        Array.ForEach(dynamicObjects, obj => obj.bounds.center += Vector3.up * obj.velocity.y);
        CollideDynamicWithStaticVertical(dynamicObjects, staticObjects);
        CollideDynamicWithDynamicVertical(dynamicObjects);
        CollideDynamicWithStaticVertical(dynamicObjects, staticObjects);

        // move all objects horizontally
        Array.ForEach(dynamicObjects, obj => obj.bounds.center += Vector3.right * obj.velocity.x);
        CollideDynamicWithStaticHorizontal(dynamicObjects, staticObjects);
        CollideDynamicWithDynamicHorizontal(dynamicObjects);
        CollideDynamicWithStaticHorizontal(dynamicObjects, staticObjects);

        Array.ForEach(dynamicObjects, obj => obj.transform.position = obj.bounds.center);
    }

    private void CollideDynamicWithStaticVertical(DynamicObject[] dynamicObjects, StaticObject[] staticObjects)
    {
        bool verticalCollisionDetected;
        do
        {
            verticalCollisionDetected = false;
            foreach(DynamicObject dynamicObj in dynamicObjects)
            {
                foreach (StaticObject staticObj in staticObjects)
                {
                    if (dynamicObj.CollidesWith(staticObj))
                    {
                        verticalCollisionDetected = true;
                        float dynamicStaticOverlap = dynamicObj.bounds.max.y - staticObj.bounds.min.y;
                        float staticDynamicOverlap = staticObj.bounds.max.y - dynamicObj.bounds.min.y;
                        if (dynamicStaticOverlap < staticDynamicOverlap)
                        {
                            dynamicObj.isOnCeiling = true;
                            dynamicObj.bounds.center -= Vector3.up * (dynamicStaticOverlap + collisionTolerance);
                            List<DynamicObject> downObjects = dynamicObj.GetAdjacencyList(DynamicObject.Direction.DOWN);
                            foreach (DynamicObject obj in downObjects)
                            {
                                obj.isOnCeiling = true;
                                obj.bounds.center -= Vector3.up * dynamicStaticOverlap;
                            }
                        }
                        else
                        {
                            dynamicObj.isOnFloor = true;
                            dynamicObj.bounds.center += Vector3.up * (staticDynamicOverlap + collisionTolerance);
                            List<DynamicObject> upObjects = dynamicObj.GetAdjacencyList(DynamicObject.Direction.UP);
                            foreach (DynamicObject obj in upObjects)
                            {
                                obj.isOnFloor = true;
                                obj.bounds.center += Vector3.up * staticDynamicOverlap;
                            }
                        }
                    }
                }
            }

        } while (verticalCollisionDetected);
    }

    private void CollideDynamicWithStaticHorizontal(DynamicObject[] dynamicObjects, StaticObject[] staticObjects)
    {
        bool horizontalCollisionDetected;
        do
        {
            horizontalCollisionDetected = false;
            foreach (DynamicObject dynamicObj in dynamicObjects)
            {
                foreach (StaticObject staticObj in staticObjects)
                {
                    if (dynamicObj.CollidesWith(staticObj))
                    {
                        horizontalCollisionDetected = true;
                        float dynamicStaticOverlap = dynamicObj.bounds.max.x - staticObj.bounds.min.x;
                        float staticDynamicOverlap = staticObj.bounds.max.x - dynamicObj.bounds.min.x;
                        if (dynamicStaticOverlap < staticDynamicOverlap)
                        {
                            dynamicObj.isOnWallRight = true;
                            dynamicObj.bounds.center -= Vector3.right * (dynamicStaticOverlap + collisionTolerance);
                            List<DynamicObject> leftObjects = dynamicObj.GetAdjacencyList(DynamicObject.Direction.LEFT);
                            foreach (DynamicObject obj in leftObjects)
                            {
                                obj.isOnWallRight = true;
                                obj.bounds.center -= Vector3.right * dynamicStaticOverlap;
                            }
                        }
                        else
                        {
                            dynamicObj.isOnWallLeft = true;
                            dynamicObj.bounds.center += Vector3.right * (staticDynamicOverlap + collisionTolerance);
                            List<DynamicObject> rightObjects = dynamicObj.GetAdjacencyList(DynamicObject.Direction.RIGHT);
                            foreach (DynamicObject obj in rightObjects)
                            {
                                obj.isOnWallLeft = true;
                                obj.bounds.center += Vector3.right * staticDynamicOverlap;
                            }
                        }
                    }
                }
            }

        } while (horizontalCollisionDetected);
    }

    private void CollideDynamicWithDynamicVertical(DynamicObject[] dynamicObjects)
    {
        bool verticalCollisionDetected;
        do
        {
            verticalCollisionDetected = false;
            for (int i = 0; i < dynamicObjects.Length-1; i++)
            {
                for (int j = i+1; j < dynamicObjects.Length; j++)
                {
                    DynamicObject obj1 = dynamicObjects[i];
                    DynamicObject obj2 = dynamicObjects[j];
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

                        if (downObject.isOnFloor)
                        {
                            upObject.isOnFloor = true;
                            upObject.bounds.center += Vector3.up * (overlap + collisionTolerance);
                            foreach(DynamicObject obj in upObjects)
                            {
                                upObject.isOnFloor = true;
                                upObject.bounds.center += Vector3.up * overlap;
                            }
                        }
                        else if(upObject.isOnCeiling)
                        {
                            downObject.isOnCeiling = true;
                            downObject.bounds.center -= Vector3.up * (overlap + collisionTolerance);
                            foreach (DynamicObject obj in upObjects)
                            {
                                downObject.isOnCeiling = true;
                                downObject.bounds.center -= Vector3.up * overlap;
                            }
                        }
                        else
                        {
                            float massDown = downObject.mass + downObjects.Aggregate(0.0f, (acc, obj) => acc + obj.mass);
                            float massUp = upObject.mass + upObjects.Aggregate(0.0f, (acc, obj) => acc + obj.mass);
                            float totalMass = massDown + massUp;

                            downObject.bounds.center -= Vector3.up * (overlap * massUp / totalMass + collisionTolerance);
                            upObject.bounds.center += Vector3.up * (overlap * massDown / totalMass + collisionTolerance);

                            downObjects.ForEach(obj => obj.bounds.center -= Vector3.up * (overlap * massUp / totalMass));
                            upObjects.ForEach(obj => obj.bounds.center += Vector3.up * (overlap * massDown / totalMass));
                        }

                        downObject.Adjacencies[DynamicObject.Direction.UP] = upObject;
                        upObject.Adjacencies[DynamicObject.Direction.DOWN] = downObject;
                    }
                }
            }
        } while (verticalCollisionDetected);
    }

    private void CollideDynamicWithDynamicHorizontal(DynamicObject[] dynamicObjects)
    {
        bool horizontalCollisionDetected;
        do
        {
            horizontalCollisionDetected = false;
            for (int i = 0; i < dynamicObjects.Length-1; i++)
            {
                for (int j = i+1; j < dynamicObjects.Length; j++)
                {
                    DynamicObject obj1 = dynamicObjects[i];
                    DynamicObject obj2 = dynamicObjects[j];
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
    }


}