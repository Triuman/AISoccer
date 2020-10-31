using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Creates fields and keeps them in the pool.
/// Accepts the fields back when they are not in use.
/// Sets ignoreCollision between all fields, so that none of them can interact with each other.
/// </summary>
public class FieldManager : MonoBehaviour
{
    public Field FieldPrefab;

    private readonly List<Field> fieldsAvailable = new List<Field>();

    public Field[] GetFields(int count)
    {
        if (fieldsAvailable.Count < count)
        {
            InitNewFields(count - fieldsAvailable.Count);
        }
        var giveAwayFields = new Field[count];
        for (int f = 0; f < count; f++)
        {
            giveAwayFields[f] = fieldsAvailable[f];
        }
        fieldsAvailable.RemoveRange(0, count);
        return giveAwayFields;
    }

    private void InitNewFields(int count)
    {
        for (int nf = 0; nf < count; nf++)
        {
            var newField = Instantiate(FieldPrefab);
            foreach (var field in fieldsAvailable)
            {
                IgnoreCollision(newField, field);
            }
            fieldsAvailable.Add(newField);
        }
    }

    public void TakeFieldBack(Field field)
    {
        fieldsAvailable.Add(field);
    }
    public void TakeFieldsBack(Field[] fields)
    {
        fieldsAvailable.AddRange(fields);
    }

    private static void IgnoreCollision(Field field1, Field field2)
    {
        var agent1Collider = field1.agent.gameObject.GetComponent<Collider2D>();
        var agent2Collider = field2.agent.gameObject.GetComponent<Collider2D>();
        var ball1Collider = field1.ball.GetComponent<Collider2D>();
        var ball2Collider = field2.ball.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(agent1Collider, agent2Collider, true);
        Physics2D.IgnoreCollision(agent1Collider, ball2Collider, true);
        Physics2D.IgnoreCollision(agent2Collider, ball1Collider, true);
        Physics2D.IgnoreCollision(ball1Collider, ball2Collider, true);

        Physics2D.IgnoreCollision(field1.agent.gameObject.GetComponent<Collider2D>(), field2.agent.gameObject.GetComponent<Collider2D>());
        foreach (var collider1 in field1.colliders)
        {
            foreach (var collider2 in field2.colliders)
            {
                Physics2D.IgnoreCollision(agent1Collider, collider2, true);
                Physics2D.IgnoreCollision(ball1Collider, collider2, true);
                Physics2D.IgnoreCollision(agent2Collider, collider1, true);
                Physics2D.IgnoreCollision(ball2Collider, collider1, true);

                Physics2D.IgnoreCollision(collider1, collider2, true);
            }
        }
    }
}
